using DemoMongoDb.UI.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DemoMongoDb.UI.Services;
using System.Web;
using System.IO;

namespace DemoMongoDb.UI.Controllers
{
    public class AccountsController : Controller
    {
        private IAccountService _accountService;

        public AccountsController()
        {
            _accountService = new AccountService();
        }

        public async Task<ActionResult> Details(string id)
        {
            var account = await _accountService.GetAccountById(id);

            var imageDetails = await _accountService.GetImageById(account.LinkedImageId);
            if (imageDetails != null && imageDetails.Length > 0)
            {
                var base64 = Convert.ToBase64String(imageDetails);
                var imgSrc = string.Format("data:image/gif;base64,{0}", base64);
                account.ImageSourceContent = imgSrc;
            }


            return View(account);
        }

        public ActionResult Create()
        {
            var accountModel = new CreateAccountModel();

            return View(accountModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountModel account)
        {
            if (ModelState.IsValid)
            {
                var id = await _accountService.CreateAccount(account);
                return RedirectToAction("Details", "Accounts", new { id });
            }

            return View(account);
        }

        public async Task<ActionResult> Edit(string id)
        {
            var account = await _accountService.GetAccountById(id);
            var editAccountModel = new UpdateAccountModel
            {
                Id = account.Id,
                Name = account.Name,
                Description = account.Description,
                Status = account.Status
            };
            return View(editAccountModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, UpdateAccountModel account)
        {
            if (account == null)
                throw new Exception($"Account not provided for id={id}");
            if (id != account.Id)
                throw new Exception($"Id {id} is different from account's id {account.Id}");

            if (ModelState.IsValid)
            {
                var result = await _accountService.UpdateAccount(id, account);
                return RedirectToAction("Details", "Accounts", new { id });
            }
            return View(account);
        }

        public async Task<ActionResult> Delete(string id)
        {
            var account = await _accountService.GetAccountById(id);
            return View(account);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var result = await _accountService.DeleteAccount(id);
            //TODO Delete linked images
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> ImportImage(HttpPostedFileBase file, string accountId)
        {
            if (file == null || file.ContentLength == 0)
                throw new Exception("File not specified");

            if (accountId == null)
                throw new Exception("AccountId not specified");

            var imageId = await _accountService.SaveImageForAccount(accountId, file.FileName, file.InputStream);

            return RedirectToAction("Details", "Accounts", new { id = accountId });
        }
    }
}