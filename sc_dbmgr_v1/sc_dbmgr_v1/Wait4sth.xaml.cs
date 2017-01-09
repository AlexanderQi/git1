using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sc_dbmgr_v1
{
    /// <summary>
    /// Wait4sth.xaml 的交互逻辑
    /// </summary>
    public partial class Wait4sth : Window
    {
        
         private Wait4sth()
        {
            InitializeComponent();

        }

        private static Wait4sth instance = null;
        public static void showing()
        {
            if (instance == null)
                instance = new Wait4sth();
            Application.Current.MainWindow.IsEnabled = false;
            instance.Show();
        }


        static Action act = null;
        public static void stopshowing()
        {
            act = new Action(close_);
            Application.Current.Dispatcher.Invoke(act);
            
           
        }

        private static void close_()
        {
            Application.Current.MainWindow.IsEnabled = true;
            instance.Close();
            instance = null;
        }
    }
}
