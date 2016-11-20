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

namespace DemoMongoDb.UI.Services
{
    public interface IAccountService
    {
        Task<AccountModel> GetAccountById(string id);

        Task<bool> UpdateAccount(string id, UpdateAccountModel account);

        Task<string> CreateAccount(CreateAccountModel account);

        Task<bool> DeleteAccount(string id);

        Task<List<AccountModel>> GetAllAccounts();

        Task<string> SaveImageForAccount(string accountId, string imageFileName, Stream fileContent);

        Task<byte[]> GetImageById(string imageId);
    }
}