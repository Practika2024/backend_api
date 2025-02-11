namespace Application.Authentications;

public class ServiceResponse
{
    public string Message { get; set; }
    public object? Payload { get; set; }

    public static ServiceResponse GetResponse(string message, object? payload)
    {
        return new ServiceResponse
        {
            Message = message,
            Payload = payload,
        };
    }
}