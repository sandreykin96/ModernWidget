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

namespace EuroWIdjet
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(Color.FromArgb(0, 34, 34, 34));

            //Сгенерирум данные для графиков

            int Np = 30;
            double[] Data1 = new double[Np + 1];

            for (int i = 0; i < Np + 1; i++)
            {
                Data1[i] = Math.Sin(i / 5.0) + 1;
            }

            //Теперь нарисуем график

            DrawingGroup aDrawingGroup = new DrawingGroup();
            GeometryDrawing drw = new GeometryDrawing();
            GeometryGroup gg = new GeometryGroup();

            drw.Brush = Brushes.Red;
            drw.Pen = new Pen(Brushes.Pink, 0.05);

            gg = new GeometryGroup();
            for (int i = 0; i < Np; i++)
            {
                LineGeometry l = new LineGeometry(new Point((double)i / (double)Np, 1.0 - (Data1[i] / 2.0)),
                    new Point((double)(i + 1) / (double)Np, 1.0 - (Data1[i + 1] / 2.0)));
                gg.Children.Add(l);
            }

            //Обрезание лишнего
            {
                drw.Brush = Brushes.Transparent;
                drw.Pen = new Pen(Brushes.White, 0.2);

                //RectangleGeometry myRectGeometry = new RectangleGeometry();
                //myRectGeometry.Rect = new Rect(-0.1, -0.1, 1.2, 1.2);
                //gg.Children.Add(myRectGeometry);

                //Numbers
                drw.Pen = new Pen(Brushes.FloralWhite, 0.005);

                for (int i = 1; i < 10; i++)
                {

                    // Create a formatted text string.
                    FormattedText formattedText = new FormattedText(
                        ((double)(1 - i * 0.1)).ToString(),
                        CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface("Verdana"),
                        0.04,
                        Brushes.Green);

                    // Build a geometry out of the formatted text.
                    Geometry geometry = formattedText.BuildGeometry(new Point(-0.1, i * 0.1 - 0.03));
                    gg.Children.Add(geometry);
                }

                drw.Geometry = gg;
                aDrawingGroup.Children.Add(drw);

                image1.Source = new DrawingImage(aDrawingGroup);
            }

        }
        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr window, int index);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public const int HWND_BOTTOM = 0x1;
        public const uint SWP_NOSIZE = 0x1;
        public const uint SWP_NOMOVE = 0x2;
        public const uint SWP_SHOWWINDOW = 0x40;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

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
}
