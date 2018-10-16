﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Messages;
using Messages.std_msgs;
using Ros_CSharp;
using tf.net;

namespace tf_example
{
    class Program
    {
        private static Transformer tfer;

        private static emTransform testLookup(emTransform intendedResult)
        {
            if (!tfer.waitForTransform(intendedResult.frame_id, intendedResult.child_frame_id, intendedResult.stamp, new Duration(new TimeData(1, 0)), null))
                return null;
            emTransform ret = new emTransform();
            if (tfer.lookupTransform(intendedResult.frame_id, intendedResult.child_frame_id, intendedResult.stamp, out ret))
            {
                Console.WriteLine("***  " + intendedResult.frame_id + " ==> " + intendedResult.child_frame_id + " ***");
                Console.WriteLine("********************** IDEAL *********************");
                Console.WriteLine(intendedResult);
                Console.WriteLine("********************** ACTUAL ********************");
                Console.WriteLine(ret);
                Console.WriteLine("***************************************************\n\n");
                return ret;
            }
            return null;
        }

        static void Main(string[] args)
        {
            ROS.Init(args, "tf_example");

            NodeHandle nh = new NodeHandle();

            ROS.Info("This node will create a Transformer to compare lookup results between four source/target frame pairs of an OpenNI2 node's transform tree with ones observed in linux with tf_echo");

            string[] nodes = null;
            while (ROS.ok && !ROS.shutting_down && (!master.getNodes(ref nodes) || !nodes.Contains("/camera/driver")))
            {
                ROS.Error("For this to work, you need to \"roslaunch openni2_launch openni2.launch\" on a PC with an ASUS Xtion or PrimeSense Carmine sensor plugged into it, and connect to the same master");
                Thread.Sleep(2000);
            }
            if (ROS.ok && !ROS.shutting_down)
            {
                tfer = new Transformer(false);

                #region tf_echo results

                /*
             * tf_echo camera_link camera_rgb_frame
             *      (0.0,-0.045,0.0)
             *      (0,0,0,1)
             */
                emTransform result1 = new emTransform() {basis = new emQuaternion(0, 0, 0, 1), origin = new emVector3(0, -0.045, 0), child_frame_id = "camera_rgb_frame", frame_id = "camera_link"};

                /*
             * tf_echo camera_link camera_rgb_optical_frame
             *      (0.0,-0.045,0.0)
             *      (-0.5,0.5,-0.5,0.5)
             */
                emTransform result2 = new emTransform() {basis = new emQuaternion(-0.5, 0.5, -0.5, 0.5), origin = new emVector3(0.0, -0.045, 0.0), child_frame_id = "camera_rgb_optical_frame", frame_id = "camera_link"};

                /*
             * tf_echo camera_rgb_frame camera_depth_frame
             *      (0.0,0.25,0.0)
             *      (0,0,0,1)
             */
                emTransform result3 = new emTransform() {basis = new emQuaternion(0, 0, 0, 1), origin = new emVector3(0.0, 0.025, 0.0), child_frame_id = "camera_depth_frame", frame_id = "camera_rgb_frame"};

                /*
             * tf_echo camera_rgb_optical_frame camera_depth_frame
             *      (-0.25,0.0,0.0)
             *      (0.5,-0.5,0.5,0.5)
             */
                emTransform result4 = new emTransform() {basis = new emQuaternion(0.5, -0.5, 0.5, 0.5), origin = new emVector3(-0.025, 0.0, 0.0), child_frame_id = "camera_depth_frame", frame_id = "camera_rgb_optical_frame"};

                #endregion

                emTransform test1 = null, test2 = null, test3 = null, test4 = null;
                do
                {
                    if (test1 == null || !string.Equals(result1.ToString(), test1.ToString()))
                        test1 = testLookup(result1);
                    if (!ROS.ok || ROS.shutting_down)
                        break;
                    if (test2 == null || !string.Equals(result2.ToString(), test2.ToString()))
                        test2 = testLookup(result2);
                    if (!ROS.ok || ROS.shutting_down)
                        break;
                    if (test3 == null || !string.Equals(result3.ToString(), test3.ToString()))
                        test3 = testLookup(result3);
                    if (!ROS.ok || ROS.shutting_down)
                        break;
                    if (test4 == null || !string.Equals(result4.ToString(), test4.ToString()))
                        test4 = testLookup(result4);
                    Thread.Sleep(100);
                } while (ROS.ok && !ROS.shutting_down && (test1 == null || !string.Equals(result1.ToString(), test1.ToString()) ||
                                                          test2 == null || !string.Equals(result2.ToString(), test2.ToString()) ||
                                                          test3 == null || !string.Equals(result3.ToString(), test3.ToString()) ||
                                                          test4 == null || !string.Equals(result4.ToString(), test4.ToString())));
            }
            if (ROS.ok && !ROS.shutting_down)
            {
                Console.WriteLine("\n\n\nALL TFs MATCH!\n\nPress enter to quit");
                Console.ReadLine();
                ROS.shutdown();
            }
        }
    }
}
