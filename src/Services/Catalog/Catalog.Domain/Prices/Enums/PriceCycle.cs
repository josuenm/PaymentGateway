using System.Text.Json.Serialization;

namespace Catalog.Domain.Prices.Enums;

public enum PriceCycle : byte
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    SemiAnnually = 3,
    Yearly = 4
}