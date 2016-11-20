using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DemoMongoDb.UI
{
    public class MvcApplication : HttpApplication
    {
        private ILog _logger = LogManager.GetLogger(typeof(MvcApplication));
        public static MongoDbAccountContext MongoDbAccountContext;

        protected void Application_Start()
        {
            _logger.Info("Initializing MVC Application");
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            InitializeMongoDb();
        }

        private void InitializeMongoDb()
        {
            _logger.Info("Initializing Mongo DB context.");
            MongoDbAccountContext = new MongoDbAccountContext();
        }
    }
}
