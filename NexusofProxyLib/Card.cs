using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusofProxyLib
{
    public class Card
    {
        public string name;
        public string scryfallOracleId;
        public IEnumerable<string> printings;
        public string layout;

        public Card(string name, string scryfallOracleId, IEnumerable<string> printings, string layout)
        {
            this.name = name;
            this.scryfallOracleId = scryfallOracleId;
            this.printings = printings;
            this.layout = layout;
        }

        public bool HasPrinting(string set)
        {
            return printings.Contains(set);
        }


        public override string ToString()
        {
            return $"({name},{layout},{scryfallOracleId})";
        }
    }
}
