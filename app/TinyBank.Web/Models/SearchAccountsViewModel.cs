using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TinyBank.Core.Model;
using TinyBank.Core.Services.Options;

namespace TinyBank.Web.Models
{
    public class SearchAccountsViewModel
    {
        public List<Account> Accounts { get; set; }
        public SearchAccountOptions SearchOptions { get; set; }

        public SearchAccountsViewModel()
        {
            Accounts = new List<Account>();
            SearchOptions = new SearchAccountOptions();
        }
    }
}
