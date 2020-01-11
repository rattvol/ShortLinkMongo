using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShortLink.Models
{
    public class LinkMade
    {
        IGridFSBucket gridFS;   // файловое хранилище
        IMongoCollection<LinkTable> Links; // коллекция в базе данных

        public LinkMade()
        {
            // строка подключения
            string connectionString = "mongodb://localhost:27017/shortlink";
            var connection = new MongoUrlBuilder(connectionString);
            // получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            // получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase(connection.DatabaseName);
            // получаем доступ к файловому хранилищу
            gridFS = new GridFSBucket(database);
            // обращаемся к коллекциям
            Links = database.GetCollection<LinkTable>("LinkTable");

        }
        public List<LinkTable> GetAll()//выборка для общего списка
        {
            // строитель фильтров
            var filter = new BsonDocument(); // фильтр для выборки всех документов

            return Links.Find(filter).ToList();
        }

        public Task<List<LinkTable>> GetShortLink(string link)//получение короткой сслыки по длинной
        {
            var filter = new BsonDocument("Longlink", link); 
            return Links.Find(filter).ToListAsync();
        }

        public List<LinkTable> GetById(string id)//поиск по ид
        {
            var filter = new BsonDocument("_id", ObjectId.Parse(id)); 

            return Links.Find(filter).ToList();
        }
        public bool GetByShortLink(string link)//поиск по короткому имени
        {
            var filter = new BsonDocument("Shortlink", link); 
            var lines = Links.Find(filter).ToList();
            bool result = lines.Count() > 0;
            return result;
        }

        //// добавление документа
        public async Task Create(LinkTable lt)
        {
            await Links.InsertOneAsync(lt);
        }
        //// обновление документа
        public async Task Update(LinkTable lt)
        {
            await Links.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(lt.Id)), lt);
        }
        //// удаление документа
        public async Task Remove(string id)
        {
            await Links.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
       
    }
}
