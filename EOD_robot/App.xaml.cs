﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EOD_robot
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var mode = System.Configuration.ConfigurationManager.AppSettings["mode"];

            if(mode == "touch")
            {

                StartupUri = new Uri("Window6.xaml", UriKind.Relative);
            }
            else
            {

                StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            }
        }
    }
}
