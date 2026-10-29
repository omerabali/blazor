namespace KuaforRandevu.Models;

public enum SlotState
{
    Available,
    Busy,
    Past
}

public sealed record TimeSlotOption(TimeOnly Start, DateTime StartAt, DateTime EndAt, SlotState State)
{
    public bool IsSelectable => State == SlotState.Available;

    public string StatusText => State switch
    {
        SlotState.Busy => "Dolu",
        SlotState.Past => "Geçti",
        _ => "Uygun"
    };
}
