using MicroServicePayment.Models;

namespace MicroServicePayment.Interfaces
{
    public interface IPaymentCardRepository
    {
        List<Paymentcard> GetPaymentCardsForEntity(string entityIdentifier);

        Paymentcard? GetPaymentCardByCardNumber(string cardNumber);
        void UpdatePaymentCard(Paymentcard paymentCard);
        
    }
}

