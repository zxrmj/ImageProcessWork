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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PhotoMart
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        string[] songs =
        {
            "不经一番寒彻骨，怎得梅花扑鼻香",
            "春江水暖鸭先知，萎蒿满地芦芽短",
            "花间漫扑双蝴蝶，宿露偏沾翠袖寒",
            "梨花风起正清明，游子寻春半出城",
            "绿蔓如藤栽，淡青花绕篱",
            "小荷贴水点横塘，蝶衣晒粉忙",
            "绿树阴浓夏日长，楼台倒影入池塘",
            "明月松间照，清泉石上流",
            "蒹葭苍苍，白露为霜",
            "秋阴不散霜飞晚，留得枯荷听雨声",
            "孤舟蓑笠翁，独钓寒江雪",
            "朔风吹雪飞万里，三更簌簌呜窗纸",
        };


        public MainWindow()
        {
            InitializeComponent();
            var story = Resources["welcomeAnime"] as Storyboard;
            story.Begin();
        }

        /// <summary>
        /// 窗口标题区鼠标移动事件，将使窗口拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Application.Current.MainWindow.DragMove();
            }
            return;
        }

        /// <summary>
        /// 窗口标题鼠标左键双击事件，窗口将最大化或还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                Application.Current.MainWindow.WindowState =
                    Application.Current.MainWindow.WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
            }
        }

        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
            return;
        }

        /// <summary>
        /// 最大化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
        }

        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btShutdown_Click(object sender, RoutedEventArgs e)
        {
            var close = (Storyboard)this.Resources["closeAnime"];
            close.Completed += (param1,param2) =>
            {
                Application.Current.Shutdown();
            };
            close.Begin();
            return;
        }

        /// <summary>
        /// 窗口加载事件，将播放窗口淡出动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainwindow_Loaded(object sender, RoutedEventArgs e)
        {
            var start = (Storyboard)this.Resources["startAnime"];
            start.Completed += (param1, param2) =>
            {
                return;
            };
            start.Begin();
            return;
        }

        /// <summary>
        /// 放大按钮单机事件，将弹出放大功能区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btZoom_Click(object sender, RoutedEventArgs e)
        {
            ppZoom.IsOpen = true;
            return;
        }

        /// <summary>
        /// 点击设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOption_Click(object sender, RoutedEventArgs e)
        {
            ppOption.IsOpen = true;
            return;
        }

        /// <summary>
        /// 鼠标拖动事件
        /// </summary>
        System.Windows.Point oldPos;

        /// <summary>
        /// 图像鼠标左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                oldPos = e.GetPosition(ImagePanel);
                MainImage.Cursor = Cursors.Hand;
            }
            
        }

        int movecnt = 0;

        /// <summary>
        /// 图像鼠标拖动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(movecnt % 3 == 0)
                {
                    System.Windows.Point newPos = e.GetPosition(ImagePanel);
                    double offset_x = newPos.X - oldPos.X;
                    double offset_y = newPos.Y - oldPos.Y;
                    MainImage.SetValue(Canvas.LeftProperty, (double)MainImage.GetValue(Canvas.LeftProperty) + offset_x);
                    MainImage.SetValue(Canvas.TopProperty, (double)MainImage.GetValue(Canvas.TopProperty) + offset_y);
                    oldPos = newPos;
                }
                else
                {
                    
                }
                movecnt++;
            }
            
            
        }

        /// <summary>
        /// 图像鼠标左键抬起事件
        /// </summary>
        private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainImage.Cursor = null;
        }

        /// <summary>
        /// 图像容器大小改变事件
        /// </summary>
        private void ImagePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var story = this.Resources["imageMoveAnime"] as Storyboard;
            double new_x, new_y;
            var db1 = story.Children[0] as DoubleAnimation;
            var db2 = story.Children[1] as DoubleAnimation;
            db1.To = new_x = ImagePanel.ActualWidth / 2 - MainImage.Width / 2;
            db2.To = new_y = ImagePanel.ActualHeight / 2 - MainImage.Height / 2;
            db1.Completed += (a, b) =>
            {
                MainImage.SetValue(Canvas.LeftProperty, new_x);
            };
            db2.Completed += (a, b) =>
            {
                MainImage.SetValue(Canvas.TopProperty, new_y);
            };
            story.Begin();
        }

        /// <summary>
        /// 图像缩放
        /// </summary>
        /// <param name="signal">缩放比例</param>
        private void MainImage_Zoom(double signal)
        {
            var imagezoom = (Storyboard)this.Resources["imageZoomAnime"];
            var db1 = imagezoom.Children[0] as DoubleAnimation;
            var db2 = imagezoom.Children[1] as DoubleAnimation;
            var db3 = imagezoom.Children[2] as DoubleAnimation;
            var db4 = imagezoom.Children[3] as DoubleAnimation;
            double new_width, new_height;
            db1.To = new_width = MainImage.Width * signal;
            db2.To = new_height = MainImage.Height * signal;
            double new_x, new_y;
            db3.To = new_x = (double)MainImage.GetValue(Canvas.LeftProperty) + (MainImage.Width - MainImage.Width * signal) / 2;
            db4.To = new_y = (double)MainImage.GetValue(Canvas.TopProperty) + (MainImage.Height - MainImage.Height * signal) / 2;
            db1.Completed += (a, b) =>
            {
                MainImage.Width = new_width;
            };
            db2.Completed += (a, b) =>
            {
                MainImage.Height = new_height;
            };
            db3.Completed += (a, b) =>
            {
                MainImage.SetValue(Canvas.LeftProperty, new_x);
            };
            db4.Completed += (a, b) =>
            {
                MainImage.SetValue(Canvas.TopProperty, new_y);
            };
            imagezoom.Begin();
            ShowMessage(((int)(new_width * 100 / rawSize.X)).ToString() + "%");

        }

        System.Windows.Point rawSize;
        double scaleIncrement = 1.2;
        private void imageZoomUp(int cnt)
        {
            MainImage_Zoom(scaleIncrement);

        }

        private void imageZoomDown(int cnt)
        {
            MainImage_Zoom(1 / scaleIncrement);
        }

        /// <summary>
        /// 点击放大按钮事件
        /// </summary>
        private void btImageZoomUp_Click(object sender, RoutedEventArgs e)
        {

            imageZoomUp(1);
            
        }
        
        /// <summary>
        /// 点击缩小按钮事件
        /// </summary>
        private void btImageZoomDown_Click(object sender, RoutedEventArgs e)
        {
            imageZoomDown(1);
        }

        /// <summary>
        /// 图像容器内鼠标滚轮事件
        /// </summary>
        private void ImagePanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                return;
            }
            if(e.Delta > 0)
            {
                imageZoomUp(1);
            }
            else if(e.Delta <0)
            {
                imageZoomDown(1);
            }
        }

        /// <summary>
        /// 左侧控制区显示隐藏事件
        /// </summary>
        private void btMenuBorderCtrl_Click(object sender, RoutedEventArgs e)
        {
            if(MenuBorder.Width.Value == 0)
            {
                MenuBorder.Width = new GridLength(200);
            }
            else
            {
                MenuBorder.Width = new GridLength(0);
            }
        }

        /// <summary>
        /// 将图片缩放到100%
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btImageNormal_Click(object sender, RoutedEventArgs e)
        {
            MainImage.SetValue(Canvas.LeftProperty, ImagePanel.ActualWidth / 2 - MainImage.Width / 2);
            MainImage.SetValue(Canvas.TopProperty, ImagePanel.ActualHeight / 2 - MainImage.Height / 2);
            var zoomScale = MainImage.ActualWidth / rawSize.X;
            MainImage_Zoom(1 / zoomScale);
            
        }

        private void MainImage_Loaded(object sender, RoutedEventArgs e)
        {
            rawSize.X = MainImage.ActualWidth;
            rawSize.Y = MainImage.ActualHeight;
        }


        private Image<Bgr, byte> mainImage;
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        private string filepath;
        private string filename;


        /// <summary>
        /// 打开图片
        /// </summary>
        /// <param name="path"></param>
        private bool ImageOpen(string path)
        {
            try
            {
                mainImage = new Image<Bgr, byte>(path);
            }
            catch (Exception)
            {
                ShowMessage("未能解析图像格式");
                return false;
            }
            ImageTitle.Content = filename = path.Split('\\').Last();
            ShowImageToUI(mainImage);
            
            filepath = path;
            return true;
        }

        /// <summary>
        /// 将图片标题表示为图片文件完整路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageTitle_MouseEnter(object sender, MouseEventArgs e)
        {
            if(filepath == null)
            {
                ImageTitle.Content = songs[DateTime.Now.Month];
            }
            else
            {
                ImageTitle.Content = filepath;
            }
        }

        /// <summary>
        /// 还原为图片标题显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageTitle_MouseLeave(object sender, MouseEventArgs e)
        {
            if (filepath == null)
            {
                ImageTitle.Content = "欢迎使用PhotoMart";
            }
            else
            {
                ImageTitle.Content = filename;
            }
        }

        /// <summary>
        /// 将图片绘制到用户界面
        /// </summary>
        /// <param name="src"></param>
        private void ShowImageToUI(Image<Bgr, byte> src)
        {
            var bitmap = src.ToBitmap();
            IntPtr ip = bitmap.GetHbitmap();
            BitmapSource bitmapSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);
            MainImage.Width = src.Width;
            MainImage.Height = src.Height;
            MainImage.Source = bitmapSrc;
            rawSize.X = MainImage.Width;
            rawSize.Y = MainImage.Height;

            MainImage.SetValue(Canvas.LeftProperty, ImagePanel.ActualWidth / 2 - MainImage.Width / 2);
            MainImage.SetValue(Canvas.TopProperty, ImagePanel.ActualHeight / 2 - MainImage.Height / 2);
            AdaptiveImage();
            
            // 调整大小数值
            if (rbtChangeSizePixel.IsChecked == true)
            {
                tbChangeSizeWidth.Text = string.Format("{0}", rawSize.X);
                tbChangeSizeHeight.Text = string.Format("{0}", rawSize.Y);
            }
            else
            {
                tbChangeSizeWidth.Text = string.Format("{0}", 100);
                tbChangeSizeHeight.Text = string.Format("{0}", 100);
            }
            spChangeSize.Visibility = Visibility.Collapsed;
            
            // 平移数值
            tbImageMoveWidth.Text = "0";
            tbImageMoveHeight.Text = "0";
            spImageMove.Visibility = Visibility.Collapsed;

            // 调整角度数值
            tbImageRotateAngle.Text = "0";
            spImageRotate.Visibility = Visibility.Collapsed;
            GC.Collect();

            // 直方图
            has_hist = false;
        }

        /// <summary>
        /// 图像自适应大小，铺满整个图像显示区
        /// </summary>
        private void AdaptiveImage()
        {
            var length = 40;
            if(MainImage.Width > ImagePanel.ActualWidth - length || MainImage.Height > ImagePanel.ActualHeight - length)
            {
                double new_x = MainImage.Width, new_y = MainImage.Height;
                double zoomScale = 1.0;
                bool flag = false;
                while(flag == false)
                {
                    new_x = new_x / scaleIncrement;
                    new_y = new_y / scaleIncrement;
                    zoomScale = zoomScale / scaleIncrement;
                    if (new_x < ImagePanel.ActualWidth - length && new_y < ImagePanel.ActualHeight - length) 
                    {
                        flag = true;
                    }
                }
                MainImage_Zoom(zoomScale);
            }
            else if(MainImage.Height < ImagePanel.ActualHeight - length || MainImage.Width < ImagePanel.ActualWidth - length)
            {
                double new_x = MainImage.Width, new_y = MainImage.Height;
                double zoomScale = 1.0;
                bool flag = false;
                while (flag == false)
                {
                    zoomScale = zoomScale * scaleIncrement;
                    new_x = new_x * scaleIncrement;
                    new_y = new_y * scaleIncrement;
                    if (new_x > ImagePanel.ActualWidth - length || new_y > ImagePanel.ActualHeight - length)
                    {
                        flag = true;
                        zoomScale = zoomScale / scaleIncrement;
                    }
                }
                MainImage_Zoom(zoomScale);
            }

        }

        /// <summary>
        /// 打开图片
        /// </summary>
        private void btImageOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "图像文件|*.bmp;*.jpg;*png|位图文件|*.bmp|JPEG文件|*.jpg|PNG文件|*.png|所有文件|*.*";
            var result = openFileDialog.ShowDialog();
            if(result == true)
            {
                result = ImageOpen(openFileDialog.FileName);
                if(result == true)
                {
                    btImageSave.IsEnabled = true;
                    welcomePanel.Visibility = Visibility.Hidden;
                    tbChangeSizeHeight.IsEnabled = true;
                    tbChangeSizeWidth.IsEnabled = true;
                    cbChangeSizeKeep.IsEnabled = true;
                    tbImageMoveHeight.IsEnabled = true;
                    tbImageMoveWidth.IsEnabled = true;
                    tbImageRotateAngle.IsEnabled = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                ShowMessage("未打开文件");
            }
        }

        /// <summary>
        /// 图片大小调整至最佳状态
        /// </summary>
        private void btImageAdaptive_Click(object sender, RoutedEventArgs e)
        {
            AdaptiveImage();
        }

        /// <summary>
        /// 文本框键盘按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 文本框粘贴事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
             if(e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = e.DataObject.GetData(typeof(string)) as string;
                foreach (char c in text)
                {
                    if (!char.IsDigit(c))
                    {
                        e.CancelCommand();
                        ShowMessage("只能键入数字");
                        return;
                    }
                }
                
            }
            else
            {
                e.CancelCommand();
                return;
            }
        }

        /// <summary>
        /// 文本框文本输入预览事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    ShowMessage("只能键入数字");
                    return;
                }
            }
        }

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal);
       
        /// <summary>
        /// 消息框显示
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMessage(string msg)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += (a, b) =>
            {
                var story = this.Resources["infoDisapper"] as Storyboard;
                story.Begin();
                story.Completed += (c, d) =>
                {
                    infoPanel.Visibility = Visibility.Hidden;
                    timer.Stop();
                };
                
            };
            info.Content = msg;
            infoPanel.Visibility = Visibility.Visible;
            timer.Start();
        }

        /// <summary>
        /// 按像数调整大小按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtChangeSizePixel_Checked(object sender, RoutedEventArgs e)
        {
            if(rawSize.X > 0 && rawSize.Y>0)
            {
                tbChangeSizeWidth.Text = string.Format("{0}", rawSize.X);
                tbChangeSizeHeight.Text = string.Format("{0}", rawSize.Y);
                spChangeSize.Visibility = Visibility.Collapsed;
            }

        }

        /// <summary>
        /// 按百分比调整大小按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtChangeSizeScale_Checked(object sender, RoutedEventArgs e)
        {
            if (rawSize.X > 0 && rawSize.Y > 0)
            {
                tbChangeSizeWidth.Text = string.Format("{0}", 100);
                tbChangeSizeHeight.Text = string.Format("{0}", 100);
                spChangeSize.Visibility = Visibility.Collapsed;
            }
            
        }

        /// <summary>
        ///  改变宽度文本框文本改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbChangeSizeWidth_TextChanged(object sender, TextChangedEventArgs e)
        {

            if(cbChangeSizeKeep.IsChecked == true)
            {
                 if(rbtChangeSizePixel.IsChecked == true)
                {
                    tbChangeSizeHeight.Text = String.Format("{0}", (int)(rawSize.Y * double.Parse(tbChangeSizeWidth.Text) / rawSize.X));
                }
                 else
                {
                    tbChangeSizeHeight.Text = tbChangeSizeWidth.Text;
                }
            }
            spChangeSize.Visibility = Visibility.Visible;
            if (tbChangeSizeWidth.Text.Count() > 1)
            {
                tbChangeSizeHeight.Text = tbChangeSizeHeight.Text.TrimStart('0');
                tbChangeSizeHeight.SelectionStart = tbChangeSizeHeight.Text.Count();
            }
        }

       /// <summary>
       /// 改变高度文本框文本改变事件
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void tbChangeSizeHeight_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (cbChangeSizeKeep.IsChecked == true)
            {
                if (rbtChangeSizePixel.IsChecked == true)
                {
                    try
                    {
                        tbChangeSizeWidth.Text = String.Format("{0}", (int)(rawSize.X * double.Parse(tbChangeSizeHeight.Text) / rawSize.Y));
                    }
                    catch
                    {
                        tbChangeSizeWidth.Text = "0";
                    }
                    
                }
                else
                {
                    tbChangeSizeWidth.Text = tbChangeSizeWidth.Text;
                }
            }
            spChangeSize.Visibility = Visibility.Visible;
            if (tbChangeSizeWidth.Text.Count() > 1)
            {
                tbChangeSizeWidth.Text = tbChangeSizeWidth.Text.TrimStart('0');
                tbChangeSizeWidth.SelectionStart = tbChangeSizeWidth.Text.Count();
            }
        }

        /// <summary>
        /// 执行改变大小事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btChangeSizeOK_Click(object sender, RoutedEventArgs e)
        {
            if(tbChangeSizeWidth.Text.Count() == 0 || tbChangeSizeHeight.Text.Count() == 0)
            {
                ShowMessage("请输入一个正数");
                return;
            }
            if (rbtChangeSizePixel.IsChecked == true)
            {
                mainImage = mainImage.Resize(int.Parse(tbChangeSizeWidth.Text), int.Parse(tbChangeSizeHeight.Text), Emgu.CV.CvEnum.Inter.Lanczos4);
            }
            else
            {
                if (cbChangeSizeKeep.IsChecked == true)
                {
                    mainImage = mainImage.Resize(int.Parse(tbChangeSizeWidth.Text) / 100.0, Emgu.CV.CvEnum.Inter.Lanczos4);
                }
                else
                {
                    int new_width = (int)(rawSize.X * int.Parse(tbChangeSizeWidth.Text) / 100.0);
                    int new_height = (int)(rawSize.Y * int.Parse(tbChangeSizeHeight.Text) / 100.0);
                    mainImage = mainImage.Resize(new_width, new_height, Emgu.CV.CvEnum.Inter.Lanczos4);
                }
            }
            ShowImageToUI(mainImage);
            spChangeSize.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 取消改变大小事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btChangeSizeCancel_Click(object sender, RoutedEventArgs e)
        {
            if (rbtChangeSizePixel.IsChecked == true)
            {
                tbChangeSizeWidth.Text = string.Format("{0}", rawSize.X);
                tbChangeSizeHeight.Text = string.Format("{0}", rawSize.Y);
            }
            else
            {
                tbChangeSizeWidth.Text = string.Format("{0}", 100);
                tbChangeSizeHeight.Text = string.Format("{0}", 100);
            }
            spChangeSize.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 点击保存纵横比按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChangeSizeKeep_Checked(object sender, RoutedEventArgs e)
        {
            if (rbtChangeSizePixel.IsChecked == true)
            {
                tbChangeSizeHeight.Text = String.Format("{0}", (int)(rawSize.Y * double.Parse(tbChangeSizeWidth.Text) / rawSize.X));
            }
            else
            {
                tbChangeSizeHeight.Text = tbChangeSizeWidth.Text;
            }
        }

        /// <summary>
        /// 允许输入负号的文本框键盘键入预览事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextboxAllowNeg_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!(char.IsDigit(c) || (c == '-' && ((TextBox)sender).Text.Count() == 0)))
                {
                    e.Handled = true;
                    ShowMessage("只能键入负号或数字");
                    return;
                }
            }
        }

        /// <summary>
        /// 允许输入负号的文本框文本改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextboxAllowNeg_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rawSize.X > 0 && rawSize.Y > 0)
            {
                spImageMove.Visibility = Visibility.Visible;
                if(tbImageMoveWidth.Text.Count() > 1)
                {
                    tbImageMoveWidth.Text = tbImageMoveWidth.Text.TrimStart('0');
                    tbImageMoveWidth.SelectionStart = tbImageMoveWidth.Text.Count();
                }
                if (tbImageMoveHeight.Text.Count() > 1)
                {
                    tbImageMoveHeight.Text = tbImageMoveHeight.Text.TrimStart('0');
                    tbImageMoveHeight.SelectionStart = tbImageMoveHeight.Text.Count();
                } 
            }
        }

        /// <summary>
        /// 平移取消事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btImageMoveCancel_Click(object sender, RoutedEventArgs e)
        {
            tbImageMoveWidth.Text = "0";
            tbImageMoveHeight.Text = "0";
            spImageMove.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 平移确定事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btImageMoveOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbImageMoveWidth.Text.Count() == 0 || tbImageMoveHeight.Text.Count() == 0)
            {
                ShowMessage("请输入平移量");
                return;
            }
            int offset_x = int.Parse(tbImageMoveWidth.Text);
            int offset_y = int.Parse(tbImageMoveHeight.Text);
            System.Windows.Media.Color color = ((SolidColorBrush)btImageMoveSelectColor.Background).Color;
            Bgr bgr = new Bgr(color.B, color.G, color.R);
            Image<Bgr, byte> temp = new Image<Bgr, byte>(mainImage.Width, mainImage.Height, bgr);
            for(int i = offset_y < 0 ? -offset_y:0;i< (offset_y >0 ? mainImage.Rows - offset_y : mainImage.Rows);i++)
            {
                for(int j = offset_x < 0? -offset_x:0;j< (offset_x >0?mainImage.Cols - offset_x : mainImage.Cols);j++)
                {
                    temp[i + offset_y, j + offset_x] = mainImage[i, j];
                }
            }
            mainImage = temp;
            ShowImageToUI(mainImage);
        }

        /// <summary>
        /// 允许输入符号的输入框粘贴事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextboxAllowNeg_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            Regex reg = new Regex("^-?[0-9]+");
            if(e.DataObject.GetDataPresent(typeof(string)))
            {
                var result = reg.IsMatch((String)e.DataObject.GetData(typeof(string)));
                if (result == false)
                {
                    e.CancelCommand();
                    ShowMessage("只能键入负号或数字");
                    return;
                }
                else
                {
                    ((TextBox)sender).Text = "";
                    return;
                }
            }
            else
            {
                e.CancelCommand();
                return;
            }

        }

        private void tbImageRotateAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rawSize.X > 0 && rawSize.Y > 0)
            {
                spImageRotate.Visibility = Visibility.Visible;
                if (tbImageRotateAngle.Text.Count() > 1)
                {
                    tbImageRotateAngle.Text = tbImageRotateAngle.Text.TrimStart('0');
                    tbImageRotateAngle.SelectionStart = tbImageRotateAngle.Text.Count();
                }
            }
        }

        /// <summary>
        /// 确认旋转图片事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btImageRotateOK_Click(object sender, RoutedEventArgs e)
        {
            int angle = int.Parse(tbImageRotateAngle.Text);
            PointF center = new PointF(mainImage.Cols / 2, mainImage.Rows / 2);
            Mat rotMat = new Mat();
            CvInvoke.GetRotationMatrix2D(center, angle, 1.0, rotMat);
            System.Windows.Media.Color color = ((SolidColorBrush)btImageRotateSelectColor.Background).Color;
            MCvScalar bgr = new MCvScalar(color.B, color.G, color.R);
            Image<Bgr, byte> temp = new Image<Bgr, byte>(mainImage.Width, mainImage.Height);
            CvInvoke.WarpAffine(mainImage, temp, rotMat, temp.Size, Emgu.CV.CvEnum.Inter.Lanczos4, Emgu.CV.CvEnum.Warp.FillOutliers , Emgu.CV.CvEnum.BorderType.Constant,bgr);
            mainImage = temp;
            ShowImageToUI(mainImage);
            
            
        }

        /// <summary>
        /// 取消旋转图片事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btImageRotateCancel_Click(object sender, RoutedEventArgs e)
        {
            tbImageRotateAngle.Text = "0";
            spImageRotate.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 开启选择颜色对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSelectColor_Click(object sender, RoutedEventArgs e)
        {
            if (mainImage == null)
            {
                ShowMessage("请先加载图片");
                return;
            }
            Button button = sender as Button;
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            System.Windows.Media.Color color = new System.Windows.Media.Color();
            SolidColorBrush scb = new SolidColorBrush();
            scb = (SolidColorBrush)button.Background;
            color = scb.Color;
            colorDialog.Color = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
            colorDialog.AllowFullOpen = true;
            var result = colorDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
                return;
            color.A = colorDialog.Color.A;
            color.B = colorDialog.Color.B;
            color.G = colorDialog.Color.G;
            color.R = colorDialog.Color.R;
            scb = new SolidColorBrush();
            scb.Color = color;
            button.Background = scb;
            if(color.B + color.G + color.R > 384)
            {
                button.Foreground = System.Windows.Media.Brushes.Black;
            }
            else
            {
                button.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void ContentPresenter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentPresenter cp = (ContentPresenter)sender;
            Expander par = (Expander)cp.TemplatedParent;
            par.IsExpanded = !par.IsExpanded;
        }

        private void btImageConstrastAdjust_Click(object sender, RoutedEventArgs e)
        {
            if(mainImage == null)
            {
                ShowMessage("请先加载图片");
                return;
            }
            ConstrastAdjust ca = new ConstrastAdjust();
            ca.ShowDialog();
        }

        public void constrastAdjust(int mode, ConstrastAdjust ca)
        {

            if(mode == 0)
            {
                double[] x = new double[2];
                x[0] = Canvas.GetLeft(ca.thumb1) + ca.thumb1.ActualWidth / 2;
                x[1] = Canvas.GetLeft(ca.thumb2) + ca.thumb2.ActualWidth / 2;
                for(int i =0;i<3;i++)
                {
                    ca.lf[i].create_lut();
                }
                for (int i = 0;i<mainImage.Rows;i++)
                {
                    for(int j = 0;j<mainImage.Cols;j++)
                    {
                        for (int k = 0; k < mainImage.NumberOfChannels; k++)
                        {
                            if (mainImage.Data[i, j, k] <= x[0])
                            {
                                mainImage.Data[i, j, k] = (byte)ca.lf[0].lookup(mainImage.Data[i, j, k]);
                            }
                            else if (mainImage.Data[i, j, k] > x[0] && mainImage.Data[i, j, k] <= x[1])
                            {
                                mainImage.Data[i, j, k] = (byte)ca.lf[1].lookup(mainImage.Data[i, j, k]);
                            }
                            else
                            {
                                mainImage.Data[i, j, k] = (byte)ca.lf[2].lookup(mainImage.Data[i, j, k]);
                            }
                        }
                        
                    }
                }
            }
            else if(mode == 1)
            {
                ca.pf.create_lut();
                for (int i = 0; i < mainImage.Rows; i++)
                {
                    for (int j = 0; j < mainImage.Cols; j++)
                    {
                        for (int k = 0; k < mainImage.NumberOfChannels; k++)
                        {
                            mainImage.Data[i, j, k] = (byte)ca.pf.compute(mainImage.Data[i, j, k]);
                        }

                    }
                }
            }
            else
            {
                ca.lgf.create_lut();
                for (int i = 0; i < mainImage.Rows; i++)
                {
                    for (int j = 0; j < mainImage.Cols; j++)
                    {
                        for (int k = 0; k < mainImage.NumberOfChannels; k++)
                        {
                            mainImage.Data[i, j, k] = (byte)ca.lgf.compute(mainImage.Data[i, j, k]);
                        }

                    }
                }
            }
            ShowImageToUI(mainImage);
        }

        private bool has_hist = false;

        private void showHist()
        {
            histPanel.Children.Clear();
            Dictionary<byte, int> dict = new Dictionary<byte, int>();
            for (int i = 0; i < mainImage.Rows; i++)
            {
                for (int j = 0; j < mainImage.Cols; j++)
                {
                    for (int k = 0; k < mainImage.NumberOfChannels; k++)
                    {
                        if (dict.ContainsKey(mainImage.Data[i, j, k]))
                        {
                            dict[mainImage.Data[i, j, k]]++;
                        }
                        else
                        {
                            dict[mainImage.Data[i, j, k]] = 1;
                        }
                    }

                }
            }
            double max = dict.Values.Max();
            double min = dict.Values.Min();
            for (int i = 0; i < dict.Keys.Count; i++)
            {
                int x = dict.Keys.ElementAt(i);
                int y = dict[(byte)x] - (int)min;
                y = (int)(y * 200.0 / (max - min));
                System.Windows.Shapes.Line line = new Line();
                line.Stroke = System.Windows.Media.Brushes.Gray;
                line.StrokeThickness = 1;
                line.X1 = x;
                line.X2 = x;
                line.Y1 = 200;
                line.Y2 = 200 - y;
                histPanel.Children.Add(line);
            }
            has_hist = true;
        }
        private void btHist_Click(object sender, RoutedEventArgs e)
        {
            if(mainImage == null)
            {
                ShowMessage("请先加载图片");
                return;
            }
            showHist();
            ppHist.IsOpen = true;
        }

        private void btHistNorm_Click(object sender, RoutedEventArgs e)
        {
            mainImage._EqualizeHist();
            showHist();
            ShowImageToUI(mainImage);
            
        }

        private void btHistSpec_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "图像文件|*.bmp;*.jpg;*png|位图文件|*.bmp|JPEG文件|*.jpg|PNG文件|*.png|所有文件|*.*";
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                Image<Bgr, byte> right = new Image<Bgr, byte>(openFileDialog.FileName);
                double[] left_hist = new double[256];
                double[] right_hist = new double[256];
                foreach (var b in mainImage.Data)
                {
                    left_hist[b]++;
                }
                foreach (var b in right.Data)
                {
                    right_hist[b]++;
                }
                for (int i = 0; i < left_hist.Count(); i++)
                {
                    left_hist[i] /= mainImage.Data.Length;
                    if (i > 0)
                    {
                        left_hist[i] += left_hist[i - 1];
                    }
                }
                for (int i = 0; i < right_hist.Count(); i++)
                {
                    right_hist[i] /= right.Data.Length;
                    if (i > 0)
                    {
                        right_hist[i] += right_hist[i - 1];
                    }
                }
                Dictionary<byte, byte> lut = new Dictionary<byte, byte>();
                for (int i = 0; i < 256; i++) 
                {
                    double diff_tmp = double.PositiveInfinity;
                    int diff_idx = 0;
                    for (int j = 0; j < 256; j++) 
                    {
                        double diff = Math.Abs(left_hist[i] - right_hist[j]);
                        if(diff < diff_tmp)
                        {
                            diff_tmp = diff;
                            diff_idx = j;
                        }
                    }
                    lut[(byte)i] = (byte)diff_idx;
                }
                lut[255] = 255;
                // 规定化
                for (int i = 0; i < mainImage.Rows; i++)
                {
                    for (int j = 0; j < mainImage.Cols; j++)
                    {
                        for (int k = 0; k < mainImage.NumberOfChannels; k++)
                        {
                            mainImage.Data[i, j, k] = lut[mainImage.Data[i, j, k]];
                        }

                    }
                }
                showHist();
                ShowImageToUI(mainImage);
            }
        }
        Random rand = new Random(System.DateTime.Now.Millisecond * (DateTime.Now.Second + 1) * (DateTime.Now.Minute + 1));
        private void shootDanmaku(string text)
        {
            
            if (text.Length > 0)
            {
                Label label = new Label();
                label.Content = text;
                
                label.SetValue(Canvas.BottomProperty, (double)rand.Next(0, (int)ImagePanel.ActualHeight));
                label.FontFamily = new System.Windows.Media.FontFamily("黑体");
                label.FontSize = 20;
                label.IsHitTestVisible = false;
                
                ImagePanel.Children.Add(label);
                DoubleAnimation da = new DoubleAnimation();
                da.From = ImagePanel.ActualWidth;
                da.To = -(20 * text.Length);
                da.Duration = new Duration(new TimeSpan(0, 0, 10));
                if (rand.NextDouble() < 0.05)
                {
                    label.Foreground = System.Windows.Media.Brushes.Red;
                    da.Duration = new Duration(new TimeSpan(0, 0, 3));
                }
                da.Completed += (a, b) =>
                {
                    ImagePanel.Children.Remove(label);
                };
                label.BeginAnimation(Canvas.LeftProperty, da);
            }
        }

        private void TextBox_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string text = ((TextBox)sender).Text;
                shootDanmaku(text);
            }
        }

        private void ImageTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                shootDanmaku((string)((Label)sender).Content);
            }
        }
    }
}
