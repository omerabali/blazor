using KuaforRandevu.Models;

namespace KuaforRandevu.Services;

public sealed class AvailabilityService
{
    public static readonly TimeOnly Opens = new(9, 0);
    public static readonly TimeOnly Closes = new(19, 0);
    public const int StepMinutes = 30;

    public IReadOnlyList<TimeSlotOption> GetSlots(
        DateOnly date,
        int durationMinutes,
        string employeeId,
        IEnumerable<BusySlot> busySlots,
        IEnumerable<Appointment> appointments,
        DateTime? clock = null)
    {
        var result = new List<TimeSlotOption>();
        if (durationMinutes <= 0 || string.IsNullOrWhiteSpace(employeeId))
            return result;

        var now = clock ?? DateTime.Now;
        var blocks = Blocked(employeeId, busySlots, appointments).ToList();

        for (var cursor = Opens; cursor < Closes; cursor = cursor.AddMinutes(StepMinutes))
        {
            var endClock = cursor.AddMinutes(durationMinutes);
            if (endClock < cursor || endClock > Closes)
                break;

            var start = date.ToDateTime(cursor);
            var end = start.AddMinutes(durationMinutes);
            var state = SlotState.Available;
            if (start <= now)
                state = SlotState.Past;
            else if (blocks.Any(block => start < block.End && end > block.Start))
                state = SlotState.Busy;

            result.Add(new TimeSlotOption(cursor, start, end, state));
        }

        return result;
    }

    public bool IsSelectable(
        DateOnly date,
        TimeOnly startTime,
        int durationMinutes,
        string employeeId,
        IEnumerable<BusySlot> busySlots,
        IEnumerable<Appointment> appointments)
    {
        return GetSlots(date, durationMinutes, employeeId, busySlots, appointments)
            .Any(slot => slot.Start == startTime && slot.IsSelectable);
    }

    public static IEnumerable<Employee> Eligible(IEnumerable<Employee> employees, IReadOnlyCollection<string> serviceIds)
    {
        if (serviceIds.Count == 0)
            return [];

        return employees.Where(employee => serviceIds.All(id => employee.ServiceIds.Contains(id)));
    }

    private static IEnumerable<(DateTime Start, DateTime End)> Blocked(
        string employeeId,
        IEnumerable<BusySlot> busySlots,
        IEnumerable<Appointment> appointments)
    {
        foreach (var slot in busySlots)
        {
            if (slot.EmployeeId == employeeId)
                yield return (slot.StartAt, slot.EndAt);
        }

        foreach (var appointment in appointments)
        {
            if (!appointment.IsCancelled && appointment.EmployeeId == employeeId)
                yield return (appointment.StartAt, appointment.EndAt);
        }
    }
}
