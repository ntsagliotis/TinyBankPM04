using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TinyBank.Core.Model;

namespace TinyBank.Core.Services
{
    public interface IAccountService
    {
        public ApiResult<Account> Create(Guid customerId,
            Options.CreateAccountOptions options);
        public ApiResult<Account> GetById(string accountId);
        public IQueryable<Account> Search(
            Options.SearchAccountOptions options);
        public ApiResult<Account> Update(string accountId,
            Options.UpdateAccountOptions options);
    }
}
