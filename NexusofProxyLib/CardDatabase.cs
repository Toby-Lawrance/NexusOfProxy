using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NexusofProxyLib
{
    class CardDatabase
    {
        void LoadFromFile(string path)
        {
            if (File.Exists(path))
            {
               var contents = File.ReadAllText(path);

               var obj = JObject.Parse(contents);
            }
            
        }
    }
}
