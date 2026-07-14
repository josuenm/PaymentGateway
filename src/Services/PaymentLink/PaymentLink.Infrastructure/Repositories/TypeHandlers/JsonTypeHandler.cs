using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization; // Importante para o JsonStringEnumConverter
using Dapper;

namespace PaymentLink.Infrastructure.Repositories.TypeHandlers;

public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = 
        { 
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper) 
        }
    };

    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        parameter.Value = value is null ? DBNull.Value : JsonSerializer.Serialize(value, _options);
    }

    public override T? Parse(object value)
    {
        if (value is null || value is DBNull) return default;
        
        return JsonSerializer.Deserialize<T>(value.ToString()!, _options);
    }
}