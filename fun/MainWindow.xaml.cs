using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Jarvis;
using OxyPlot;
using LiveCharts.Wpf;
using EO.Internal;
using LiveCharts;
using LiveCharts.Defaults;
//using LiveCharts.Configurations;
//using System.Windows.Forms.DataVisualization.Charting;
//using LiveCharts.Defaults;

namespace fun
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Helper.addvar();

        }
        //public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

       // public IList<DataPoint> Points { get; private set; }

        public ObservableCollection<XY> collection { get; set; }
        public ChartValues<ObservablePoint> ValuesY { get; private set; }
        //public ChartValues<Axis> ValuesX { get; private set; }
        // public ChartValues<Axis> Valuesxy { get; private set; }

        private void EX_Click(object sender, RoutedEventArgs e) {
            TA.Text = "0";
            TB.Text = "1";
            Num.Text = "6";
            Func.Text = "(1/2)*x*y^2-y";
            toch.Text = "e^(-x)/(((x+1)*e^(-x))/2+(1/2))";
            PCH.Text = "1";


        }
        private void PUPH_Click(object sender, RoutedEventArgs e)
        {

            // string F = Func.Text;
            double a = Convert.ToDouble(TA.Text);
            double b = Convert.ToDouble(TB.Text);
            int n = Convert.ToInt32(Num.Text);
            if (n <= 4)
            {
                Num.Text = "5";
                n = 5;
            }

            double[] x = new double[n];
            double[] y = new double[n];
            double[] dy = new double[n];
            double[] yt = new double[n];
            double[,] k = new double[4, n];
            double[] ye = new double[n];
            double[] q = new double[n];

            double h = (b - a) / (n - 1);
            x[0] = a;
            y[0] = Convert.ToDouble(PCH.Text);
            for (int i = 1; i < n; i++)
            {
                x[i] = x[i - 1] + h;
                //y[i] = f(x[i]);

            }
            //y[0] = a;
            yt[0] = ft(x[0]);
            List<tabl> tabls = new List<tabl>(n);
            //tabls.Add(new tabl(x[0], y[0] - h * f(x[i - 1], y[i - 1]), h * f(x[i - 1], y[i - 1]), ft(x[i]), ft(x[i] - y[0] - h * f(x[i - 1], y[i - 1]))
            for (int i = 1; i < 4; i++)
            {
                if (i == 0)
                {
                    yt[i] = ft(x[i]);
                    //q[i] = h * f(x[i], y[i]);
                }
                else
                {
                    k[0, i - 1] = f(x[i - 1], y[i - 1]);
                    k[1, i - 1] = f(x[i - 1] + h / 2, y[i - 1] + h * k[0, i - 1] / 2);
                    k[2, i - 1] = f(x[i - 1] + h / 2, y[i - 1] + h * k[1, i - 1] / 2);
                    k[3, i - 1] = f(x[i - 1] + h, y[i - 1] + h * k[2, i - 1]);


                    dy[i - 1] = h / 6 * (k[0, i-1] + 2 * k[1, i-1] + 2 * k[2, i-1] + k[3, i-1]);
                    y[i] = y[i - 1] + dy[i - 1];
                    yt[i] = ft(x[i]);
                    ye[i] = yt[i] - y[i];



                }

            }
            for (int i = 0; i < n; i++)
                q[i] = h * f(x[i], y[i]);
            double[] dq = new double[n - 1];
            double[] d2q = new double[n - 2];

            for (int i = 0; i < n-1; i++)
                dq[i] = q[i + 1] - q[i];
            for (int i = 0; i < n-2; i++)
            {
                d2q[i] = dq[i + 1] - dq[i];
            }
            for (int i = 4; i < n; i++)
            {
                // q[i] = h * f(x[i], y[i]);

                //y[i] = y[i - 1] + h / 24 * (55 * y[i - 1] - 59 * y[i - 2] + 37 * y[i - 3] - 9 * y[i - 4]);
                y[i] = y[i - 1] + q[i - 1] + (1 / 2) * dq[i - 2] + (5 / 12) * d2q[i - 3];
                q[i] = h * f(x[i], y[i]);
                if(i<n-1)
                    dq[i] = q[i + 1] - q[i];
                if(i<n-2)

                d2q[i] = dq[i + 1] - dq[i];

                yt[i] = ft(x[i]);
                //tabls.Add(new tabl(x[i], y[i], yt[i], q[i],dq[i],d2q[i], ye[i]));
            }
            for (int i = 0; i < n; i++)
            {
                if (i < n - 2)
                    tabls.Add(new tabl(x[i], y[i], yt[i], q[i], dq[i], d2q[i], ye[i]));
                else if (i<n-1)
                    tabls.Add(new tabl(x[i], y[i], yt[i], q[i], dq[i], 0, ye[i]));
                else if(i<n)
                    tabls.Add(new tabl(x[i], y[i], yt[i], q[i], 0, 0, ye[i]));

                //else if(i>n-2 && i < n - 1)
                //tabls.Add(new tabl(x[i], y[i], yt[i], q[i], dq[i], 0, ye[i]));
                //else if(i>n-1)
                //    tabls.Add(new tabl(x[i], y[i], yt[i], q[i], 0, 0, ye[i]));


            }
            List<XY> FU = new List<XY>(n);
            for (int i = 0; i < n; i++)
            {
                FU.Add(new XY(x[i], y[i]));
            }
            Tabl.ItemsSource = FU;
            vidp.ItemsSource = tabls;


            //-------------------------------график---------------
            DataContext = null;
            MyValues = new ChartValues<ObservablePoint>();
            MyValuesT = new ChartValues<ObservablePoint>();
            for (int i = 0; i < n; i++)
            {
                MyValues.Add(new ObservablePoint(x[i], y[i]));
                MyValuesT.Add(new ObservablePoint(x[i], yt[i]));
            }
            var lineSeries = new LineSeries
            {
                Values = MyValues,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                Title = "шукана",
                //PointGeometrySize = 10,
                //PointGeometry = DefaultGeometries.Circle,
                DataLabels = false,

            };
            var lineSeries2 = new LineSeries
            {
                Values = MyValuesT,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                Title = "уточнена",

                //PointGeometrySize = 10,
                //PointGeometry = DefaultGeometries.Circle,
                DataLabels = false,

            };
            SeriesCollection = new SeriesCollection { lineSeries, lineSeries2 };
            DataContext = this;


            //double fuz = Math.E(-x[0]) / ();
        }
        public ChartValues<ObservablePoint> MyValues { get; set; }
        public ChartValues<ObservablePoint> MyValuesT { get; private set; }

        public SeriesCollection SeriesCollection { get; set; }
        //public ChartValues<ObservablePoint> kek { get; set; }
        //public SeriesCollection kek { get; set; }
           // CartesianChart_SizeChanged
        public double f(double x, double y)
    {
        string res = Convert.ToString(Math.Round((float)Helper.Function_from_string(x, y, Func.Text), 5));
        return Convert.ToDouble(res);

    }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            MessageBox.Show(pressed.Content.ToString());
        }

        public double ft(double x)
    {
        string res = Convert.ToString(Math.Round((float)Helper.Function_from_string(x, toch.Text), 5));
        return Convert.ToDouble(res);
    }

        private void CartesianChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
    public class XY
    {
        public XY(double X,double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public double X { get; set; }
        public double Y { get; set; }
        
        //public XY(double X, double Y)
        //{
        //    this.X = X;
        //    this.Y = Y;
        //}
    }
    public class tabl
    {
        public tabl(double X, double Y, double YT, double Q,double dQ,double d2Q,double EY)
        {
            this.X = X;
            this.Y = Y;
            this.YT = YT;
            this.EY = EY;
            this.Q = Q;
            this.d2Q = d2Q;
            this.dQ = dQ;




        }
        public double X { get; set; }
        public double Y { get; set; }
        public double YT { get; set; }
        public double Q { get; }
        public double dQ { get; }
        public double d2Q { get; }
        public double EY { get; set; }



        //public XY(double X, double Y)
        //{
        //    this.X = X;
        //    this.Y = Y;
        //}
    }

}
