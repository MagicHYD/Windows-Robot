#region Imports

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Messages;
using Messages.custom_msgs;
using Ros_CSharp;
using ROS_ImageWPF;
using XmlRpc_Wrapper;
using Int32 = Messages.std_msgs.Int32;
using String = Messages.std_msgs.String;
using m = Messages.std_msgs;
using gm = Messages.geometry_msgs;
using nm = Messages.nav_msgs;
using sm = Messages.sensor_msgs;
using System.Text;

#endregion


namespace CompressedImageView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       

        public MainWindow()
        {
            InitializeComponent();
        }
        NodeHandle nh;

        VoicePlay vp = new VoicePlay();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] sParam = new string[1];
            //sParam[0] = "__hostname:=192.168.0.8";
            sParam[0] = "__master:=http://192.168.1.108:11311";

            ROS.Init(sParam, "Image_Test2");
            nh = new NodeHandle();


            VoicePlay.EasyPlayer_Init();
        }

        protected override void OnClosed(EventArgs e)
        {
            ROS.shutdown();
            vp.EasyPlayer_CloseStream();
            base.OnClosed(e);
        }

        private void flippydippy<T>(T img) where T : iROSImage
        {
            var i = img as iROSImage;
            if (i != null)
            {
                if (i.IsSubscribed())
                {
                    i.getGenericImage().fps.Content = "PAUSED";
                    i.Desubscribe();
                }
                else
                {
                    i.getGenericImage().fps.Content = "0";
                    i.Resubscribe();
                }
            }
            else 
                Console.WriteLine("TOO MANY ASSUMPTIONS!");
        }


        private void _1(object sender, RoutedEventArgs e)
        {
            flippydippy(TestImage1);
        }
        private void _2(object sender, RoutedEventArgs e)
        {
          //  flippydippy(TestImage2);
        }
        private void _3(object sender, RoutedEventArgs e)
        {
         //   flippydippy(TestImage3);
        }
        private void _4(object sender, RoutedEventArgs e)
        {
          //  flippydippy(TestImage4);
        }
        private void _5(object sender, RoutedEventArgs e)
        {
           // flippydippy(TestImage5);
        }
        private void _6(object sender, RoutedEventArgs e)
        {
            vp.EasyPlayer_OpenStream("rtmp://192.168.1.108:2017/live/test");
        }

        public void _click_001(object sender, RoutedEventArgs e)
        {

            TestImage1.Width = this.Width;
            TestImage1.Height = this.Height;

            TestImage2.Visibility = System.Windows.Visibility.Hidden;
            TestImage3.Visibility = System.Windows.Visibility.Hidden;
            TestImage4.Visibility = System.Windows.Visibility.Hidden;
            TestImage5.Visibility = System.Windows.Visibility.Hidden;
            TestImage6.Visibility = System.Windows.Visibility.Hidden;
            bt1.Visibility = System.Windows.Visibility.Hidden;
            bt2.Visibility = System.Windows.Visibility.Hidden;
            bt3.Visibility = System.Windows.Visibility.Hidden;
            bt4.Visibility = System.Windows.Visibility.Hidden;
            bt5.Visibility = System.Windows.Visibility.Hidden;
            bt6.Visibility = System.Windows.Visibility.Hidden;            
        }
        private void _7(object sender, RoutedEventArgs e)
        {
            vp.EasyPlayer_OpenStream("rtmp://192.168.1.108:2017/live/test");
           
        }

        private void TestImage6_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TestImage1_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
