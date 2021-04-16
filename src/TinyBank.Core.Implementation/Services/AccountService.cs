﻿using System;
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
    public class AccountService : IAccountService
    {
        private readonly ICustomerService _customers;
        private readonly Data.TinyBankDbContext _dbContext;

        public AccountService(
            ICustomerService customers,
            Data.TinyBankDbContext dbContext)
        {
            _customers = customers;
            _dbContext = dbContext;
        } 

        public ApiResult<Account> Create(Guid customerId,
            CreateAccountOptions options)
        {
            if (options == null) {
                return ApiResult<Account>.CreateFailed(
                    Constants.ApiResultCode.BadRequest, $"Null {nameof(options)}");
            }

            if (string.IsNullOrWhiteSpace(options.CurrencyCode) ||
              !options.CurrencyCode.Equals(
                  Constants.CurrencyCode.Euro, StringComparison.OrdinalIgnoreCase)) {
                return ApiResult<Account>.CreateFailed(
                    Constants.ApiResultCode.BadRequest,
                    $"Invalid or unsupported currency {options.CurrencyCode}");
            }

            var customerResult = _customers.GetById(customerId);

            if (!customerResult.IsSuccessful()) {
                return customerResult.ToResult<Account>();
            }

            var customer = customerResult.Data;

            var account = new Account() {
                AccountId = CreateAccountId(customer.CountryCode),
                Balance = 0M,
                CurrencyCode = options.CurrencyCode,
                Customer = customer,
                State = Constants.AccountState.Active,
                Description = options.Description
            };

            _dbContext.Add(account);

            try {
                _dbContext.SaveChanges();
            } catch (Exception) {
                return ApiResult<Account>.CreateFailed(
                    Constants.ApiResultCode.InternalServerError, "Could not save account");
            }

            return ApiResult<Account>.CreateSuccessful(account);
        }

        private string CreateAccountId(string countryCode)
        {
            var random = new Random();
            var accountId = $"{countryCode}{random.Next(1000, int.MaxValue).ToString().PadLeft(20, '0')}";

            return accountId;
        }

        public ApiResult<Account> GetById(string accountId)
        {
            var account = Search(
                new SearchAccountOptions() {
                    AccountId = accountId
                })
                .SingleOrDefault();

            if (account == null) {
                return new ApiResult<Account>() {
                    Code = Constants.ApiResultCode.NotFound,
                    ErrorText = $"Customer {accountId} was not found"
                };
            }

            return new ApiResult<Account>() {
                Data = account
            };
        }

        public IQueryable<Account> Search(SearchAccountOptions options)
        {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            // SELECT FROM CUSTOMER
            var q = _dbContext.Set<Account>()
                .AsQueryable();

            if (options.CustomerId != null) {
                q = q.Where(c => c.CustomerId == options.CustomerId);
            }

            // AND VatNumber = options.VatNumber
            if (!string.IsNullOrWhiteSpace(options.AccountId)) {
                q = q.Where(c => c.AccountId == options.AccountId);
            }

            if (options.TrackResults != null &&
              !options.TrackResults.Value) {
                q = q.AsNoTracking();
            }

            if (options.Skip != null) {
                q = q.Skip(options.Skip.Value);
            }

            q = q.Take(options.MaxResults ?? 500);

            return q;
        }

        public ApiResult<Account> Update(
            string accountId, UpdateAccountOptions options)
        {
            if (options == null) {
                return new TinyBank.Core.ApiResult<Account>() {
                    Code = ApiResultCode.NotFound,
                    ErrorText = $"Bad request"
                };
            }

            var result = GetById(accountId);

            if (!result.IsSuccessful()) {
                return result;
            }

            var account = result.Data;

            if (account != null) {
                account.State = options.State;

                _dbContext.SaveChanges();
            }
            else {
                return new TinyBank.Core.ApiResult<Account>() {
                    Code = ApiResultCode.NotFound,
                    ErrorText = $"Account not found !"
                };
            }

            return new ApiResult<Account>() {
                Data = account
            };
        }
    }
}
