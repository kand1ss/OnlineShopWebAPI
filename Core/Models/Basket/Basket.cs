namespace Core;

public class Basket
{
    public Guid Id { get; set; }
    public List<BasketItem> Items { get; set; } = [];
    public decimal TotalPrice { get; set; }
}