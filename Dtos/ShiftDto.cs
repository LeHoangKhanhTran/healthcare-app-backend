public record ShiftDto(Guid ShiftId, DayOfWeek Weekday, string StartTime, string FinishTime, int Slots);
public record CreateShiftDto(DayOfWeek Weekday, string StartTime, string FinishTime, int Slots);
public record UpdateShiftDto(Guid ShiftId, DayOfWeek Weekday, int StartHour, int StartMinute, int FinishHour, int FinishMinute, int Slots);