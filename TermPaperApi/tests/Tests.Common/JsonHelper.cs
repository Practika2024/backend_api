using System.Net.Http.Json;
using System.Text.Json;
using Application.Services;

namespace Tests.Common;

public class JsonHelper
{
    private readonly JsonSerializerOptions _defaultOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<T> GetPayloadAsync<T>(HttpResponseMessage response)
    {
        var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse>(_defaultOptions)
                              ?? throw new InvalidOperationException("Response content is null or invalid.");

        var payloadJson = JsonSerializer.Serialize(serviceResponse.Payload, _defaultOptions);

        var payload = JsonSerializer.Deserialize<T>(payloadJson, _defaultOptions)
                      ?? throw new InvalidOperationException("Failed to deserialize payload.");

        return payload;
    }
}