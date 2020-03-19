using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace EuroWIdjet
{
    static class Currency
    {
        public const string Trades = "https://api.exmo.com/v1/trades/?pair=USD_RUB";

        public static double GetCourse()
        {
            var trades = GetTrades();
            var listNums = ParseTrades(trades);
            double awerage = CalkAverage(listNums);
            return awerage;
        }

        private static string GetTrades()
        {
            string line = "";

            WebClient client = new WebClient();
            try
            {
                using (Stream stream = client.OpenRead(Trades))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        line = reader.ReadLine();
                    }
                }
            }
            catch { }

            return line;
        }

        private static double CalkAverage(List<double> arr)
        {
            double sum = 0;

            foreach (var item in arr)
            {
                sum += item;
            }

            return sum / arr.Count();
        }

        private static List<double> ParseTrades(string line)
        {
            var sentences = line.Split(',');
            var arr = new List<double>();

            foreach (var item in sentences)
            {
                if (item.Contains("price"))
                {
                    arr.Add(Convert.ToDouble(Regex.Replace(item, @"[^\d-[.]]", "").Replace(".", ",")));
                }
            }
            return arr;
        }
    }
}
