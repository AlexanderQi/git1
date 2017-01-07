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

namespace sc_dbmgr_v1
{
    /// <summary>
    /// uiDbbackup.xaml 的交互逻辑
    /// </summary>
    public partial class uiDbbackup : UserControl
    {
        public uiDbbackup()
        {
            InitializeComponent();
            button_connect.Click += Button_connect_Click;
            button_backup.Click += Button_backup_Click;
            uiDbConnector.Instance.ConnectStringsChanged += Instance_ConnectStringsChanged;
        }

        private void Instance_ConnectStringsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_backup_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_connect_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
