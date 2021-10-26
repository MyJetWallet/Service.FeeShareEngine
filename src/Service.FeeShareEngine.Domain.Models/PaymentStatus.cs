namespace Service.FeeShareEngine.Domain.Models
{
    public enum PaymentStatus
    {
        New,
        Reserved,
        Paid,
        FailedToReserve,
        FailedToPay,
    }
}