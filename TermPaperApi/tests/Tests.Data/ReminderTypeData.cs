using Domain.ProductTypes;
using Domain.ReminderTypes;

namespace Tests.Data;

public class ReminderTypeData
{
    public static ReminderType MainReminderType => new()
    {
        Name = "Main Test Reminder Type"
    };
}