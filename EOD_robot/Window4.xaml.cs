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
using System.Windows.Shapes;

using ROS_ImageWPF;
using Ros_CSharp;

namespace EOD_robot
{
    /// <summary>
    /// Window4.xaml 的交互逻辑
    /// </summary>
    public partial class Window4 : Window
    {
        public Window4()
        {
            InitializeComponent();
        }
        public CommFunction cf = null;
        public Window6 w6 = null;
        int a = 5000;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        Thickness img_Margin;
        double img_width;
        double img_height;
        private void ImageMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CompressedImageControl img = (CompressedImageControl)sender;
            double width = this.Width;
            double height = this.Height;

            if (img.Tag != null && ((int)img.Tag == 90 || (int)img.Tag == 270))
            {
                width = this.Height;
                height = this.Width;
            }
            Visibility Visible = Visibility.Visible;
            if ((int)img.Width != (int)width)
            {

                img_Margin = img.Margin;
                img_width = img.Width;
                img_height = img.Height;
                img.Margin = new Thickness(0, 0, 0, 0);
                img.Width = width;
                img.Height = height;
                Visible = Visibility.Hidden;
            }
            else
            {
                img.Margin = img_Margin;
                img.Width = img_width;
                img.Height = img_height;
                Visible = Visibility.Visible;
            }


            for (int i = 0; i < imgList.Count; i++)
            {
                if (imgList[i] != img)
                {
                    imgList[i].Visibility = Visible;
                }
            }
        }
        private void He_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x06;
            cf.control_data[4] = 0x5a;
        }
        private void Kai_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x06;
            cf.control_data[4] = 0x46;
        }
        private void jixiezhua_youxuan_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x05;
            cf.control_data[4] = 0x5a;
        }
        private void jixiezhua_zuoxuan_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x05;
            cf.control_data[4] = 0x46;
        }
        private void wan_shang_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x04;
            cf.control_data[4] = 0x5a;
        }
        private void wan_xia_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x04;
            cf.control_data[4] = 0x46;
        }       
        private void xiaobi_s_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x03;
            cf.control_data[4] = 0x5a;
        }
        private void xiaobi_x_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x03;
            cf.control_data[4] = 0x46;
        }
        private void dabi_xia_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x02;
            cf.control_data[4] = 0x5a;
        }
        private void dabi_shang_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x02;
            cf.control_data[4] = 0x46;
        }
        private void baibi_shang_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x07;
            cf.control_data[4] = 0x5a;
        }
        private void baibi_xia_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x07;
            cf.control_data[4] = 0x46;
        }
        private void zuoxuan_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x01;
            cf.control_data[4] = 0x5a;
        }
        private void youxuan_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x02;
            cf.control_data[3] = 0x01;
            cf.control_data[4] = 0x46;
        }
        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            cf.control_data[0] = 0x00;
            cf.control_data[3] = 0x00;
            cf.control_data[4] = 0x73;
        }
        private void yuntai_shang(object sender, RoutedEventArgs e)
        {
            /*a += 20;
           int z = Math.Abs(a);
           float c = 0, d = 0, m = 0, f = 0;
           c = (z) & 0xff;
           d = (z >> 8) & 0xff;
           m = (z >> 16) & 0xff;
           f = (z >> 24) & 0xff;
           cf.control_data[8] = (byte)c;
           cf.control_data[9] = (byte)d;
           cf.control_data[10] = (byte)m;
           if (a < 0)
           {
               cf.control_data[11] = 0x01;
           }
           else
           {
               cf.control_data[11] = 0x00;
           }*/
            cf.control_data[8] = 0x01;
        }

        private void luzhi(object sender, RoutedEventArgs e)
        {
            cf.camera_data[0] = System.Text.Encoding.Default.GetBytes("s")[0];
            cf.autoEvent_pub_camera.Set();
        }

        private void tingzhi(object sender, RoutedEventArgs e)
        {
            cf.camera_data[0] = System.Text.Encoding.Default.GetBytes("e")[0];
            cf.autoEvent_pub_camera.Set();
        }

        private void zhuaquzhongwu(object sender, RoutedEventArgs e)
        {
            cf.control_data[0] = 0x01;
            cf.control_data[7] = System.Text.Encoding.Default.GetBytes("V")[0];
        }

        private void zhankai(object sender, RoutedEventArgs e)
        {
            cf.control_data[0] = 0x01;
            cf.control_data[7] = System.Text.Encoding.Default.GetBytes("W")[0];
        }

        private void yijianhuishou(object sender, RoutedEventArgs e)
        {
            cf.control_data[0] = 0x01;
            cf.control_data[7] = System.Text.Encoding.Default.GetBytes("X")[0];
        }

        private void panpataijie(object sender, RoutedEventArgs e)
        {
            cf.control_data[0] = 0x01;
            cf.control_data[7] = System.Text.Encoding.Default.GetBytes("Y")[0];
        }

        private void yuntai_xia(object sender, RoutedEventArgs e)
        {
            /*a = a - 20;
           int z = Math.Abs(a);
           float c = 0, d = 0, m = 0, f = 0;
           c = (z) & 0xff;
           d = (z >> 8) & 0xff;
           m = (z >> 16) & 0xff;
           f = (z >> 24) & 0xff;
           cf.control_data[8] = (byte)c;
           cf.control_data[9] = (byte)d;
           cf.control_data[10] = (byte)m;
           if (a < 0)
           {
               cf.control_data[11] = 0x01;
           }
           else
           {
               cf.control_data[11] = 0x00;
           }*/
            cf.control_data[8] = 0x02;
        }

        private void yuntai_youxuan(object sender, RoutedEventArgs e)
        {
            /*a = a - 20;
            int z = Math.Abs(a);
            float c = 0, d = 0, m = 0, f = 0;
            c = (z) & 0xff;
            d = (z >> 8) & 0xff;
            m = (z >> 16) & 0xff;
            f = (z >> 24) & 0xff;
            cf.control_data[12] = (byte)c;
            cf.control_data[13] = (byte)d;
            cf.control_data[14] = (byte)m;
            if (a < 0)
            {
                cf.control_data[15] = 0x01;
            }
            else
            {
                cf.control_data[15] = 0x00;
            }*/
            cf.control_data[9] = 0x02;
        }

        private void yuntai_zuoxuan(object sender, RoutedEventArgs e)
        {
            /*a = a + 20;
            int z = Math.Abs(a);
            float c = 0, d = 0, m = 0, f = 0;
            c = (z) & 0xff;
            d = (z >> 8) & 0xff;
            m = (z >> 16) & 0xff;
            f = (z >> 24) & 0xff;
            cf.control_data[12] = (byte)c;
            cf.control_data[13] = (byte)d;
            cf.control_data[14] = (byte)m;
            if (a < 0)
            {
                cf.control_data[15] = 0x01;
            }
            else
            {
                cf.control_data[15] = 0x00;
            }*/
            cf.control_data[9] = 0x01;
        }

        private void disu(object sender, RoutedEventArgs e)
        {
            cf.control_data[1] = 0x44;
        }

        private void zhongsu(object sender, RoutedEventArgs e)
        {
            cf.control_data[1] = 0x4d;
        }

        private void gaosu(object sender, RoutedEventArgs e)
        {
            cf.control_data[1] = 0x55;
        }
        int kg = 0;//灯的状态
        private void qianzhaoming(object sender, RoutedEventArgs e)
        {
            kg = 1;
            cf.control_data[5] = 0x01;
            cf.control_data[6] = 0x4e;
        }

        private void houzhaoming(object sender, RoutedEventArgs e)
        {
            kg = 1;
            cf.control_data[5] = 0x02;
            cf.control_data[6] = 0x4e;
        }

        private void jiaquzhaoming(object sender, RoutedEventArgs e)
        {
            kg = 1;
            cf.control_data[5] = 0x03;
            cf.control_data[6] = 0x4e;
        }

        private void quanjingzhaoming(object sender, RoutedEventArgs e)
        {
            kg = 1;
            cf.control_data[5] = 0x04;
            cf.control_data[6] = 0x4e;
        }
        private void kaiguan(object sender, RoutedEventArgs e)
        {
            kg++;
            if (kg == 2)
            {
                kg = 0;
            }
            if (kg==1)
            {
                cf.control_data[5] = 0xfe;
                cf.control_data[6] = 0x46;
            }
            else
            {
                cf.control_data[5] = 0xfe;
                cf.control_data[6] = 0x4e;
            }

        }

        private void yunsuqianjin(object sender, RoutedEventArgs e)
        {

        }

        private void shipinku(object sender, RoutedEventArgs e)
        {
            var localAvi = System.Configuration.ConfigurationManager.AppSettings["localAvi"];
            string path = localAvi;
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void Yuyin(object sender, RoutedEventArgs e)
        {
            cf.camera_data[0] = System.Text.Encoding.Default.GetBytes("y")[0];
            cf.autoEvent_pub_camera.Set();
        }

        private void zhuaqumoshi(object sender, RoutedEventArgs e)
        {
            w6.Show();
            this.Hide();
        }
        protected override void OnClosed(EventArgs e)
        {

            if(w6 != null)
            {
                w6.Close();
                w6 = null;
            }
            base.Close();
        }
        List<CompressedImageControl> imgList = new List<CompressedImageControl>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置全屏  
            this.Left = 0;
            this.Top = 0;
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            //   this.Topmost = true;

            imgList.Add(TestImage1);
            imgList.Add(TestImage2);
            imgList.Add(TestImage3);
            imgList.Add(TestImage4);

            for (int i = 0; i < imgList.Count; i++)
            {
                string key = "angel" + i.ToString();
                var angel = System.Configuration.ConfigurationManager.AppSettings[key];
                if (angel == "" || angel == "0") continue;
                try
                {
                    int nAngel = Convert.ToInt32(angel);
                    if (nAngel > 0)
                    {
                        TransformGroup tg = imgList[i].LayoutTransform as TransformGroup;

                        if (tg != null)
                        {
                            double h = imgList[i].Height;
                            double w = imgList[i].Width;

                            RotateTransform rotateTransform = new RotateTransform(nAngel);
                            tg.Children.Add(rotateTransform);
                            imgList[i].Tag = nAngel;
                            if (nAngel == 90 || nAngel == 270)
                            {
                                imgList[i].Width = h;
                                imgList[i].Height = w;                                 
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private void cunchu(object sender, RoutedEventArgs e)
        {
            cf.camera_data[0] = System.Text.Encoding.Default.GetBytes("d")[0];
            cf.autoEvent_pub_camera.Set();
        }
    }
}
