using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using ROS_ImageWPF;
using Ros_CSharp;
using XmlRpc_Wrapper;

using System.Threading;

using System.Net.Sockets;
using System.Net;


namespace EOD_robot
{
    public partial class CommFunction
    {

        string host;
        string micro;
        public int imageCount = 6;
        public bool closing;
        public byte[] camera_data = new byte[1];
        //机器人相关控制
        public byte[] control_data = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x73, 0x00, 0x46, 0x51, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


        NodeHandle nh = null;

        VoicePlay vp = new VoicePlay();


        Window window = null;

        SharpDX.DirectInput.Joystick[] curJoystick = null;
        bool[] joy_button1 = new bool[256];

        bool[] joy_button_pre1 = new bool[256];

        int x1, x2, y1, y2;
        int xpre1, xpre2, ypre1, ypre2;

        bool[] joy_button2 = new bool[256];

        bool[] joy_button_pre2 = new bool[256];




        public delegate void ReceiveFromRosDelegate(byte[] data);
        public ReceiveFromRosDelegate receive = null;



        public delegate void KeyPressDelegate(int index, bool[] pre, bool[] cur, int x, int y, int xpre, int ypre);
        public KeyPressDelegate keypress = null;


        public CommFunction()
        {

            var reomoteIp = System.Configuration.ConfigurationManager.AppSettings["reomoteIp"];

            if (reomoteIp != null && reomoteIp.ToString().Length > 0)
            {
                host = reomoteIp.ToString();
            }
            var microIp = System.Configuration.ConfigurationManager.AppSettings["microIp"];

            if (microIp != null && microIp.ToString().Length > 0)
            {
                micro = microIp.ToString();
            }

        }
        public void Load(Window window)
        {

            this.window = window;
            Thread t1 = new Thread(loadThread);
            t1.IsBackground = true;
            t1.Start();


            // 设置全屏  
            window.Left = 0;
            window.Top = 0;
            window.WindowState = System.Windows.WindowState.Normal;
            window.WindowStyle = System.Windows.WindowStyle.None;
            window.ResizeMode = System.Windows.ResizeMode.NoResize;
            //   this.Topmost = true;


        }

        public void OnClosed()
        {
            closing = true;
            ROS.shutdown();
            autoEvent_pub_camera.Set();
            autoEvent_pub_control.Set();
            autoEvent_ui.Set();
            vp.EasyPlayer_CloseStream();
        }

        private void TJsListening1()
        {
            try
            {
                while (!closing)
                {
                    //初始化新按键
                    for (int i = 0; i < joy_button1.Length; i++)
                    {
                        joy_button_pre1[i] = joy_button1[i];
                        joy_button1[i] = false;
                    }
                    xpre1 = x1;
                    x1 = 32511;
                    ypre1 = y1;
                    y1 = 32511;

                    //如果手柄掉了，会引发异常，结束该线程
                    var CurJoyState = curJoystick[0].GetCurrentState();

                    //获取按键信息
                    //获取的按键名比厂家软件显示的小1
                    int ButtonsLen = CurJoyState.Buttons.Length;
                    x1 = CurJoyState.X;
                    y1 = CurJoyState.Y;

                    if (ButtonsLen > joy_button1.Length)
                    {
                        Array.Resize(ref joy_button1, ButtonsLen);
                    }
                    // bool bHasChange = false;
                    for (int j = 0; j < ButtonsLen; j++)
                    {
                        if (Convert.ToInt32(CurJoyState.Buttons[j]) > 0)
                        {
                            joy_button1[j] = true;//按下
                        }
                        /*  if(joy_button_pre1[j] != joy_button1[j])
                          {
                              bHasChange = true;
                          }*/
                    }
                    // if(bHasChange)
                    {
                        keypress(0, joy_button_pre1, joy_button1, x1, y1, xpre1, ypre1);
                    }
                }
            }
            catch
            {
                curJoystick[0] = null;
            }
        }
        private void TJsListening2()
        {
            try
            {
                while (!closing)
                {
                    //初始化新按键
                    for (int i = 0; i < joy_button2.Length; i++)
                    {
                        joy_button_pre2[i] = joy_button2[i];
                        joy_button2[i] = false;
                    }
                    xpre2 = x2;
                    x2 = 32767;
                    ypre2 = y2;
                    y2 = 32767;
                    //如果手柄掉了，会引发异常，结束该线程
                    var CurJoyState = curJoystick[1].GetCurrentState();

                    //获取按键信息
                    //获取的按键名比厂家软件显示的小1
                    int ButtonsLen = CurJoyState.Buttons.Length;
                    x2 = CurJoyState.X;
                    y2 = CurJoyState.Y;
                    if (ButtonsLen > joy_button2.Length)
                    {
                        Array.Resize(ref joy_button2, ButtonsLen);
                    }
                    // bool bHasChange = false;
                    for (int j = 0; j < ButtonsLen; j++)
                    {
                        if (Convert.ToInt32(CurJoyState.Buttons[j]) > 0)
                        {
                            joy_button2[j] = true;//按下
                        }
                        //  if (joy_button_pre2[j] != joy_button2[j])
                        {
                            //     bHasChange = true;
                        }
                    }
                    //if (bHasChange)
                    {
                        keypress(1, joy_button_pre2, joy_button2, x2, y2, xpre2, ypre2);
                    }


                }
            }
            catch
            {
                curJoystick[1] = null;
            }
        }

        //相机相关控制

        Publisher<Messages.std_msgs.UInt8MultiArray> pub_camera;
        public AutoResetEvent autoEvent_pub_camera = new AutoResetEvent(false);


        public AutoResetEvent autoEvent_ui = new AutoResetEvent(false);


        public AutoResetEvent autoEvent_pub_control = new AutoResetEvent(false);
        Publisher<Messages.std_msgs.UInt8MultiArray> pub_control;
        Subscriber<Messages.std_msgs.UInt8MultiArray> sub_control;

        Thread pubthreadCamara = null;
        void publishCameraThread()
        {
            if (pubthreadCamara != null)
            {
                pubthreadCamara.Abort();
            }
            pubthreadCamara = new Thread(() =>
            {

                Thread download = null;
                while (ROS.ok && !closing)
                {
                    Messages.std_msgs.UInt8MultiArray msg = new Messages.std_msgs.UInt8MultiArray();
                    msg.data = camera_data;

                    pub_camera.publish(msg);

                

                    if (camera_data[0] == System.Text.Encoding.Default.GetBytes("d")[0])
                    {
                        if (download != null)
                        {
                            download.Abort();
                        }
                        download = new Thread(() =>
                        {

                            //Thread.Sleep(200);
                            string date = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
                            var remoteAvi = System.Configuration.ConfigurationManager.AppSettings["remoteAvi"];
                            var localAvi = System.Configuration.ConfigurationManager.AppSettings["localAvi"];
                            for (int i = 1; i < 7; i++)
                            {
                                string errorinfo;
                                string localfile = localAvi + date + "_" + i + ".avi";
                                string filename = remoteAvi + "camera" + i + ".avi";
                                SFTPOperation ftp = new SFTPOperation(host, "22", "fanfan", "fanfan");
                                ftp.Get(filename, localfile);
                            }
                        });
                        download.Start();

                    }
                    //for (int i = 0; i < camera_data.Length; i++)
                    //{
                    //    camera_data[i] = 0;
                    //}

                    autoEvent_pub_camera.WaitOne();
                }
            });
            pubthreadCamara.Start();

        }

        Thread pubthread = null;
        void publishControlThread()
        {

            if (pubthread != null)
            {
                pubthread.Abort();
            }
            pubthread = new Thread(() =>
            {

                while (ROS.ok && !closing)
                {

                    Messages.std_msgs.UInt8MultiArray msg;
                    msg = new Messages.std_msgs.UInt8MultiArray();
                    msg.data = control_data;
                    pub_control.publish(msg);
                    Thread.Sleep(100);
                    //autoEvent_pub_control.WaitOne();

                }
            });
            pubthread.Start();

        }

        public void subCallback(Messages.std_msgs.UInt8MultiArray msg)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                //msg.data[0]
                if (receive != null)
                {
                    receive(msg.data);
                }
            }), new TimeSpan(0, 0, 1));
        }
        private void loadThread()
        {
            while (!closing)
            {
                if (!ROS.ok)
                {
                    string[] sParam = new string[2];
                    sParam[0] = "__hostname:=" + GetLocalIP();
                    //sParam[0] = "__hostname:=192.168.0.30";
                    sParam[1] = "__master:=http://" + host + ":11311";

                    ROS.Init(sParam, "Image_Test2");


                    nh = new NodeHandle();

                    VoicePlay.EasyPlayer_Init();

                    vp.host = host;
                    vp.micro = micro;

                    vp.EasyPlayer_OpenStream("rtmp://" + host + ":2017/live/test");

                    pub_camera = nh.advertise<Messages.std_msgs.UInt8MultiArray>("/EOD_camera", 1, true);

                    pub_control = nh.advertise<Messages.std_msgs.UInt8MultiArray>("/platform_msg", 1, true);
                    // sub_control = nh.subscribe<Messages.std_msgs.UInt8MultiArray>("/robot_state", 1, subCallback);
                    //sub_control = nh.subscribe<Messages.std_msgs.UInt8MultiArray>("/platform_msg", 1, subCallback);


                    publishCameraThread();
                    publishControlThread();
                }
                if (this.keypress != null)
                {
                    loadJoy();
                }
                Thread.Sleep(100);

            }

        }

        Thread t1 = null;
        Thread t2 = null;
        private void loadJoy()
        {
            var Joystick1 = System.Configuration.ConfigurationManager.AppSettings["Joystick1"];
            var Joystick2 = System.Configuration.ConfigurationManager.AppSettings["Joystick2"];
            string joy1 = "";
            if (Joystick1 != null)
            {
                joy1 = Joystick1.ToString();
            }
            string joy2 = "";
            if (Joystick2 != null)
            {
                joy2 = Joystick2.ToString();
            }

            int JoystickCount = 0;
            var dirInput = new SharpDX.DirectInput.DirectInput();
            if (curJoystick == null)
            {
                curJoystick = new SharpDX.DirectInput.Joystick[2];
                curJoystick[0] = null;
                curJoystick[1] = null;
            }
            bool bNeadJoy = false;
            if (curJoystick[0] == null && joy1 != "")
            {
                bNeadJoy = true;
            }

            if (curJoystick[1] == null && joy2 != "")
            {
                bNeadJoy = true;
            }
            if (!bNeadJoy)
            {
                return;
            }
            var allDevices = dirInput.GetDevices();

            foreach (var item in allDevices)
            {
                if (SharpDX.DirectInput.DeviceType.Joystick == item.Type)
                {
                    //记录新建线程的手柄名称 
                    if (item.ProductName == joy1)
                    {
                        curJoystick[0] = new SharpDX.DirectInput.Joystick(dirInput, item.InstanceGuid);
                        curJoystick[0].Properties.AxisMode = SharpDX.DirectInput.DeviceAxisMode.Absolute;
                        curJoystick[0].Acquire();
                        JoystickCount++;
                        if (t1 != null)
                        {
                            t1.Abort();
                        }
                        t1 = new Thread(TJsListening1);
                        t1.IsBackground = true;
                        t1.Start();
                    }
                    else if (item.ProductName == joy2)
                    {
                        curJoystick[1] = new SharpDX.DirectInput.Joystick(dirInput, item.InstanceGuid);
                        curJoystick[1].Properties.AxisMode = SharpDX.DirectInput.DeviceAxisMode.Absolute;
                        curJoystick[1].Acquire();
                        JoystickCount++;
                        if (t2 != null)
                        {
                            t2.Abort();
                        }
                        t2 = new Thread(TJsListening2);
                        t2.IsBackground = true;
                        t2.Start();
                    }

                    //curJoystick.Unacquire();//释放手柄
                }
            }
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


        public static string GetLocalIP()
        {
            try
            {
                var localIp = System.Configuration.ConfigurationManager.AppSettings["localIp"];

                if (localIp != null && localIp.ToString().Length > 0)
                {
                    return localIp.ToString();
                }
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
                return "";
            }
        }
    }
}
