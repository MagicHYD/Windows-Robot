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
using ROS_ImageWPF;
using Ros_CSharp;
using XmlRpc_Wrapper;

using System.Threading;

using System.Net.Sockets;
using System.Net; 

namespace EOD_robot
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
        }
        CommFunction cf = new CommFunction();
        int a =5000;

        List<CompressedImageControl> imgList = new List<CompressedImageControl>();
        
         

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*  string[] sParam = new string[1];
              //sParam[0] = "__hostname:=192.168.0.8";
              sParam[0] = "__master:=http://" + host + ":11311";

              ROS.Init(sParam, "Image_Test2");
              nh = new NodeHandle();


              VoicePlay.EasyPlayer_Init();

              vp.host = host;
              vp.micro = "麦克风 (Realtek High Definition Au";
              vp.EasyPlayer_OpenStream("rtmp://" + host+ ":2017/live/test");
             * */

         

            img_background.Visibility = System.Windows.Visibility.Hidden;
            grid_button.Visibility = System.Windows.Visibility.Hidden;
            btnPlayVoide.Visibility = System.Windows.Visibility.Hidden;


            cf.receive = ReceiveFromRosDelegate;
            cf.keypress = KeyPressDelegate;
            cf.Load(this);

            
             
            imgList.Add(TestImage1);
            imgList.Add(TestImage2);
            imgList.Add(TestImage3);
            imgList.Add(TestImage4);
            imgList.Add(TestImage5);
            imgList.Add(TestImage6);

            for (int i = 0;i< imgList.Count; i++)
            {
                string key = "angel" + i.ToString();
                var angel = System.Configuration.ConfigurationManager.AppSettings[key];
                if (angel == "" || angel == "0") continue;
                try
                {
                    int nAngel = Convert.ToInt32(angel);
                    if (nAngel > 0)
                    {
                        double h = imgList[i].Height;
                        double w = imgList[i].Width;
                        TransformGroup tg = imgList[i].LayoutTransform as TransformGroup;

                        if (tg != null)
                        {
                            RotateTransform rotateTransform = new RotateTransform(nAngel);
                            tg.Children.Add(rotateTransform);
                            imgList[i].Tag = nAngel;                            
                        }
                    }
                }
                catch
                {

                }
            }



            adjustPos(Visibility.Visible, null);
        }

        void ReceiveFromRosDelegate(byte[] data)
        {
            int i = data[5];
            this.Title = i.ToString();
        }

        //按键控制
        void KeyPressDelegate(int index, bool[] pre, bool[] cur, int x, int y,int xpre, int ypre)
        {
            switch(index)
            {
                //if (index == 0)//手柄1
                case 0:
                
                    if (y == 0)
                    {
                        if (y == ypre)
                        {
                            return;
                        }
                        if (y == 0)
                        {

                            if (cf.imageCount == 6)
                            {
                                cf.imageCount = 4;
                            }
                            else
                            {
                                cf.imageCount = 6;
                            }
                            App.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                adjustPos(Visibility.Visible, null);

                            }), new TimeSpan(0, 0, 1));

                        }
                        else
                        {

                        }

                        Thread.Sleep(10);
                    }

                    else if (y == 65535)
                    {
                        cf.control_data[5] = 0xfe;
                        cf.control_data[6] = 0x46;
                    }
                    else if (x == 0)
                    {
                        cf.control_data[5] = 0xfe;
                        cf.control_data[6] = 0x4e;
                    }
                    else if (x == 65535)
                    {

                    }
                    else if (cur[0])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x06;
                        cf.control_data[4] = 0x5a;
                    }
                    else if (cur[1])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x06;
                        cf.control_data[4] = 0x46;
                    }
                    else if (cur[2])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x05;
                        cf.control_data[4] = 0x5a;
                    }
                    else if (cur[3])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x05;
                        cf.control_data[4] = 0x46;
                    }
                    else if (cur[4])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x04;
                        cf.control_data[4] = 0x5a;
                    }
                    else if (cur[5])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x04;
                        cf.control_data[4] = 0x46;
                    }
                    else if (cur[6])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x03;
                        cf.control_data[4] = 0x5a;
                    }
                    else if (cur[7])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x03;
                        cf.control_data[4] = 0x46;
                    }
                    else if (cur[8])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x02;
                        cf.control_data[4] = 0x5a;
                    }
                    else if (cur[9])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x02;
                        cf.control_data[4] = 0x46;
                    }
                    else if (cur[10])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x07;
                        cf.control_data[4] = 0x46;
                    }
                    else if (cur[11])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x07;
                        cf.control_data[4] = 0x5a;
                    }
                    else
                    {
                        Thread.Sleep(20);
                        cf.control_data[0] = 0x00;
                        cf.control_data[3] = 0x00;
                        cf.control_data[4] = 0x73;
                       
                    }
                    break;



                //if (index == 1)//手柄2
                case 1:
                
                    if (y == 0)
                    {
                        cf.control_data[5] = 0x01;
                        cf.control_data[6] = 0x4e;
                    }

                    else if (y == 65535)
                    {
                        cf.control_data[5] = 0x02;
                        cf.control_data[6] = 0x4e;
                    }
                    else if (x == 0)
                    {
                        cf.control_data[5] = 0x03;
                        cf.control_data[6] = 0x4e;
                    }
                    else if (x == 65535)
                    {
                        cf.control_data[5] = 0x04;
                        cf.control_data[6] = 0x4e;
                    }
                    else if (cur[0])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x01;
                        cf.control_data[4] = 0x5a;
                    }
                    else if (cur[1])
                    {
                        cf.control_data[0] = 0x02;
                        cf.control_data[3] = 0x01;
                        cf.control_data[4] = 0x46;
                    }
                    else if (cur[2] || cur[3])
                    {
                        if (cur[2])
                        {
                            cf.camera_data[0] = System.Text.Encoding.Default.GetBytes("s")[0];
                        }
                        if (cur[3])
                        {
                            cf.camera_data[0] = System.Text.Encoding.Default.GetBytes("e")[0];

                        }
                        cf.autoEvent_pub_camera.Set();

                    }
                    else if (cur[4] || cur[5] || cur[6] || cur[7])
                    {
                        if (cur[4])
                        {
                            cf.control_data[0] = 0x01;
                            cf.control_data[7] = System.Text.Encoding.Default.GetBytes("V")[0];
                        }
                        else if (cur[5])
                        {
                            cf.control_data[0] = 0x01;
                            cf.control_data[7] = System.Text.Encoding.Default.GetBytes("W")[0];

                        }
                        else if (cur[6])
                        {
                            cf.control_data[0] = 0x01;
                            cf.control_data[7] = System.Text.Encoding.Default.GetBytes("X")[0];

                        }
                        else if (cur[7])
                        {
                            cf.control_data[0] = 0x01;
                            cf.control_data[7] = System.Text.Encoding.Default.GetBytes("Y")[0];

                        }
                    }
                    else if (cur[8])
                    {
                        /*if (cur[8])
                        {
                            a = a + 20;
                        }
                        int z = Math.Abs(a);
                        float c = 0, d = 0, e = 0, f = 0;
                        c = (z) & 0xff;
                        d = (z >> 8) & 0xff;
                        e = (z >> 16) & 0xff;
                        f = (z >> 24) & 0xff;
                        cf.control_data[8] = (byte)c;
                        cf.control_data[9] = (byte)d;
                        cf.control_data[10] = (byte)e;
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
                    else if (cur[9])
                    {
                        /*if (cur[9])
                        {
                            a = a - 20;
                        }
                        int z = Math.Abs(a);
                        float c = 0, d = 0, e = 0, f = 0;
                        c = (z) & 0xff;
                        d = (z >> 8) & 0xff;
                        e = (z >> 16) & 0xff;
                        f = (z >> 24) & 0xff;
                        cf.control_data[8] = (byte)c;
                        cf.control_data[9] = (byte)d;
                        cf.control_data[10] = (byte)e;
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
                    else if (cur[10])
                    {
                        /*if (cur[10])
                        {
                            a = a + 20;
                        }
                        int z = Math.Abs(a);
                        float c = 0, d = 0, e = 0, f = 0;
                        c = (z) & 0xff;
                        d = (z >> 8) & 0xff;
                        e = (z >> 16) & 0xff;
                        f = (z >> 24) & 0xff;
                        cf.control_data[12] = (byte)c;
                        cf.control_data[13] = (byte)d;
                        cf.control_data[14] = (byte)e;
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
                    else if (cur[11])
                    {
                        /*if (cur[11])
                        {
                            a = a - 20;
                        }
                        int z = Math.Abs(a);
                        float c = 0, d = 0, e = 0, f = 0;
                        c = (z) & 0xff;
                        d = (z >> 8) & 0xff;
                        e = (z >> 16) & 0xff;
                        f = (z >> 24) & 0xff;
                        cf.control_data[12] = (byte)c;
                        cf.control_data[13] = (byte)d;
                        cf.control_data[14] = (byte)e;
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
                    else if (cur[12])
                    {
                        cf.control_data[1] = 0x44;
                    }
                    else if (cur[13])
                    {
                        cf.control_data[1] = 0x4d;
                    }
                    else if (cur[14])
                    {
                        cf.control_data[1] = 0x55;
                    }
                    else if (cur[15])
                    {
                        cf.control_data[2] = 0x46;
                    }
                    else if (cur[16])
                    {
                        cf.control_data[2] = 0x4c;
                    }
                    else if (cur[17])
                    {
                        cf.control_data[2] = 0x42;
                    }
                    else if (cur[18])
                    {
                        cf.control_data[2] = 0x52;
                    }
                    else if (cur[19])
                    {

                    }
                    else
                    {
                        Thread.Sleep(20);
                        cf.control_data[0] = 0x00;
                        cf.control_data[2] = 0x00;
                        cf.control_data[3] = 0x00;
                        cf.control_data[4] = 0x73;
                        cf.control_data[8] = 0x00;
                        cf.control_data[9] = 0x00;
                        
                    }
                    break;
            }

        }

    
        protected override void OnClosed(EventArgs e)
        {

            cf.OnClosed();
            base.OnClosed(e);
        }

        void adjustPos(Visibility Visible, CompressedImageControl img)
        {
            double left = 0;
            double top = 0;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;

            if (cf.imageCount == 6)
            {
                width -= 5 * 2; // 6个视频
            }
            else
            {
                width -= 5; // 4个视频
            }
            height -= 5;


            for (int i = 0; i < imgList.Count && i < cf.imageCount; i++)
            {
                if (imgList[i] != img)
                {
                    imgList[i].Visibility = Visible;
                }
                if (Visible == System.Windows.Visibility.Visible)
                {

                    double img_w;
                    double img_h;

                    img_w = width / (cf.imageCount / 2);
                    img_h = height / 2;

                    imgList[i].Margin = new Thickness(left, top, 0, 0);
                    if (imgList[i].Tag != null && ((int)imgList[i].Tag == 90 || (int)imgList[i].Tag == 270))
                    {
                        imgList[i].Width = img_h;
                        imgList[i].Height = img_w;
                    }
                    else
                    {
                        imgList[i].Width = img_w;
                        imgList[i].Height = img_h;
                    }
                    left += img_w + 5;

                    if (i == (cf.imageCount / 2 - 1))
                    {
                        top = height / 2 + 5;
                        left = 0;
                    }
                }

            }
            for (int i = cf.imageCount; i < imgList.Count; i++)
            {
                imgList[i].Visibility = Visibility.Hidden;
            }
        }
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
            if ((int)img.Width != (int)width)
            {

                adjustPos(Visibility.Hidden, img);
                img.Width = width;
                img.Height = height;
            }
            else
            {
                adjustPos(Visibility.Visible, img);
            }
        }

 
    }
}
