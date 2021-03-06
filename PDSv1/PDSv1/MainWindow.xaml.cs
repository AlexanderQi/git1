﻿using System;
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
using log4net;
using log4net.Config;
using System.IO;

namespace PDSv1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ILog log;
        private void InitLog()
        {
            var logfile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log4net.config");
            XmlConfigurator.ConfigureAndWatch(logfile);
            log = LogManager.GetLogger(AppDomain.CurrentDomain.BaseDirectory);
        }
        public MainWindow()
        {
            InitLog();
            InitializeComponent();
            log.Info("Program Start");
        }

        private void r1_MouseEnter(object sender, MouseEventArgs e)
        {

            if (e.Source.GetType() == typeof(ListBoxItem))
            {
                pop1.IsOpen = false;
                pop1.IsOpen = true;
                pcaption.Text = e.Source.ToString() + " >>> " + e.OriginalSource.ToString();
                
            }

            c0.Width = new GridLength(263f, GridUnitType.Star);
            log.Info("Mouse Enter");
        }

        private void r1_MouseLeave(object sender, MouseEventArgs e)
        {
            if (pop1.IsOpen)
            {
                e.Handled = true;
            }
            else
            {
                c0.Width = new GridLength(10f, GridUnitType.Star);
            }
            log.Info("Mouse Leave");
        }
    }
}
