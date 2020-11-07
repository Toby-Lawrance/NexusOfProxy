using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NexusofProxyLib
{
    public class MTGJson
    {
        private Dictionary<string, Card> cardDatabase;

        public MTGJson()
        {
            LoadCache();
        }

        public Card GetCard(string name)
        {
            return cardDatabase.ContainsKey(name) ? cardDatabase[name] : null;
        }

        public string DownloadCache()
        {
            string url = @"https://mtgjson.com/api/v5/AtomicCards.json";
            try
            {
                using WebClient client = new WebClient();
                var cont = client.DownloadString(new Uri(url));
                return cont;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public void LoadCache(bool forceRedownload = false)
        {
            string metaUrl = @"https://mtgjson.com/api/v5/Meta.json";
            
            bool Downloaded = false;
            var contents = "{}";
            if (File.Exists("AtomicCards.json") && !forceRedownload)
            {
                contents = File.ReadAllText("AtomicCards.json");
            }
            else
            {
                Console.WriteLine("Downloading file");
                contents = DownloadCache();
                Downloaded = true;
            }


            var data = JObject.Parse(contents);
            var version = data["meta"]["version"].Value<string>();

            try
            {
                using (WebClient client = new WebClient())
                {
                    string metaData = client.DownloadString(new Uri(metaUrl));
                    string metaVersion = JObject.Parse(metaData)["data"]["version"].Value<string>();
                    if (!Downloaded && (version != metaVersion))
                    {
                        Console.WriteLine("Redownloading due to mismatched Versions");
                        contents = DownloadCache();
                        data = JObject.Parse(contents);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            

            var dataContent = (JObject) data["data"];
            cardDatabase = dataContent.Properties()
                .Select(p => p.Name)
                .Select(n => (n, ((JArray) dataContent[n]).First))
                .Select(tup => new Card(tup.n, tup.First["identifiers"]["scryfallOracleId"].Value<string>(),
                    tup.First["printings"].ToObject<IEnumerable<string>>(), tup.First["layout"].Value<string>()))
                .ToDictionary(k => k.name);

            GC.Collect();
        }
    }
}