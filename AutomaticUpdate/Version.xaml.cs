using AutomaticUpdate.Model;
using AutomaticUpdate.ViewModel;
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
using System.Windows.Shapes;

namespace AutomaticUpdate
{
    /// <summary>
    /// Version.xaml 的交互逻辑
    /// </summary>
    public partial class Version : Window
    {

        public Version()
        {
            InitializeComponent();
            MainWindow.PassDataBetweenForm += new MainWindow.PassDataBetweenFormHandler(FrmChild_PassDataBetweenForm);
        }

        private void FrmChild_PassDataBetweenForm(object sender, LocationEventArgs e)
        {
            this.Left = e.x;
            this.Top = e.y;
        }

        public void ShowVersion()
        {
            List<VersionDetail> VersionDetailList = new List<VersionDetail>();
            String[] s = ClientViewMode.VersionDetail.Split(',');
            foreach (string s1 in s)
            {
                VersionDetail versionDetail = new VersionDetail();
                versionDetail.VersionDetailItem = s1;
                VersionDetailList.Add(versionDetail);
            }
            VersionList.ItemsSource = VersionDetailList;
        }

        public static void ShutDown()
        {
            Application.Current.Shutdown();
        }

        //最小化窗口到任务栏
        public void MinimizeWindow()
        {
            this.WindowState = WindowState.Minimized;
        }

        public void Restore()
        {
            this.WindowState = WindowState.Normal;
        }
    }
}
