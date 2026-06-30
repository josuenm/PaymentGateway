using System.Text;
using System.Text.RegularExpressions;

namespace Shared.Mocks;

public static class PixMock
{
    public static string Generate(string paymentId, decimal amount = 0)
    {
        var encodedId = Convert.ToBase64String(Encoding.UTF8.GetBytes(paymentId))
            .Replace("=", "").Replace("+", "-").Replace("/", "_");
        
        var sb = new StringBuilder();
        sb.Append("000201");
        sb.Append("2636");
        sb.Append("0014BR.GOV.BCB.PIX");
        sb.Append($"01{encodedId.Length + 4:D2}01GW{encodedId}");
        sb.Append("52040000");
        sb.Append("5303986");
        
        if (amount > 0)
        {
            var amountStr = amount.ToString("F2").Replace(".", "").Replace(",", "");
            sb.Append($"54{amountStr.Length:D2}{amountStr}");
        }
        
        sb.Append("5802BR");
        sb.Append("5912Gateway Mock");
        sb.Append("6009Sao Paulo");
        sb.Append($"62{paymentId.Length + 3:D2}ID:{paymentId}");
        sb.Append("6304");
        
        var crc = 0xFFFF;
        var data = sb.ToString().Replace("6304", "");
        foreach (var c in data)
        {
            crc ^= c;
            for (var i = 0; i < 8; i++)
                crc = (crc & 1) != 0 ? (crc >> 1) ^ 0x8408 : crc >> 1;
        }
        sb.Append(crc.ToString("X4"));
        
        return sb.ToString();
    }

    public static string ExtractPaymentId(string pixCode)
    {
        var match = Regex.Match(pixCode, @"62(\d{2})ID:(.+?)(?=\d{2}\d{2}|6304|$)");
        if (match.Success)
            return match.Groups[2].Value;

        match = Regex.Match(pixCode, @"01GW([A-Za-z0-9\-_]+)");
        if (match.Success)
        {
            var encoded = match.Groups[1].Value.Replace("-", "+").Replace("_", "/");
            var padding = encoded.Length % 4;
            if (padding > 0) encoded += new string('=', 4 - padding);
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }

        throw new ArgumentException("Invalid PIX code");
    }
}