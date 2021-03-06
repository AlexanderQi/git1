﻿using System;
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
using log4net;
using log4net.Config;
using System.IO;
namespace sc_dbmgr_v1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ILog log; //log配置初始化在APP.XAML.CS中
        public MainWindow()
        {
            
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            log = LogManager.GetLogger("log");
            log.Info("StartPath:"+AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
        }

        


        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
