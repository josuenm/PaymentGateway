using System.Text.Json.Serialization;

namespace Catalog.Domain.Enums;

public enum PriceFrequency : byte
{
    [JsonPropertyName("one_time")]
    OneTime = 1, 
    [JsonPropertyName("recurring")]
    Recurring = 2
}