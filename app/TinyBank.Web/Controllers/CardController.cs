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
    [Route("card")]
    public class CardController : Controller
    {
        //private readonly ICustomerService _customers;
        private readonly ICardService _cards;
        private readonly ILogger<HomeController> _logger;
        private readonly TinyBankDbContext _dbContext;

        // Path: '/card'
        public CardController(
            TinyBankDbContext dbContext,
            ILogger<HomeController> logger,
            //ICustomerService customers,
            ICardService cards)
        {
            _logger = logger;
            //_customers = customers;
            _cards = cards;
            _dbContext = dbContext;
        }

        //public IActionResult UpdateDetails(string accountId,
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("checkout")]
        public IActionResult Card(
            [FromBody] CarPaymentOptions options)
        {
            //1111222233334444
            var result = _cards.MakePayment(options);

            if (!result.IsSuccessful()) {
                return result.ToActionResult();
            }

            return Ok();
        }
    }
}
