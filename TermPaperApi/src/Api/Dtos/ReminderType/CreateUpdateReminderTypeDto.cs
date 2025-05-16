namespace Api.Dtos.ReminderType;

public record CreateUpdateReminderTypeDto
{
    public string Name { get; init; } = null!;
}