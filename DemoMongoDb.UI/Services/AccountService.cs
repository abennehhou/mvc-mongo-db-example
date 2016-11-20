using DemoMongoDb.UI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver.Linq;
using DemoMongoDb.UI.Domain;
using System.IO;
using MongoDB.Driver.GridFS;
using log4net;

namespace DemoMongoDb.UI.Services
{
    public class AccountService : IAccountService
    {
        private ILog _logger = LogManager.GetLogger(typeof(AccountService));

        public async Task<AccountModel> GetAccountById(string id)
        {
            var objectId = ObjectId.Parse(id);
            var dbAccount = await MvcApplication.MongoDbAccountContext.Accounts
                .AsQueryable()
                .Where(x => x._id == objectId)
                .FirstOrDefaultAsync();

            var account = new AccountModel
            {
                Id = dbAccount._id.ToString(),
                CreationDate = dbAccount.CreationDate,
                Name = dbAccount.Name,
                Description = dbAccount.Description,
                Status = dbAccount.Status,
                ModificationDate = dbAccount.ModificationDate,
                LinkedImageId = dbAccount.LinkedImageId.ToString()
            };

            return account;
        }

        public async Task<bool> UpdateAccount(string id, UpdateAccountModel account)
        {
            _logger.Debug($"Updating account {id}");
            var objectId = ObjectId.Parse(id);
            if (account == null)
                throw new Exception($"Account not provided for id={id}");
            if (id != account.Id)
                throw new Exception($"Id {id} is different from account's id {account.Id}");

            var modificationUpdate = Builders<Account>.Update
                .Set(x => x.Name, account.Name)
                .Set(x => x.Description, account.Description)
                .Set(x => x.Status, account.Status)
                .Set(x => x.ModificationDate, DateTime.UtcNow);

            var result = await MvcApplication.MongoDbAccountContext.Accounts.UpdateOneAsync(x => x._id == objectId, modificationUpdate);
            _logger.Debug($" account {id} updated. isAcknownledged?: {result.IsAcknowledged}.");

            return result.IsAcknowledged;
        }

        public async Task<string> CreateAccount(CreateAccountModel account)
        {
            _logger.Debug("Updating account");
            var dbAccount = new Account
            {
                CreationDate = DateTime.UtcNow,
                Name = account.Name,
                Description = account.Description,
                Status = "Created"
            };

            await MvcApplication.MongoDbAccountContext.Accounts.InsertOneAsync(dbAccount);
            var id = dbAccount._id.ToString();
            _logger.Debug($"account {id} created.");

            return id;
        }

        public async Task<List<AccountModel>> GetAllAccounts()
        {
            _logger.Debug("Retrieving all accounts");
            var databaseAccounts = await MvcApplication.MongoDbAccountContext.Accounts.AsQueryable().ToListAsync();
            var accounts = databaseAccounts
                .Select(dbAccount => new AccountModel
                {
                    Id = dbAccount._id.ToString(),
                    CreationDate = dbAccount.CreationDate,
                    Name = dbAccount.Name,
                    Description = dbAccount.Description,
                    Status = dbAccount.Status,
                    ModificationDate = dbAccount.ModificationDate,
                    LinkedImageId = dbAccount.LinkedImageId.ToString()
                })
                .ToList();

            _logger.Debug($"{accounts.Count} retrieved.");

            return accounts;
        }

        public async Task<bool> DeleteAccount(string id)
        {
            _logger.Debug($"Deleting account {id}");
            var objectId = new ObjectId(id);
            var result = await MvcApplication.MongoDbAccountContext.Accounts.DeleteOneAsync(x => x._id == objectId);
            _logger.Debug($" account {id} deleted. isAcknownledged?: {result.IsAcknowledged}.");
            return result.IsAcknowledged;
        }

        public async Task<string> SaveImageForAccount(string accountId, string imageFileName, Stream fileContent)
        {
            _logger.Debug($"Saving image {imageFileName} for customer {accountId}.");
            if (accountId == null)
                throw new Exception("SaveImageForAccount: Account id not provided.");

            ObjectId imageId;
            using (fileContent)
            {
                var options = new GridFSUploadOptions
                {
                    Metadata = new BsonDocument("fileName", imageFileName)
                };
                imageId = await MvcApplication.MongoDbAccountContext.ImagesBucket.UploadFromStreamAsync(imageFileName, fileContent, options);
            }
            var objectId = ObjectId.Parse(accountId);

            var modificationUpdate = Builders<Account>.Update
                .Set(x => x.LinkedImageId, imageId)
                .Set(x => x.ModificationDate, DateTime.UtcNow);

            var result = await MvcApplication.MongoDbAccountContext.Accounts.UpdateOneAsync(x => x._id == objectId, modificationUpdate);

            return imageId.ToString();
        }

        public async Task<byte[]> GetImageById(string imageId)
        {
            _logger.Debug($"Getting image {imageId}.");
            var imageObjectId = new ObjectId(imageId);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", imageObjectId);
            using (var cursor = await MvcApplication.MongoDbAccountContext.ImagesBucket.FindAsync(filter))
            {
                var fileInfo = (await cursor.ToListAsync()).FirstOrDefault();
                // fileInfo either has the matching file information or is null
                if (fileInfo == null)
                    return null;
                var imageBytes = await MvcApplication.MongoDbAccountContext.ImagesBucket.DownloadAsBytesAsync(imageObjectId);
                return imageBytes;
            }
        }
    }
}