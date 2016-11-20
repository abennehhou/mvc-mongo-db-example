using DemoMongoDb.UI.Domain;
using DemoMongoDb.UI.Models;
using DemoMongoDb.UI.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DemoMongoDb.UI.Controllers
{
    public class HomeController : Controller
    {
        private IAccountService _accountService;

        public HomeController()
        {
            _accountService = new AccountService();
        }

        public async Task<ActionResult> Index()
        {
            var accounts = await _accountService.GetAllAccounts();

            return View(accounts);
        }
    }
}