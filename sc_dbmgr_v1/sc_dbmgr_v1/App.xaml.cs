using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using log4net;
using log4net.Config;
using System.IO;


namespace sc_dbmgr_v1
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private ILog log;
        private void InitLog()
        {
            var logfile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log4net.config");
            XmlConfigurator.ConfigureAndWatch(logfile);
            log = LogManager.GetLogger("log");
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            log.Error(e.Exception);
            MessageBox.Show(e.Exception.ToString(), "App error occurred.");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitLog();
        }
    }
}
