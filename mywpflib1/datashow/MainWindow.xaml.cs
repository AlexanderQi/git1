using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Threading;

namespace datashow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        public Page_JL PJL = new Page_JL();
        public PageEquip PEQ = new PageEquip();
        public Page curPage;
        private static DispatcherTimer readDataTimer = new DispatcherTimer();
        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            setCurPage(PJL);
            readDataTimer.Tick += ReadDataTimer_Tick;
            readDataTimer.Interval = new TimeSpan(0, 0, 10);

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //frame.Width = instance.ActualWidth;
            //frame.Height = instance.ActualHeight;
            //frame.Margin = new Thickness(6);
            readDataTimer.Start();
        }

        public void setCurPage(Page p)
        {
            frame.Navigate(p);
            curPage = p;
        }

        private void ReadDataTimer_Tick(object sender, EventArgs e)
        {
            curPage.RaiseEvent(new RoutedEventArgs(PageFunctionBase.LoadedEvent));
            
        }

        private void TileBarItem_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void TileBarItem_Click_1(object sender, EventArgs e)
        {
            setCurPage(PJL);
        }

        private void TileBarItem_Click_2(object sender, EventArgs e)
        {
            setCurPage(PEQ);
        }

        private void TileBarItem_Click_3(object sender, EventArgs e)
        {
            
            setCurPage(PEQ);

        }
    }

    ///<summary>
    /// 我的PNG转换器
    /// </summary>
    public class MyPngConverter : IValueConverter
    {
   

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();

            string str = (string)value;
            return str + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
