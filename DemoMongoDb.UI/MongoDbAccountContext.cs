using DemoMongoDb.UI.Domain;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Configuration;
using System.Threading.Tasks;

namespace DemoMongoDb.UI
{
    public class MongoDbAccountContext
    {
        private ILog _logger = LogManager.GetLogger(typeof(MongoDbAccountContext));
        public IMongoDatabase AccountDatabase;
        public GridFSBucket ImagesBucket { get; set; }
        private const string MongoDbConnectionStringAppSettingKey = "MongoDbConnectionString";
        private const string MongoDbAccountDatabaseNameAppSettingKey = "MongoDbAccountDatabaseName";
        private const string AccountCollectionName = "Account";

        public MongoDbAccountContext()
        {
            // The context is thread safe and can be injected with IOC, can be static.
            var connectionString = ConfigurationManager.AppSettings[MongoDbConnectionStringAppSettingKey];
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            var client = new MongoClient(settings);
            _logger.Debug(string.Format("Mongo Client initialized with connection string {0}.", connectionString));
            var accountDbName = ConfigurationManager.AppSettings[MongoDbAccountDatabaseNameAppSettingKey];
            AccountDatabase = client.GetDatabase(accountDbName);
            _logger.Debug(string.Format("Connection to database '{0}' initialized.", accountDbName));
            ImagesBucket = new GridFSBucket(AccountDatabase);
        }

        public IMongoCollection<Account> Accounts => AccountDatabase.GetCollection<Account>(AccountCollectionName);

        public async Task<string> GetBuildInfoAsync()
        {
            var buildInfoCommand = new BsonDocument("buildinfo", 1);
            var buildInfo = await AccountDatabase.RunCommandAsync<BsonDocument>(buildInfoCommand);

            var result = buildInfo.ToJson();
            _logger.Debug(string.Format("Build info retrieved: '{0}'", result));

            return result;
        }
    }
}