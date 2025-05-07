namespace Core.Payment;

public class PaymentData
{
    public int Ccv { get; set; }
    public string CardNumber { get; set; } = "";
    public string CardHolderName { get; set; } = "";
    public string ExpirationDate { get; set; } = "";
}