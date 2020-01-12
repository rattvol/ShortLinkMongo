using Microsoft.Extensions.Configuration;
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
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();
            // строка подключения
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
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
        public async Task<List<LinkTable>> GetAll()//выборка для общего списка
        {
            // строитель фильтров
            var filter = new BsonDocument(); // фильтр для выборки всех документов

            return await Links.Find(filter).ToListAsync();
        }
        public async Task<List<LinkTable>> GetSearch(string line)//выборка для общего списка
        {
            // строитель фильтров
            
                var builder = new FilterDefinitionBuilder<LinkTable>();
                var filters = builder.Or(builder.Regex("Longlink", line), builder.Regex("Shortlink", line)); // фильтр для выборки всех документов  
                return await Links.Find(filters).ToListAsync();

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
        public async Task<List<LinkTable>> GetLongLink(string link)//получение длинной сслыки по короткой
        {
            var filter = new BsonDocument("Shortlink", link);
            var result = await Links.FindAsync(filter);
            return result.ToList();
        }

        // добавление записи
        public async Task Create(LinkTable lt)
        {
            await Links.InsertOneAsync(lt);
        }
        // обновление записи
        public async Task Update(LinkTable lt)
        {
            await Links.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(lt.Id)), lt);
        }
        // пометка как удаленной записи
        public async Task Remove(string id)
        {
            await Links.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
        public async void CountOn(LinkTable newdoc)
        {
            int newcount = newdoc.Count+1;
            var updateDef = Builders<LinkTable>.Update.Set("Count", newcount);
            await Links.UpdateOneAsync(new BsonDocument("Shortlink", newdoc.Shortlink), updateDef);
        }

    }
}
