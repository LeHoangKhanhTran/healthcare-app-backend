using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;
public class Shift 
{
    [BsonId]
    [BsonElement("ShiftId")]
    public Guid ShiftId;

    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public DayOfWeek Weekday { get; set; }
    public string StartTime { get; set; }
    public string FinishTime { get; set; }
    public int Slots { get; set; }

    public Shift(DayOfWeek weekday, string startTime, string finishTime, int slots)
    {
        this.ShiftId = Guid.NewGuid();
        this.Weekday = weekday;
        this.StartTime = startTime;
        this.FinishTime = finishTime;
        this.Slots = slots;
    }

    public static bool IsOverlap(Shift firstShift, Shift secondShift)
    {
        if (firstShift.Weekday != secondShift.Weekday) return false;
        TimeOnly firstStartTime = TimeOnly.Parse(firstShift.StartTime);
        TimeOnly firstFinishTime = TimeOnly.Parse(firstShift.FinishTime);
        TimeOnly secondStartTime = TimeOnly.Parse(secondShift.StartTime);
        TimeOnly secondFinishTime = TimeOnly.Parse(secondShift.FinishTime);
        bool startTimeOverlap = (firstStartTime > secondStartTime && firstStartTime < secondFinishTime) || (secondStartTime > firstStartTime && secondStartTime < firstFinishTime);
        bool finishTimeOverlap = (firstFinishTime > secondStartTime && firstFinishTime < secondFinishTime) || (secondFinishTime > firstStartTime && secondFinishTime < firstFinishTime);
        return startTimeOverlap || finishTimeOverlap;
    }

    public void DecreaseSlot()
    {
        this.Slots--;
    }

    public void IncreaseSlot()
    {
        this.Slots++;
    }
}