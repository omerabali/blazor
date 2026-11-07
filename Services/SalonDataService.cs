using System.Net.Http.Json;
using KuaforRandevu.Models;

namespace KuaforRandevu.Services;

public sealed class SalonDataService
{
    private readonly HttpClient _http;
    private Task? _catalogLoad;
    private Task<IReadOnlyList<BusySlot>>? _busyLoad;
    private IReadOnlyList<SalonService> _services = [];
    private IReadOnlyList<Employee> _employees = [];

    public SalonDataService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<SalonService>> GetServicesAsync()
    {
        await EnsureCatalogAsync();
        return _services;
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync()
    {
        await EnsureCatalogAsync();
        return _employees;
    }

    public Task<IReadOnlyList<BusySlot>> GetBusySlotsAsync()
    {
        if (_busyLoad is null || _busyLoad.IsFaulted || _busyLoad.IsCanceled)
            _busyLoad = LoadBusySlotsAsync();

        return _busyLoad;
    }

    private Task EnsureCatalogAsync()
    {
        if (_catalogLoad is null || _catalogLoad.IsFaulted || _catalogLoad.IsCanceled)
            _catalogLoad = LoadCatalogAsync();

        return _catalogLoad;
    }

    private async Task LoadCatalogAsync()
    {
        var servicesTask = _http.GetFromJsonAsync<List<SalonService>>("data/services.json");
        var employeesTask = _http.GetFromJsonAsync<List<Employee>>("data/employees.json");
        await Task.WhenAll(servicesTask, employeesTask);

        _services = await servicesTask ?? [];
        _employees = await employeesTask ?? [];
    }

    private async Task<IReadOnlyList<BusySlot>> LoadBusySlotsAsync()
        => await _http.GetFromJsonAsync<List<BusySlot>>("data/busy-slots.json")
           ?? throw new System.Text.Json.JsonException("Dolu saatler okunamadı.");
}
