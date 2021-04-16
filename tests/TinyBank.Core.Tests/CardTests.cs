using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;

using TinyBank.Core.Implementation.Data;
using TinyBank.Core.Model;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;
using TinyBank.Core.Constants;

using Xunit;

namespace TinyBank.Core.Tests
{
    public class CardTests : IClassFixture<TinyBankFixture>
    {
        private readonly ICardService _cards;
        private readonly TinyBankDbContext _dbContext;

        public CardTests(TinyBankFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _cards = fixture.GetService<ICardService>();
        }

        [Fact]
        public void CardOld_Register_Success()
        {
            var customer = new Customer() {
                Firstname = "Dimitris",
                Lastname = "Pnevmatikos",
                VatNumber = "117008855",
                Email = "dpnevmatikos@codehub.gr",
                IsActive = true
            };

            var account = new Account() {
                Balance = 1000M,
                CurrencyCode = "EUR",
                State = Constants.AccountState.Active,
                AccountId = "GR123456789121"
            };

            customer.Accounts.Add(account);

            var card = new Card() {
                Active = true,
                CardNumber = "4111111111111111",
                CardType = Constants.CardType.Debit
            };

            account.Cards.Add(card);

            _dbContext.Add(customer);
            _dbContext.SaveChanges();

            var customerFromDb = _dbContext.Set<Customer>()
                .Where(c => c.VatNumber == "117008855")
                .Include(c => c.Accounts)
                .ThenInclude(a => a.Cards)
                .SingleOrDefault();

            var customerCard = customerFromDb.Accounts
                .SelectMany(a => a.Cards)
                .Where(c => c.CardNumber == "4111111111111111")
                .SingleOrDefault();

            Assert.NotNull(customerCard);
            Assert.Equal(Constants.CardType.Debit, customerCard.CardType);
            Assert.True(customerCard.Active);
        }

        [Fact]
        public void Card_Register_Success()
        {
            var customer = new Customer() {
                Firstname = "MyName",
                Lastname = "MySurname",
                VatNumber = "123456788",
                Email = "mymail@mydomain.gr",
                IsActive = true
            };

            var account = new Account() {
                Balance = 1000M,
                CurrencyCode = "EUR",
                State = Constants.AccountState.Active,
                AccountId = "GR00000000000000001237"
            };

            customer.Accounts.Add(account);

            var card = new Card() {
                Active = true,
                CardNumber = "1111222233335555",
                CardType = Constants.CardType.Debit,
                Expiration = DateTimeOffset.Parse("2023-06-30")
            };

            account.Cards.Add(card);

            _dbContext.Add(customer);
            _dbContext.SaveChanges();

            var customerFromDb = _dbContext.Set<Customer>()
                .Where(c => c.VatNumber == "123456788")
                .Include(c => c.Accounts)
                .ThenInclude(a => a.Cards)
                .SingleOrDefault();

            var customerCard = customerFromDb.Accounts
                .SelectMany(a => a.Cards)
                .Where(c => c.CardNumber == "1111222233335555")
                .SingleOrDefault();

            Assert.NotNull(customerCard);
            Assert.Equal(Constants.CardType.Debit, customerCard.CardType);
            Assert.True(customerCard.Active);
        }

        [Fact]
        public void Card_Payment_Success()
        {
            var options = new CarPaymentOptions() {
                CardNumber = "1111222233335555",
                ExpirationMonth = 6,
                ExpirationYear = 2023,
                Amount = 10
            };

            var result = _cards.MakePayment(options);
            Assert.Equal(result.Code, ApiResultCode.Success);
        }

        [Fact]
        public void Card_Payment_Fail()
        {
            var options = new CarPaymentOptions() {
                CardNumber = "0000000000000000",
                ExpirationMonth = 6,
                ExpirationYear = 2021,
                Amount = 10
            };

            var result = _cards.MakePayment(options);
            Assert.Equal(result.Code, ApiResultCode.BadRequest);
        }
    }
}
