using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NexusofProxyLib;

namespace TestDriver
{
    class Program
    {
        private static string[] flipLayouts = {"modal_dfc", "transform", "meld", "double_faced_token"};

        static void Main(string[] args)
        {
            var cache = new MTGJson();
            var end = new ScryfallEndPoint();

            Console.Write("Enter path to deck file: ");
            string deckPath = Console.ReadLine();

            Console.Write("Would you like a blank page after each?[y/n]:");
            bool empty = Console.ReadKey().Key == ConsoleKey.Y;
            Console.WriteLine();

            var fi = new FileInfo(deckPath);

            if (fi.Exists)
            {
                var pdf = new PDFCreator(end,empty);
                var lines = File.ReadAllLines(deckPath);
                int count = 0;
                foreach (var line in lines)
                {
                    var vals = ParseLine(line, cache);
                    if (vals is not null)
                    {
                        foreach (var valueTuple in vals)
                        {
                            for (int i = 0; i < valueTuple.Item1; i++)
                            {
                                pdf.AddCard(valueTuple.Item2, valueTuple.Item3,valueTuple.Item4);
                            }
                        }
                    }
                    Console.WriteLine($"[{++count}/{lines.Length}]");
                }

                pdf.SavePDF($"{(fi.Name).Remove((fi.Name.Length - fi.Extension.Length))}.pdf");
            }
        }

        static bool CheckCard(string name, string set, MTGJson cache)
        {
            var c = cache.GetCard(name);
            if (c is null) return false;
            return String.IsNullOrWhiteSpace(set) || c.HasPrinting(set);
        }

        static List<(int, string, string,bool)> ParseLine(string line, MTGJson cache)
        {
            var match = Regex.Match(line,
                @"^(?<number>\d+)x? (?<name>.+?)( \/\/ (?<secondname>.+?))?( \((?<set>[A-Z]{3})\))?$");
            if (!match.Success)
            {
                return null;
            }
            else
            {
                var output = new List<(int, string, string,bool)>();
                string name = match.Groups["name"].Value;
                string secondName = "";
                string set = "";
                int num = int.Parse(match.Groups["number"].Value);
                if (match.Groups["set"].Success)
                {
                    set = match.Groups["set"].Value;
                }

                if (match.Groups["secondname"].Success)
                {
                    secondName = match.Groups["secondname"].Value;
                    var combinedName = $"{name} // {secondName}";
                    if (CheckCard(combinedName, set, cache))
                    {
                        var c = cache.GetCard(combinedName);
                        if (flipLayouts.Contains(c.layout))
                        {
                            output.Add((num, name, set,false));
                            output.Add((num, secondName, set,true));
                        }
                        else
                        {
                            output.Add((num, combinedName, set,false));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Couldn't find: {combinedName}");
                    }
                }
                else
                {
                    if (CheckCard(name, set, cache))
                    {
                        output.Add((num, name, set,false));
                    }
                    else
                    {
                        Console.WriteLine($"Couldn't find: {name}");
                    }
                }

                return output;
            }
        }
    }
}