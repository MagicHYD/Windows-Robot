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
using System.Net.Sockets;
using System.Net; 

namespace SimpleSubscriber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Subscriber<Messages.std_msgs.String> sub;
        NodeHandle nh;
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取本机IP出错:" + ex.Message);
                return "";
            }
        }
        public MainWindow()
        {

            string[] sParam = new string[2];
            sParam[0] = "__hostname:=" + GetLocalIP();
            sParam[1] = "__master:=http://192.168.1.108:11311";
            ROS.Init(sParam, "wpf_listener3");

            InitializeComponent();


            nh = new NodeHandle();

            sub = nh.subscribe<Messages.std_msgs.String>("/chatter", 10, subCallback);
        }

        public void subCallback(Messages.std_msgs.String msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                l.Content = "Receieved:\n" + msg.data;
            }), new TimeSpan(0,0,1));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ROS.shutdown();
            ROS.waitForShutdown();
            base.OnClosing(e);
        }
    }
}
