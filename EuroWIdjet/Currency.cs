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
            var trades = getTrades();
            var listNums = parseTrades(trades);
            double awerage = calkAverage(listNums);
            return awerage;
        }

        private static string getTrades()
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

        private static double calkAverage(List<double> arr)
        {
            double sum = 0;
            foreach (var item in arr)
            {
                sum += item;
            }
            return sum / arr.Count();
        }

        private static List<double> parseTrades(string line)
        {
            var sentences = line.Split(',');
            var arr = new List<double>();

            foreach (var item in sentences)
            {
                if (item.Contains("price"))
                {
                    var a = Regex.Replace(item, @"[^\d-[.]]", "")/*.Replace(".", ",")*/;
                    double r = Convert.ToDouble(a);
                    arr.Add(r);
                }
            }
            return arr;
        }
    }
}
