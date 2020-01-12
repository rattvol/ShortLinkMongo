using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ShortLink.Models
{
   
    public class LinkTable
    {
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Longlink { get; set; }
        public string Shortlink { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }

    }
}
