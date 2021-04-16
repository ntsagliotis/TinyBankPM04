using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using TinyBank.Core.Constants;
using TinyBank.Core.Implementation.Data;
using TinyBank.Core.Model;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;

namespace TinyBank.Core.Implementation.Services
{
    public class CardService : ICardService
    {
        private readonly ICustomerService _customers;
        private readonly Data.TinyBankDbContext _dbContext;

        public CardService(
            ICustomerService customers,
            Data.TinyBankDbContext dbContext)
        {
            //_cards = cards;
            _customers = customers;
            _dbContext = dbContext;
        }

        public ApiResult<Card> MakePayment(CarPaymentOptions options)
        {
            if (options == null) {
                return ApiResult<Card>.CreateFailed(
                    Constants.ApiResultCode.BadRequest, $"Null {nameof(options)}");
            }

            if (string.IsNullOrWhiteSpace(options.CardNumber)) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Null or empty {nameof(options.CardNumber)}"
                };
            }

            if (options.ExpirationYear == 0) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Null or empty {nameof(options.ExpirationYear)}"
                };
            }

            if (options.ExpirationMonth == 0) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Null or empty {nameof(options.ExpirationMonth)}"
                };
            }

            var cardExists = Exists(options.CardNumber);

            if (!cardExists) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Card {nameof(options.CardNumber)} not found"
                };
            }

            var cardResult = GetByCardNumber(options.CardNumber);
            var card = cardResult.Data;

            if (!(card.Expiration.Month == options.ExpirationMonth)) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Card {nameof(options.CardNumber)} has expired"
                };
            }

            if (!(card.Expiration.Year == options.ExpirationYear)) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Card {nameof(options.CardNumber)} has expired"
                };
            }

            if (!(card.Active == true)) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Card {nameof(options.CardNumber)} is inactive"
                };
            }
            if (card.Accounts[0].Balance < options.Amount) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Not enough balance found for card {nameof(options.CardNumber)}"
                };
            }


            card.Accounts[0].Balance = card.Accounts[0].Balance - options.Amount;

            try {
                _dbContext.SaveChanges();
            }
            catch (Exception) {
                return ApiResult<Card>.CreateFailed(
                    Constants.ApiResultCode.InternalServerError, "Could not make payment");
            }

            return ApiResult<Card>.CreateSuccessful(card);
        }

        private bool Exists(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber)) {
                throw new ArgumentNullException(cardNumber);
            }

            return Search(
                new SearchCardOptions() {
                    CardNumber = cardNumber
                }).Any();
        }

        public IQueryable<Card> Search(SearchCardOptions options)
        {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            var q = _dbContext.Set<Card>()
                .AsQueryable();

            if (options.CardId != null) {
                q = q.Where(c => c.CardId == options.CardId);
            }

            if (!string.IsNullOrWhiteSpace(options.CardNumber)) {
                q = q.Where(c => c.CardNumber == options.CardNumber);
            }

            q = q.Take(options.MaxResults ?? 10);

            return q;
        }

        public ApiResult<Card> GetByCardNumber(string cardNumber)
        {
            var card = Search(
                new SearchCardOptions() {
                    CardNumber = cardNumber
                })
                .Include(c => c.Accounts)
                .SingleOrDefault();

            if (card == null) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.NotFound,
                    ErrorText = $"Card {cardNumber} was not found"
                };
            }

            return new ApiResult<Card>() {
                Data = card
            };
        }
    }
}
