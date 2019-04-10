using System;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using DisMongo.BsonDoc;
using System.Collections.Generic;

namespace DisMongo
{
    class Program
    {
        static void Main(string[] args)
        {

            MongoClient client = new MongoClient("mongodb://127.0.0.1:27017");
            var database = client.GetDatabase("demo");
            var collection = database.GetCollection<BsonBook>("users");

            using (var cursor = client.ListDatabases())
            {
                foreach (var document in cursor.ToEnumerable())
                {
                    Console.WriteLine(document.ToString());
                }
            }

            if (Console.ReadLine() == "Insert")
            {
                BsonBook book = new BsonBook
                {
                    BookName=".NET Core",
                    Price=100,
                    Category="Programming",
                    Author="Alex"
                };
                collection.InsertOne(book);
                collection.InsertOneAsync(book);

                Console.WriteLine("插入数据成功！");
            }


            if (Console.ReadLine() =="InsertMulti")
            {
                List<BsonBook> books = new List<BsonBook>();
                books.Add(new BsonBook {
                    BookName = ".NET Framework",
                    Price = 100,
                    Category = "Programming",
                    Author = "Blex"
                });
                books.Add(new BsonBook
                {
                    BookName = ".NET MVC",
                    Price = 100,
                    Category = "Programming",
                    Author = "Clex"
                });
                collection.InsertMany(books);
                collection.InsertManyAsync(books);

                Console.WriteLine("插入多条成功！");
            }

            //计数统计
            var count = collection.CountDocuments(new BsonDocument());
            Console.WriteLine("一共存储条数！"+count);

            //查询 - 无条件
            if (Console.ReadLine() =="Find")
            {
                var document = collection.Find(new BsonDocument()).FirstOrDefault();
                Console.WriteLine(document.ToString());
                var documentAsync = collection.Find(new BsonDocument()).FirstOrDefaultAsync();
                Console.WriteLine(documentAsync.ToString());

                var documents = collection.Find(new BsonDocument()).ToList();
                var documentsAsync = collection.Find(new BsonDocument()).ToListAsync();

                var cursor = collection.Find(new BsonDocument()).ToCursor();
                foreach (var doc in cursor.ToEnumerable())
                {
                    Console.WriteLine(doc);
                }
            }

            //查询 - 有条件
            if (Console.ReadLine() =="FindAsync")
            {
                var filter = Builders<BsonBook>.Filter.Eq("BookName", ".NET MVC");
                var document = collection.Find(filter).First();
                Console.WriteLine(document);
                var documents = collection.Find(filter).FirstAsync();
                Console.WriteLine(documents);
            }

            if (Console.ReadLine() == "Pro")
            {
                var projection = Builders<BsonBook>.Projection.Exclude("_id");
                var document = collection.Find(new BsonDocument()).Project(projection).First();
                Console.WriteLine(document.ToString());
            }

            if (Console.ReadLine() == "Update")
            {
                var filter = Builders<BsonBook>.Filter.Eq("BookName", ".NET MVC");
                var update = Builders<BsonBook>.Update.Set("BookName", ".NET MVC1.0");
                collection.UpdateOne(filter, update);
                Console.WriteLine("更新成功！");
            }

            if (Console.ReadLine() == "Delete")
            {
                var filter = Builders<BsonBook>.Filter.Eq("BookName", ".NET Framework");
                collection.DeleteOne(filter);
                Console.WriteLine("删除成功！");
            }

            Console.Read();
        }
    }
}
