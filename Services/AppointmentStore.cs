using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using KuaforRandevu.Models;
using Microsoft.JSInterop;

namespace KuaforRandevu.Services;

public sealed class AppointmentStore : IAsyncDisposable
{
    private const string StorageKey = "kuafor.appointments";
    private readonly IJSRuntime _js;
    private readonly AvailabilityService _availability;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);
    private IJSObjectReference? _module;

    public AppointmentStore(IJSRuntime js, AvailabilityService availability)
    {
        _js = js;
        _availability = availability;
    }

    public async Task<IReadOnlyList<Appointment>> GetAllAsync()
    {
        var raw = await (await ModuleAsync()).InvokeAsync<string?>("getItem", StorageKey);
        return Deserialize(raw);
    }

    public Task<(bool Changed, IReadOnlyList<Appointment> Appointments)> TryAddAsync(
        Appointment appointment, IReadOnlyList<BusySlot> busySlots) => UpdateAsync(items =>
    {
        var start = appointment.StartAt;
        var duration = (int)(appointment.EndAt - start).TotalMinutes;
        if (!_availability.IsSelectable(DateOnly.FromDateTime(start), TimeOnly.FromDateTime(start),
                duration, appointment.EmployeeId, busySlots, items))
            return false;

        appointment.Id = CreateCode(start, items);
        items.Add(appointment);
        return true;
    });

    public Task<(bool Changed, IReadOnlyList<Appointment> Appointments)> CancelAsync(string id) => UpdateAsync(items =>
    {
        var item = items.FirstOrDefault(appointment => appointment.Id == id);
        if (item is null || item.IsCancelled || item.StartAt <= DateTime.Now)
            return false;

        item.IsCancelled = true;
        return true;
    });

    private static string CreateCode(DateTime when, IEnumerable<Appointment> items)
    {
        var used = items.Select(appointment => appointment.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var year = when.Year;
        for (var attempt = 0; attempt < 40; attempt++)
        {
            var code = $"KFR-{year}-{Random.Shared.Next(100, 1000)}";
            if (used.Add(code))
                return code;
        }

        var suffix = 1000;
        while (used.Contains($"KFR-{year}-{suffix}"))
            suffix++;
        return $"KFR-{year}-{suffix}";
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
            return;

        await _module.DisposeAsync();
        _module = null;
    }

    private List<Appointment> Deserialize(string? raw)
    {
        if (raw is null)
            return [];

        var items = JsonSerializer.Deserialize<List<Appointment>>(raw, _json);
        if (items is null || items.Any(item => item is null || item.ServiceIds is null))
            throw new JsonException("Saklanan randevular okunamadı.");

        return items;
    }

    private async Task<(bool Changed, IReadOnlyList<Appointment> Appointments)> UpdateAsync(
        Func<List<Appointment>, bool> update)
    {
        List<Appointment> items = [];
        using var callback = DotNetObjectReference.Create(new StorageUpdate(raw =>
        {
            items = Deserialize(raw);
            return update(items) ? JsonSerializer.Serialize(items, _json) : null;
        }));
        var changed = await (await ModuleAsync()).InvokeAsync<bool>("updateItem", StorageKey, callback);
        return (changed, items);
    }

    private async Task<IJSObjectReference> ModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "./js/storage.js");

    private sealed class StorageUpdate
    {
        private readonly Func<string?, string?> _apply;

        [DynamicDependency(nameof(Apply))]
        public StorageUpdate(Func<string?, string?> apply) => _apply = apply;

        [JSInvokable]
        public string? Apply(string? raw) => _apply(raw);
    }
}
