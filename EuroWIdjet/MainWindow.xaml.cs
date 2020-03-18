using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Globalization;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace EuroWIdjet
{
    public partial class MainWindow : Window
    {

        public const int HWND_BOTTOM = 0x1;
        public const uint SWP_NOSIZE = 0x1;
        public const uint SWP_NOMOVE = 0x2;
        public const uint SWP_SHOWWINDOW = 0x40;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr window, int index);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public DrawingImage Graph { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Background = new SolidColorBrush(Color.FromArgb(0, 34, 34, 34));

            var valueList = new Dictionary<DateTime, double>();
            int i = 0;
            while (i < 10)
            {
                double currensy = GetCurrency();
               
                valueList.Add(DateTime.Now, currensy);
                lineChart.DataContext = valueList;
                
                Thread.Sleep(1000);
                i++;
            }

        }

        double GetCurrency()
        {
            return Currency.GetCourse();
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ShoveToBackground()
        {
            SetWindowPos((int)this.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        public static void HideFromAltTab(IntPtr Handle)
        {
            SetWindowLong(Handle, GWL_EXSTYLE, GetWindowLong(Handle, GWL_EXSTYLE) | WS_EX_TOOLWINDOW);
        }

        private IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(this).Handle;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HideFromAltTab(Handle);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ShoveToBackground();
        }
    }

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
                    var a = Regex.Replace(item, @"[^\d-[.]]", "").Replace(".", ",");
                    double r = Convert.ToDouble(a);
                    arr.Add(r);
                }
            }
            return arr;
        }
    }
}
