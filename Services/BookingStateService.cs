namespace KuaforRandevu.Services;

public sealed class BookingStateService
{
    private readonly HashSet<string> _selectedServiceIds = [];

    public IReadOnlyCollection<string> SelectedServiceIds => _selectedServiceIds;
    public string? EmployeeId { get; private set; }
    public DateOnly? Date { get; private set; }
    public TimeOnly? StartTime { get; private set; }
    public string? LastCreatedCode { get; private set; }

    public event Action? Changed;

    public void MarkCreated(string code) => LastCreatedCode = code;

    public string? TakeCreatedCode()
    {
        var code = LastCreatedCode;
        LastCreatedCode = null;
        return code;
    }

    public void ToggleService(string serviceId)
    {
        if (!_selectedServiceIds.Remove(serviceId))
            _selectedServiceIds.Add(serviceId);

        EmployeeId = null;
        StartTime = null;
        Changed?.Invoke();
    }

    public void SetEmployee(string employeeId)
    {
        if (EmployeeId == employeeId)
            return;

        EmployeeId = employeeId;
        StartTime = null;
        Changed?.Invoke();
    }

    public void SetDate(DateOnly date)
    {
        if (Date == date)
            return;

        Date = date;
        StartTime = null;
        Changed?.Invoke();
    }

    public void SetStartTime(TimeOnly? startTime)
    {
        if (StartTime == startTime)
            return;

        StartTime = startTime;
        Changed?.Invoke();
    }

    public void ClearEmployeeIfMissing(IEnumerable<string> eligibleIds)
    {
        if (EmployeeId is null || eligibleIds.Contains(EmployeeId))
            return;

        EmployeeId = null;
        StartTime = null;
        Changed?.Invoke();
    }
}
