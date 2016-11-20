using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoMongoDb.UI.Models
{
    public class UpdateAccountModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}