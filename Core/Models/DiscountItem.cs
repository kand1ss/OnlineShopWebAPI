namespace Core;

public class DiscountItem
{
    public Guid ProductId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal FinalPrice { get; set; }
}