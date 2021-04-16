using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using TinyBank.Core.Implementation.Data;
using TinyBank.Core.Model;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;
using TinyBank.Web.Extensions;
using TinyBank.Web.Models;

namespace TinyBank.Web.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        //private readonly ICustomerService _customers;
        private readonly IAccountService _accounts;
        private readonly ILogger<HomeController> _logger;
        private readonly TinyBankDbContext _dbContext;

        // Path: '/account'
        public AccountController(
            TinyBankDbContext dbContext,
            ILogger<HomeController> logger,
            //ICustomerService customers,
            IAccountService accounts)
        {
            _logger = logger;
            //_customers = customers;
            _accounts = accounts;
            _dbContext = dbContext;
        }


        [HttpPut("{accountId}")]
        public IActionResult UpdateDetails(string accountId,
            [FromBody] UpdateAccountOptions options)
        {
            var result = _accounts.Update(accountId, options);

            if (!result.IsSuccessful()) {
                return result.ToActionResult();
            }

            return Ok();
        }
    }
}
