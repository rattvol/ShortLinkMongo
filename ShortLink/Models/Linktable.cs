using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ShortLink.Models
{
    //public class LinkTable
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public int Id { get; set; }
    //    public string Longlink { get; set; }
    //    public string Shortlink { get; set; }
    //    public DateTime? Date { get; set; }
    //    public sbyte? Deleted { get; set; }
    //}
    //public class Log
    //{
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public int IdLink { get; set; }
    //    public int? Count { get; set; }
    //}
    public class LinkTable
    {
        
        //[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Longlink { get; set; }
        public string Shortlink { get; set; }
        public DateTime? Date { get; set; }
        public sbyte? Deleted { get; set; }
        public int? Count { get; set; }

    }
}
