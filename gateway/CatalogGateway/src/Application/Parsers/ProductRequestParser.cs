namespace CatalogGateway.Application.Parsers;

public static class ProductRequestParser
{
    public static bool TryParseDecimal(string? input, out decimal result, out string error)
    {
        if (decimal.TryParse(input, out result))
        {
            error = "";
            return true;
        }

        error = "Price must be a valid decimal number";
        return false;
    }

    public static bool TryParseGuid(string? input, out Guid result, out string error)
    {
        if (Guid.TryParse(input, out result))
        {
            error = "";
            return true;
        }

        error = "ID must be a valid GUID";
        return false;
    }
}