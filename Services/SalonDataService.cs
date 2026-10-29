using System.Net.Http.Json;
using KuaforRandevu.Models;

namespace KuaforRandevu.Services;

public sealed class SalonDataService
{
    private readonly HttpClient _http;
    private Task? _load;
    private IReadOnlyList<SalonService> _services = [];
    private IReadOnlyList<Employee> _employees = [];
    private IReadOnlyList<BusySlot> _busySlots = [];

    public SalonDataService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<SalonService>> GetServicesAsync()
    {
        await EnsureLoadedAsync();
        return _services;
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync()
    {
        await EnsureLoadedAsync();
        return _employees;
    }

    public async Task<IReadOnlyList<BusySlot>> GetBusySlotsAsync()
    {
        await EnsureLoadedAsync();
        return _busySlots;
    }

    private Task EnsureLoadedAsync() => _load ??= LoadAsync();

    private async Task LoadAsync()
    {
        var servicesTask = _http.GetFromJsonAsync<List<SalonService>>("data/services.json");
        var employeesTask = _http.GetFromJsonAsync<List<Employee>>("data/employees.json");
        var busyTask = _http.GetFromJsonAsync<List<BusySlot>>("data/busy-slots.json");
        await Task.WhenAll(servicesTask, employeesTask, busyTask);

        _services = await servicesTask ?? [];
        _employees = await employeesTask ?? [];
        _busySlots = await busyTask ?? [];
    }
}
