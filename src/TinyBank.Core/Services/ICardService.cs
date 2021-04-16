using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TinyBank.Core.Model;

namespace TinyBank.Core.Services
{
    public interface ICardService
    {
        public ApiResult<Card> GetByCardNumber(string cardNumber);
        public ApiResult<Card> MakePayment(Options.CarPaymentOptions options);
        

        //public ApiResult<Card> Create(Guid customerId,
        //    Options.CreateCardOptions options);
        //public ApiResult<Card> GetById(string CardId);
        //public ApiResult<Card> GetByCardNumber(string CardNumber);
        //public IQueryable<Card> Search(
        //    Options.SearchCardOptions options);
        //public ApiResult<Card> Update(string CardId,
        //    Options.UpdateCardOptions options);
    }
}
