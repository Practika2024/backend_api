namespace Api.Dtos.ReminderType;

public record ReminderTypeDto
{
    public int? Id { get; init; }
    public string Name { get; init; } = null!;
}