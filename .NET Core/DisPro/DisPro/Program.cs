using System;
using System.Collections.Generic;
using ServiceStack.Redis;

namespace DisPro
{
    class Program
    {
        static RedisClient _client = new RedisClient("127.0.0.1",6379);
        static void Main(string[] args)
        {
            Console.WriteLine("开始执行Redis测试");
            string res = Console.ReadLine();
            if (res == "F")
            {
                _client.Set<string>("Name", "Acctrue");
                _client.Set<User>("User", new User { Name = "AcctrueO", Pwd = "O" });
                _client.Set<List<User>>("List", new List<User>() { new User { Name = "AcctrueL1", Pwd = "O" }, new User { Name = "AcctrueL2", Pwd = "O" } });

                Console.WriteLine(_client.Get<string>("Name"));
                Console.WriteLine(_client.Get<User>("User").Name);
                Console.WriteLine(_client.Get<List<User>>("List")[0].Name);
            }

            if (res == "H")
            {

               // _client.SetEntryInHash("User", "Name", "Acctrue");
                var _1stKey = _client.GetHashKeys("User");
                Console.WriteLine("输出KEY!");
                _1stKey.ForEach(k=>Console.WriteLine(k));
                
                var _1stValue = _client.GetHashValues("User");
                Console.WriteLine("输出VALUE!");
                _1stValue.ForEach(k => Console.WriteLine(k));
            }

            if (res == "L")
            {
                _client.EnqueueItemOnList("Name","Acctrue");
                _client.EnqueueItemOnList("Name","Bcctrue");
                long _Length = _client.GetListCount("Name");

                Console.WriteLine("出队！");
                Console.WriteLine(_client.DequeueItemFromList("Name"));
                Console.WriteLine(_client.DequeueItemFromList("Name"));

                _client.PushItemToList("Name", "Acctrue");
                Console.WriteLine("出栈！");
                _client.PopItemFromList("Name");
            }

            if (res == "S")
            {
                _client.AddItemToSet("Name","Acctrue");
                HashSet<string>hashSet = _client.GetAllItemsFromSet("Name");

                _client.AddItemToSet("BName","Bcctrue");
                HashSet<string> UnionHashSet = _client.GetUnionFromSets("Name", "BName");
                HashSet<string> IntersectHashSet = _client.GetIntersectFromSets("Name", "BName");
                HashSet<string> DifferencesHashSet = _client.GetDifferencesFromSet("Name", "BName");

                //ZSet有序列表
                _client.AddItemToSortedSet("Name", "Acctrue",1);
                _client.AddItemToSortedSet("Name", "Acctrue",2);
                _client.AddItemToSortedSet("Name", "Acctrue",3);

            }

            Console.Read();
        }

        public class User
        {
            public string Name { get; set; }
            public string Pwd { get; set; }
        }
    }
}
