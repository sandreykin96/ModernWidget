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

            startSycle();

        }

        private void startSycle()
        {
            int i = 0;
            var data = new Dictionary<DateTime, double>();

            while (i < 10)
            {
                double currensy = GetCurrency();
                var time = DateTime.Now;
                data.Add(time, currensy);

                Thread.Sleep(100);
                i++;
            }
            //data.Add(DateTime.Now, 79);
            //Thread.Sleep(100);
            //data.Add(DateTime.Now, 80);

            image1.Source = drawGraph(data);
        }

        private DrawingImage drawGraph(Dictionary<DateTime, double> data)
        {
            var pointsCount = data.Count - 1;

            DrawingGroup drawingGroup = new DrawingGroup();
            GeometryDrawing geometryDrawing = new GeometryDrawing();
            GeometryGroup geometryGroup = new GeometryGroup();
            geometryDrawing.Brush = Brushes.Red;
            geometryDrawing.Pen = new Pen(Brushes.Pink, 0.05);

            geometryGroup = new GeometryGroup();

            for (int i = 1; i <= pointsCount; i++)
            {
                var a = new Point((double)i*5 / (double)pointsCount, 100-data.Values.ElementAt(i));
                var b = new Point((double)(i - 1)*5 / (double)pointsCount, 100-data.Values.ElementAt(i - 1));

                LineGeometry line = new LineGeometry(a, b);
                geometryGroup.Children.Add(line);
            }

            geometryDrawing.Brush = Brushes.Transparent;
            geometryDrawing.Pen = new Pen(Brushes.White, 0.015);
            
            //Numbers

            var maxValue = data.Values.Max();
            var minValue = data.Values.Min();
            var diff = (maxValue - minValue)/10;
            
            //for (int i = 1; i < 10; i++)
            //{
            //    // Create a formatted text string.
            //    FormattedText formattedText = new FormattedText(
            //        ((double)(0 - i *diff)).ToString(),
            //        CultureInfo.GetCultureInfo("en-us"),
            //        FlowDirection.LeftToRight,
            //        new Typeface("Verdana"),
            //        0.04,
            //        Brushes.Green);

            //    //Build a geometry out of the formatted text.
            //    Geometry geometry = formattedText.BuildGeometry(new Point(0, -i * diff -minValue ));
            //    geometryGroup.Children.Add(geometry);
            //}

            geometryDrawing.Geometry = geometryGroup;
            drawingGroup.Children.Add(geometryDrawing);
            return new DrawingImage(drawingGroup);
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
                    var a = Regex.Replace(item, @"[^\d-[.]]", "")/*.Replace(".", ",")*/;
                    double r = Convert.ToDouble(a);
                    arr.Add(r);
                }
            }
            return arr;
        }
    }
}
