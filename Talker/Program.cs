﻿#region Imports

using System;
using System.IO;
using System.Threading;
using Messages;
using Messages.custom_msgs;
using Ros_CSharp;
using XmlRpc_Wrapper;
using Int32 = Messages.std_msgs.Int32;
using String = Messages.std_msgs.String;
using m = Messages.std_msgs;
using gm = Messages.geometry_msgs;
using nm = Messages.nav_msgs;
using sm = Messages.sensor_msgs;
using System.Text;

#endregion

namespace Talker
{
    public class Program
    {
        private static void Main(string[] args)
        {
            ROS.Init(args, "Talker");
            NodeHandle node = new NodeHandle();
            Publisher<m.String> Talker = node.advertise<m.String>("/chatter", 1);
            int count = 0;
            
            while (ROS.ok)
            {
                ROS.Info("Publishing a chatter message:    \"Blah blah blah " + count + "\"");
                String pow = new String("Blah blah blah " + (count++));

                Talker.publish(pow);
                Thread.Sleep(1000);
            }
            
            ROS.shutdown();
            ROS.waitForShutdown();
        }
    }
}