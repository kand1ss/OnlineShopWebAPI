namespace Core.Payment;

public interface IPaymentMethod
{
    PaymentData GetPaymentData();
}