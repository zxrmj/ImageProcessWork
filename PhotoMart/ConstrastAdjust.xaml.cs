using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoMart
{
    public class Func
    {
        private Dictionary<double, double> lookuptable = new Dictionary<double, double>();
        private bool has_lut = false;
        public void create_lut()
        {
            lookuptable.Clear();
            for (int i = 0; i < 256; i++)
            {
                lookuptable[i] = compute(i);
            }
            has_lut = true;
        }
        public double lookup(double input)
        {
            return lookuptable[input];
        }
        public virtual double compute(double input)
        {
            return 0;
        }
    }

    public class LinearFunction: Func
    {
        public double k;
        public double b;
        public override string ToString()
        {
            return string.Format("F(x) = {0}x + {1}", Math.Round(k,2), Math.Round(b, 2));
        }
        public override double compute(double input)
        {
            if (double.IsInfinity(k))
                return input;
            double result = k * input + b;
            if (result >= 255)
                result = 255;
            return result;
        }
        
    }
    public class PowerFunction :Func
    {
        public double r;
        public override string ToString()
        {
            return string.Format("F(x) = Power(x,{0})", Math.Round(r, 2));
        }
        public override double compute(double input)
        {
            double result = Math.Pow(input, r);
            if (result >= 255)
                result = 255;
            return result;
        }

    }
    public class LogFunction:Func
    {
        public double v;
        public override string ToString()
        {
            return string.Format("F(x) = Log(x,{0})", v);
        }
        public override double compute(double input)
        {
            input /= 255;
            double result = Math.Log(input + v, v + 1);
            if (result >= 1)
                result = 1;
            return result*255;
        }

    }
    /// <summary>
    /// ConstrastAdjust.xaml 的交互逻辑
    /// </summary>
    public partial class ConstrastAdjust : Window
    {
        public int AdjustMode = 0;
        public LinearFunction[] lf = new LinearFunction[3];
        public PowerFunction pf = new PowerFunction();
        public LogFunction lgf = new LogFunction();
        public ConstrastAdjust()
        {
            InitializeComponent();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
               DragMove();
            }
        }

        private void btWindowClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // 线性调整
        private void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            double dTop = Canvas.GetTop(thumb) + e.VerticalChange;
            double dLeft = Canvas.GetLeft(thumb) + e.HorizontalChange;
            if (dTop <= -thumb.Height/2)
                dTop = -thumb.Height / 2;
            else if (dTop >= 256 - thumb.Height / 2)
                dTop = 256 - thumb.Height / 2;
            if (dLeft <= -thumb.Width / 2)
                dLeft = -thumb.Width / 2;
            else if (dLeft >= 256 - thumb.Height / 2)
                dLeft = 256 - thumb.Height / 2;
            if(thumb == thumb1)
            {
                if(dLeft >= Canvas.GetLeft(thumb2))
                {
                    dLeft = Canvas.GetLeft(thumb2);
                }
                line1.X2 = line2.X1 = dLeft + thumb.Width /2;
                line1.Y2 = line2.Y1 = dTop + thumb.Height / 2; 
            }
            else if(thumb == thumb2)
            {
                if(dLeft <= Canvas.GetLeft(thumb1))
                {
                    dLeft = Canvas.GetLeft(thumb1);
                }
                line2.X2 = line3.X1 = dLeft + thumb.Width / 2; 
                line2.Y2 = line3.Y1 = dTop + thumb.Height / 2;;
            }
            Canvas.SetTop(thumb, dTop);
            Canvas.SetLeft(thumb, dLeft);
            Line[] lines = { line1, line2, line3 };
            tb1.Text = "";
            for(int i =0;i<3;i++)
            {
                lf[i] = new LinearFunction();
                lf[i].k = (lines[i].Y1 - lines[i].Y2) / (lines[i].X1 - lines[i].X2);
                lf[i].b = lines[i].Y1 - lines[i].X1 * lf[i].k;
                tb1.Text += lf[i].ToString() + "\n";
            }
            if(lf[1].k == -1 && lf[1].b == 256)
            {
                tb2.Text = "负片";
            }
            else if(lf[0].k == 0 && lf[2].k == 0 && double.IsInfinity(lf[1].k))
            {
                tb2.Text = "二值化";
            }
            else
            {
                tb2.Text = "";
            }
        }

        private void lbMode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lbLineAdjust.Visibility = Visibility.Visible;
            lbPowerAdjust.Visibility = Visibility.Visible;
            lbLogAdjust.Visibility = Visibility.Visible;
            ((Label)sender).Visibility = Visibility.Collapsed;
            lbModeHidden.Visibility = Visibility.Visible;
        }

        private void lbModeHidden_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender == lbLineAdjust)
            {
                lbModeTitle.Content = lbLineAdjust.Content;
                AdjustMode = 0;
                LinePanel.Visibility = Visibility.Visible;
                PowerPanel.Visibility = Visibility.Hidden;
                LogPanel.Visibility = Visibility.Hidden;
                
            }
            else if(sender == lbPowerAdjust)
            {
                lbModeTitle.Content = lbPowerAdjust.Content;
                AdjustMode = 1;
                LinePanel.Visibility = Visibility.Hidden;
                PowerPanel.Visibility = Visibility.Visible;
                LogPanel.Visibility = Visibility.Hidden;
            }
            else if(sender == lbLogAdjust)
            {
                lbModeTitle.Content = lbLogAdjust.Content;
                AdjustMode = 2;
                LinePanel.Visibility = Visibility.Hidden;
                PowerPanel.Visibility = Visibility.Hidden;
                LogPanel.Visibility = Visibility.Visible;
                DrawLog(0.5);
            }
            lbLineAdjust.Visibility = Visibility.Collapsed;
            lbPowerAdjust.Visibility = Visibility.Collapsed;
            lbLogAdjust.Visibility = Visibility.Collapsed;
            lbMode.Visibility = Visibility.Visible;
            lbModeHidden.Visibility = Visibility.Collapsed;
        }

        // 幂次thumb
        private void thumb3_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            double dTop = Canvas.GetTop(thumb) + e.VerticalChange;
            if (dTop <= -thumb.Height / 2 + 20)
                dTop = -thumb.Height / 2 + 20;
            else if (dTop >= 236 - thumb.Height / 2)
                dTop = 236 - thumb.Height / 2;
            Canvas.SetTop(thumb, dTop - thumb.Height / 2);
            // 计算幂次
            double power = Math.Log(dTop / 256.0, 0.5);
            double b = 1 - power;
            // 绘制曲线
            if (power > 1)
            {
                Bezier.Point1 = new Point(-b / power, 0);
            }
            else
            {
                Bezier.Point1 = new Point(0, b);
            }
            pf.r = power;
            tb1.Text = pf.ToString();
        }

        private void DrawLog(double v)
        {
            LogPoints.Segments.Clear();
            for(double i = 0;i<1;i+=0.01)
            {
                double y = Math.Log(1 + v * i, v + 1);
                LineSegment ls = new LineSegment();
                ls.Point = new Point(i * 255, y * 255);
                LogPoints.Segments.Add(ls);
            }
        }

        private void PowerPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition((Canvas)sender);

        }

        private void thumb4_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            double dTop = Canvas.GetTop(thumb) + e.VerticalChange;
            if (dTop <= -thumb.Height / 2 + 128)
                dTop = -thumb.Height / 2 + 128;
            else if (dTop >= 236 - thumb.Height / 2)
                dTop = 236 - thumb.Height / 2;
            Canvas.SetTop(thumb, dTop - thumb.Height / 2);
            // 计算底数
            int Base = (int)Math.Floor(Math.Pow(2, (dTop - 120.5) * 9 / 108));
            DrawLog(Base);
            // 更新文字
            tb1.Text = "F(x) = Log(x," + Base.ToString() + ")";
        }

        private void btWindowOK_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).constrastAdjust(AdjustMode, this);
            Close();
        }
    }
}
