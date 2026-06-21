namespace Billing.Domain.PriceReplicas.Enums;

public enum PriceReplicaCycle : byte
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    SemiAnnually = 3,
    Yearly = 4
}