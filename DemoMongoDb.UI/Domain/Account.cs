using MongoDB.Bson;
using System;

namespace DemoMongoDb.UI.Domain
{
    public class Account
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string Status { get; set; }
        public ObjectId LinkedImageId { get; set; }

    }
}