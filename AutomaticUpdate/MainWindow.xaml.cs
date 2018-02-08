using AutomaticUpdate.Model;
using AutomaticUpdate.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AutomaticUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        int num = 0; //已经复制成功的文件数
        List<String> ListDirectory = new List<String> { };
        Dictionary<string, DeTailFile> ListDF = new Dictionary<string, DeTailFile>();
        Dictionary<string, string> filesInfo = new Dictionary<string, string>();
        ClientViewMode ClientVM = new ClientViewMode();

        //添加一个委托
        public delegate void PassDataBetweenFormHandler(object sender, LocationEventArgs e);
        //添加一个PassDataBetweenFormHandler类型的事件
        public static event PassDataBetweenFormHandler PassDataBetweenForm;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ClientVM;
            ClientVM.Over += ClientVM_Over;
            this.LocationChanged += new EventHandler(Window_LocationChanged);
            this.StateChanged += new EventHandler(FormMain_Resize);
            RollImg();

            AppLog.Info("Info log");
            AppLog.Error("Error log");

            //this.Begin.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.BeginUpdate));
            //ListDF = QueryFiles.FindFile(SrcDirectory + "\\", ListDF, ListDirectory);    //搜索要复制的文件夹和文件
            //fileDate.Text = Convert.ToString(ListDF.Count());  //需要复制文件的总数  
            //FileList.ItemsSource = ListDF.Values;
        }

        private void RollImg()
        {
            List<BitmapImage> ls_adv_img = new List<BitmapImage>();
            List<int> listAdv = new List<int>() { 1, 2 };

            // 根据自己的业务逻辑进行赋值操作  
            foreach (int i in listAdv)
            {
                BitmapImage img;
                try
                {
                    img = new BitmapImage(new Uri(string.Format(@"Resource\Image\{0}.png", i), UriKind.Relative));
                }
                catch (Exception ex)
                {
                    img = new BitmapImage();
                }
                ls_adv_img.Add(img);
            }

            this.rollImg.ls_images = ls_adv_img;

            this.rollImg.Begin();
        }

        private void BeginUpdate(object sender, RoutedEventArgs e)
        {
            ClientVM.ButtonSwitch();
        }

        private void ClientVM_Over(object sender,EventArgs e)
        {
            Begin.Content = "完成";
            Begin.Click -= BeginUpdate;
            Begin.Click += CloseWindows;
        }

        private void CloseWindows(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        ////开始自动更新
        //private void BeginUpdate(object sender, RoutedEventArgs e)
        //{
        //    string SrcFilePath;
        //    string DestFilePath;
        //    pb.Maximum = ListDF.Count();    //与UI线程处于同一个线程
        //    foreach (String s in ListDirectory) //复制文件夹
        //    {
        //        String s1 = DestDirctory + s.Replace(SrcDirectory, "");
        //        Directory.CreateDirectory(s1);
        //    }
        //    foreach (String s in ListDF.Keys)   //复制文件
        //    {
        //        SrcFilePath = s;
        //        DestFilePath = DestDirctory + s.Replace(SrcDirectory, "");
        //        filesInfo.Add(SrcDirectory.Substring(SrcDirectory.LastIndexOf("/"))+s.Replace(SrcDirectory,""), QueryFiles.GetMD5HashFromFile(s)); //拼接xml里文件名的相对路径
        //        Path p = new Path { SrcPath = SrcFilePath, DestPath = DestFilePath };
        //        ThreadPool.QueueUserWorkItem(UpdateFile, p);    //使用线程池
        //    }
        //    CreatXml.CreateXML(filesInfo);  //创建xml文件
        //}

        //复制的过程
        private void UpdateFile(String srcPath, String destPath)
        {
            int a = 0;
            FileInfo f = new FileInfo(srcPath);
            FileStream fsR = f.OpenRead();
            FileStream fsW = File.Create(destPath);
            byte[] buffer = new byte[1024];
            int n = 0;  //输入输出流的值
            while (true)
            {
                n = fsR.Read(buffer, 0, 1024);
                if (n == 0)
                {
                    if (a == 0)  //windows里有0字节的文件
                    {
                        ListDF[srcPath].FValue = 100;
                    }
                    break;
                }
                fsW.Write(buffer, 0, n);
                fsW.Flush();
                a += n;
                ListDF[srcPath].FValue = a * 100 / fsR.Length;  //单个文件进度条的更新
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<long>(UpdateProgressBar), ++num); //总进度条的更新
            ListDF[srcPath].IsCompleted = "下载完成";
            ListDF[srcPath].IsCompletedColor = "#59b342";
            fsR.Close();
            fsW.Close();
        }

        //关闭窗口
        private void ColseWindow(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            Version.ShutDown();
        }

        //最小化窗口到任务栏
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //复制数据的委托
        private void UpdateFile(object obj)
        {
            FilePath p = obj as FilePath;
            UpdateFile(p.SrcPath, p.DestPath);
        }

        //更新总进度条
        private void UpdateProgressBar(long fileNum) 
        {
            fileDate.Text = Convert.ToString(fileNum);
            pb.Value = 50;
        }

        private void Window_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)    //可拖动
        {
                this.DragMove();
        }

        private void Window_LocationChanged(object sender,EventArgs e)
        {
            LocationEventArgs args = new LocationEventArgs(this.Left + this.Width , this.Top);
            PassDataBetweenForm(this, args);
        }   //通知子窗口一起移动

        private void FormMain_Resize(object sender, EventArgs e)
        {
            //窗体恢复正常时  
            if (this.WindowState == WindowState.Normal)
            {
                ClientVM.Restore();
            }
        }

    }

}
