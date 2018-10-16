using System;
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

using Ros_CSharp;
using XmlRpc_Wrapper;
using Messages;
using System.Threading;

namespace SimplePublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Publisher<Messages.std_msgs.String> pub;
        NodeHandle nh;

        private bool closing;
        private Thread pubthread;

        public MainWindow()
        {
            InitializeComponent();
            string[] sParam = new string[1];
            //sParam[0] = "__hostname:=192.168.0.8";
            sParam[0] = "__master:=http://192.168.1.108:11311";

            ROS.Init(sParam, "wpf_talker");
            nh = new NodeHandle();

            pub = nh.advertise<Messages.std_msgs.String>("/chatter", 1, true);

            pubthread = new Thread(() =>
            {
                int i = 0;
                Messages.std_msgs.String msg;
                while (ROS.ok && !closing)
                {
                    msg = new Messages.std_msgs.String("foo " + (i++));        
                    pub.publish(msg); 
                    Dispatcher.Invoke(new Action(() =>
                    {
                        l.Content = "Sending: " + msg.data;
                    }),new TimeSpan(0,0,1));
                    Thread.Sleep(100);
                }
            });
            pubthread.Start();
        }

        protected override void  OnClosed(EventArgs e)
        {
            if (!closing)
            {
                closing = true;
                pubthread.Join();
            }
            ROS.shutdown();
            ROS.waitForShutdown();
            base.OnClosed(e);
        }
    }
}
