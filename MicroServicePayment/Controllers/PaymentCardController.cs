using Microsoft.AspNetCore.Mvc;
using MicroServicePayment.DTO;
using MicroServicePayment.Interfaces;
using AutoMapper;

namespace MicroServicePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentCardController : ControllerBase
    {
        private readonly IPaymentCardRepository _paymentCardRepository;
        private readonly IMapper _mapper;

        public PaymentCardController(IPaymentCardRepository paymentCardRepository, IMapper mapper)
        {
            _paymentCardRepository = paymentCardRepository;
            _mapper = mapper;
        }

        [HttpGet("{UserIdentifier}/GetPaymentCardsForUser")]
        public IActionResult GetPaymentCardsForEntity(string entityIdentifier)
        {
            // Appel du repository pour obtenir les cartes de paiement associées à l'entité
            var paymentCards = _paymentCardRepository.GetPaymentCardsForEntity(entityIdentifier);

            // Vérifier s'il n'y a aucune carte de paiement trouvée pour l'entité donnée
            if (paymentCards == null || paymentCards.Count == 0)
            {
                // Vous pouvez personnaliser le message d'erreur selon vos besoins
                return NotFound(new ApiError { Message = "No payment cards found for the specified User." });
            }

            // Mapper les entités vers les DTOs
            var paymentCardDTOs = _mapper.Map<List<PaymentcardDTO>>(paymentCards);

            // Retourner les DTOs en tant que réponse de l'API
            return Ok(paymentCardDTOs);
        }

        [HttpPut("{cardNumber}/UpdatePaymentCard")]
        public IActionResult UpdatePaymentCard(string cardNumber, [FromBody] UpdatePaymentCardDTO updateDTO)
        {
            // Récupérer la carte de paiement à partir du numéro de carte
            var paymentCard = _paymentCardRepository.GetPaymentCardByCardNumber(cardNumber);

            if (paymentCard == null)
            {
                return NotFound(new ApiError { Message = "Bank card not found. Please specify valid values for cardNumber." });
            }

            // Mettre à jour les propriétés de la carte de paiement
            paymentCard.Maxgab = updateDTO.NewGab;
            paymentCard.Maxtpe = updateDTO.NewTpe;

            // Enregistrer les modifications dans la base de données
            _paymentCardRepository.UpdatePaymentCard(paymentCard);

            return Ok(paymentCard);
        }

        [HttpGet("{cardNumber}/GetcartStatus")]
        public IActionResult GetCreditStatus(string cardNumber)
        {
            // Récupérer la carte de paiement à partir du numéro de carte
            var paymentCard = _paymentCardRepository.GetPaymentCardByCardNumber(cardNumber);

            if (paymentCard == null)
            {
                return NotFound(new ApiError { Message = "Bank card not found. Please specify valid values for cardNumber." });
            }

            // Vous devrez adapter le code ci-dessous en fonction de votre modèle de données
            // Supposons que la propriété du statut de crédit est "CreditStatus" de type string
            var creditStatus = paymentCard.Status;

            return Ok(creditStatus);
        }
    }
}
