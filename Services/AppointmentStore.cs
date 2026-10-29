using System.Text.Json;
using KuaforRandevu.Models;
using Microsoft.JSInterop;

namespace KuaforRandevu.Services;

public sealed class AppointmentStore : IAsyncDisposable
{
    private const string StorageKey = "kuafor.appointments";
    private readonly IJSRuntime _js;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);
    private IJSObjectReference? _module;
    private List<Appointment>? _items;

    public AppointmentStore(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<IReadOnlyList<Appointment>> GetAllAsync()
    {
        if (_items is not null)
            return _items;

        try
        {
            var raw = await (await ModuleAsync()).InvokeAsync<string?>("getItem", StorageKey);
            _items = string.IsNullOrWhiteSpace(raw)
                ? []
                : JsonSerializer.Deserialize<List<Appointment>>(raw, _json) ?? [];
        }
        catch (Exception ex) when (ex is JSException or JsonException)
        {
            _items = [];
        }

        return _items;
    }

    public async Task AddAsync(Appointment appointment)
    {
        var items = await MutableAsync();
        items.Add(appointment);
        await PersistAsync();
    }

    public async Task<bool> CancelAsync(string id)
    {
        var items = await MutableAsync();
        var item = items.FirstOrDefault(appointment => appointment.Id == id);
        if (item is null || item.IsCancelled || item.StartAt <= DateTime.Now)
            return false;

        item.IsCancelled = true;
        await PersistAsync();
        return true;
    }

    public string CreateCode(DateTime when)
    {
        if (_items is null)
            throw new InvalidOperationException("Randevular yüklenmeden kod üretilemez.");

        var used = _items.Select(appointment => appointment.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var year = when.Year;
        for (var attempt = 0; attempt < 40; attempt++)
        {
            var code = $"KFR-{year}-{Random.Shared.Next(100, 1000)}";
            if (used.Add(code))
                return code;
        }

        return $"KFR-{year}-{Random.Shared.Next(1000, 10000)}";
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
            return;

        await _module.DisposeAsync();
        _module = null;
    }

    private async Task<List<Appointment>> MutableAsync()
    {
        await GetAllAsync();
        return _items!;
    }

    private async Task PersistAsync()
    {
        var json = JsonSerializer.Serialize(_items, _json);
        await (await ModuleAsync()).InvokeVoidAsync("setItem", StorageKey, json);
    }

    private async Task<IJSObjectReference> ModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "./js/storage.js");
}
