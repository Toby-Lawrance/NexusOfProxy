using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks.Sources;

namespace NexusofProxyLib
{
    public class ScryfallEndPoint
    {
        private static string cardURL = @"https://api.scryfall.com/cards/named";

        private Stopwatch scryfallDelay;

        public ScryfallEndPoint()
        {
            scryfallDelay = Stopwatch.StartNew();
        }

        public string MakeRequest(string cardName,string set = "",bool backFace = false, bool exact = true)
        {
            string param = exact ? "exact" : "fuzzy";
            string setParam = String.IsNullOrWhiteSpace(set) ? "" : $"&set={set}";
            string backParam = backFace ? "&face=back" : "";
            string requestURL = $"{cardURL}?{param}={cardName.Replace(' ','+')}&format=image&version=png{setParam}{backParam}";
            string fileName = $@"tmp\{cardName.Replace('/','-')}.png";
            var fi = new FileInfo(fileName);

            if (fi.Exists)
            {
                return fileName;
            }
            else
            {
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
            }

            if (scryfallDelay.ElapsedMilliseconds < 100)
            {
                Thread.Sleep(100 - (int)scryfallDelay.ElapsedMilliseconds);
            }

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(requestURL), fileName);
                scryfallDelay.Restart();
            }

            if (File.Exists(fileName))
            {
                Console.WriteLine($"File made: {fileName}");
                return fileName;
            }
            else
            {
                Console.WriteLine("Unable to get card image");
                return "";
            }
        }
    }
}
