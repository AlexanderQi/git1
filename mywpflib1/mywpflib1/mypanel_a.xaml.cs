using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static readonly DependencyProperty DCaptionProperty = DependencyProperty.Register("DCaption", typeof(string), typeof(MyPanel_A),
            new PropertyMetadata("caption", OnValueChanged, null));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
            Trace.WriteLine("OnValueChanged--> " + e.NewValue + ";" + e.OldValue);
            (d as MyPanel_A).Caption = e.NewValue as string;
        }

        public MyPanel_A()
        {
            InitializeComponent();
            
            
        }

        public string Caption{
            get {return tbCaption.Text; }
            set {tbCaption.Text=value;}
        }

        public string DCaption
        {
            get { return (string)GetValue(DCaptionProperty); }
            set {
                SetValue(DCaptionProperty, value);
            }
        }


        public Brush Icon
        {
            get {return ellipse_icon.Fill; }
            set {ellipse_icon.Fill=value; }
        }

    }
}
