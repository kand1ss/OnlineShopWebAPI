namespace Core;

public class Order
{
    public Guid Id { get; set; }
    public Guid BasketId { get; set; }
    public Guid DelivererId { get; set; }
    
    public OrderStatus Status { get; set; }
    public string DeliveryAddress { get; set; } = "";
    
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    
    public decimal BasketPrice { get; set; }
    public decimal DeliveryPrice { get; set; }
    public decimal TotalPrice => BasketPrice + DeliveryPrice;
}