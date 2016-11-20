using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoMongoDb.UI.Models
{
    public class AccountModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string Status { get; set; }
        public string LinkedImageId { get; set; }
        public string ImageSourceContent { get; set; }
    }
}