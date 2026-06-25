using System.Data;
using System.Text.Json;
using Dapper;
using PaymentLink.Domain.PaymentLinks.Enums;

namespace PaymentLink.Infrastructure.Data.TypeHandlers;

public class PaymentLinkMethodsHandler : SqlMapper.TypeHandler<IEnumerable<PaymentLinkMethods>>
{
    public override void SetValue(IDbDataParameter parameter, IEnumerable<PaymentLinkMethods>? value)
    {
        parameter.Value = value != null
            ? JsonSerializer.Serialize(value.Select(m => m.ToString()))
            : DBNull.Value;
    }

    public override IEnumerable<PaymentLinkMethods> Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            return Enumerable.Empty<PaymentLinkMethods>();

        var methods = JsonSerializer.Deserialize<IEnumerable<string>>(value.ToString()!);
        return methods?.Select(m => Enum.Parse<PaymentLinkMethods>(m))
            ?? Enumerable.Empty<PaymentLinkMethods>();
    }
}