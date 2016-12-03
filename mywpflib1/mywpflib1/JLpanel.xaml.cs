using System.Windows.Controls;
using System.Windows.Media;

namespace mywpflib1
{
    /// <summary>
    /// JLpanel.xaml 的交互逻辑
    /// </summary>
    public partial class JLpanel : UserControl
    {
        public JLpanel()
        {
            InitializeComponent();
        }

        public string caption0
        {
            get { return tb0.Text; }
            set { tb0.Text = value; }
        }

        public string caption1
        {
            get { return tb1.Text; }
            set { tb1.Text = value; }
        }
        public string caption11
        {
            get { return tb11.Text; }
            set { tb11.Text = value; }
        }

        public string caption2
        {
            get { return tb2.Text; }
            set { tb2.Text = value; }
        }
        public string caption22
        {
            get { return tb22.Text; }
            set { tb22.Text = value; }
        }

        public string caption3
        {
            get { return tb3.Text; }
            set { tb3.Text = value; }
        }
        public string caption33
        {
            get { return tb33.Text; }
            set { tb33.Text = value; }
        }

        public string caption4
        {
            get { return tb4.Text; }
            set { tb4.Text = value; }
        }
        public string caption44
        {
            get { return tb44.Text; }
            set { tb44.Text = value; }
        }


        public string caption5
        {
            get { return tb5.Text; }
            set { tb5.Text = value; }
        }
        public string caption55
        {
            get { return tb55.Text; }
            set { tb55.Text = value; }
        }

        public string caption6
        {
            get { return tb6.Text; }
            set { tb6.Text = value; }
        }
        public string caption66
        {
            get { return tb66.Text; }
            set { tb66.Text = value; }
        }


        public ImageSource p1
        {
            get { return img1.Source; }
            set { img1.Source = value; }
        }
        public ImageSource p2
        {
            get { return img2.Source; }
            set { img2.Source = value; }
        }
        public ImageSource p3
        {
            get { return img3.Source; }
            set { img3.Source = value; }
        }
        public ImageSource p4
        {
            get { return img4.Source; }
            set { img4.Source = value; }
        }

        public ImageSource p5
        {
            get { return img5.Source; }
            set { img5.Source = value; }
        }

        public ImageSource p6
        {
            get { return img6.Source; }
            set { img6.Source = value; }
        }
    }
}
