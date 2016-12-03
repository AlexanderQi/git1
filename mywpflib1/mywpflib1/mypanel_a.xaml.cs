using System;
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

namespace mywpflib1
{
    /// <summary>
    /// MyPanel_A.xaml 的交互逻辑
    /// </summary>
    public partial class MyPanel_A : UserControl
    {
        public MyPanel_A()
        {
            InitializeComponent();
        }

        public string Caption{
            get {return tbCaption.Text; }
            set {tbCaption.Text=value; }
        }


        public Brush Icon
        {
            get {return ellipse_icon.Fill; }
            set {ellipse_icon.Fill=value; }
        }

    }
}
