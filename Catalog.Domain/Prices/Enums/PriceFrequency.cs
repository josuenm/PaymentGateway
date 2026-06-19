using System.Text.Json.Serialization;

namespace Catalog.Domain.Enums;

public enum PriceFrequency : byte
{
    OneTime = 1, 
    Recurring = 2
}