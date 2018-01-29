using CommonDemo.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AutomaticUpdate.ViewModel
{
    class ClientViewMode : NotifyChangedBase
    {
        static string TempFile = ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "temp";
        static string SrcDirectory ;
        static string Directory ;
        static ClientXml cx = new ClientXml();
        static List<string> FilePath = new List<string>();
        static Dictionary<string, string> FilesInfo = new Dictionary<string, string>();
        static List<string> FilterFile = new List<string>();   //需要过滤的文件信息
        static Socket Client;
        Boolean Pause = false;
        string fileName;
        long length;
        int IsFirstConnect  = 0;
        public event EventHandler Over;
        static int Count = 0;
        public Version version = new Version(); //显示新版本更新内容面板
        public static string VersionDetail; //新版本更新内容
        public static string ServerVersion;

        #region 属性

        private int _PBValue;
        public int PBValue
        {
            get { return this._PBValue; }
            set
            {
                this._PBValue = value;
                RaisePropertyChanged("PBValue");
            }
        }

        private string _FileNameText;
        public string FileNameText
        {
            get { return this._FileNameText; }
            set
            {
                this._FileNameText = value;
                RaisePropertyChanged("FileNameText");
            }
        }

        private string _ButtonContent;
        public string ButtonContent
        {
            get { return this._ButtonContent; }
            set
            {
                this._ButtonContent = value;
                RaisePropertyChanged("ButtonContent");
            }
        }

        private string _Rate;
        public string Rate
        {
            get { return this._Rate; }
            set
            {
                this._Rate = value;
                RaisePropertyChanged("Rate");
            }
        }

        private string _Time;
        public string Time
        {
            get { return this._Time; }
            set
            {
                this._Time = value;
                RaisePropertyChanged("Time");
            }
        }

        private string _VersionText;
        public string VersionText
        {
            get { return this._VersionText; }
            set
            {
                this._VersionText = value;
                RaisePropertyChanged("VersionText");
            }
        }

        private Visibility _VersionVisibility;
        public Visibility VersionVisibility
        {
            get { return this._VersionVisibility; }
            set
            {
                this._VersionVisibility = value;
                RaisePropertyChanged("VersionVisibility");
            }
        }
        
        private double _TaskbarValue;
        public double TaskbarValue
        {
            get { return this._TaskbarValue; }
            set
            {
                this._TaskbarValue = value;
                RaisePropertyChanged("TaskbarValue");
            }
        }

        private string _TaskbarState;
        public string TaskbarState
        {
            get { return this._TaskbarState; }
            set
            {
                this._TaskbarState = value;
                RaisePropertyChanged("TaskbarState");
            }
        }

        #endregion

        #region Loaded 事件命令

        public RelayCommand LoadCommand { get; private set; }

        private void ExecuteLoadCommand()
        {
            VersionVisibility = Visibility.Hidden;
            ReadClientXml();
            SrcDirectory = cx.SrcDirectory;
            Directory = SrcDirectory.Substring(0, SrcDirectory.LastIndexOf(Path.DirectorySeparatorChar));    //递归删除绝对路径的字符串截取，减少计算

            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(cx.IP), cx.Port);  //把ip和端口转化为IPEndpoint实例  
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建Socket  
            Client.Connect(ipe);    //连接到服务器 

            Client.Send(Encoding.Default.GetBytes(IsFirstConnect.ToString()));

            byte[] buffer = new byte[1024];
            int count = Client.Receive(buffer);
            ServerVersion = Encoding.Default.GetString(buffer, 0, count);
            if (cx.Version.Equals(ServerVersion))
            {
                Client.Close();
                FileNameText = "您的软件已经不需要更新";
                Over.Invoke(null, EventArgs.Empty);
            }
            else
            {
                Client.Send(Encoding.Default.GetBytes("y"));
                VersionVisibility = Visibility.Visible;
                VersionText = "已装 : " + cx.Version + "\t升级 : " + ServerVersion;
                byte[] b = new byte[1024];
                int c = Client.Receive(b);
                VersionDetail = Encoding.Default.GetString(b, 0, c);
                version.ShowVersion();
            }
            IsFirstConnect = 1;
            Client.Close();
        }

        public RelayCommand VersionCommand { get;private set; }

        private void ExecuteVersionCommand()
        {          
            if (version.IsVisible)
            {
                version.Hide();
            }
            else
            {
                version.Show();           
            }
        }

        public RelayCommand MinimizeWindowCommand { get; private set; }

        private void ExecuteMinimizeWindowCommand()
        {
            version.MinimizeWindow();
        }

        #endregion

        BackgroundWorker backgroundWroker;
        public ClientViewMode()
        {
            backgroundWroker = new BackgroundWorker();
            backgroundWroker.DoWork += BackgroundWorker_DoWork;
            backgroundWroker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            LoadCommand = new RelayCommand(ExecuteLoadCommand);
            VersionCommand = new RelayCommand(ExecuteVersionCommand);
            MinimizeWindowCommand = new RelayCommand(ExecuteMinimizeWindowCommand);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        { 
            Pause = false;
            FileNameText = "获取服务器数据中...";
            for (int i = 0; i < 3; i++)
            {               
                ReceiveFile(TempFile, Client);
            }         
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!Pause)
            {
                this.TaskbarState = "None";
                this.ButtonContent = "完成";
                Client.Close();
                this.FileNameText = "正在安装中...";
                this.Rate = "";
                this.Time = "";
                CompressDecompress.UnZip(TempFile + Path.DirectorySeparatorChar + "Update.zip", SrcDirectory, true);
                List<string> ls = QueryXml(TempFile + Path.DirectorySeparatorChar + "DeleteFile.xml");
                DeleteFile(SrcDirectory, ls);
                DeleteXml();
                this.FileNameText = "安装完成";
                Over.Invoke(null, EventArgs.Empty);
            }

        }

        public void ButtonSwitch()
        {
            if (Count !=1)  //在第一次等待服务端压缩数据时，使Button失效
            {
                Count++;
                if (Count % 2 == 1)
                {
                    this.TaskbarState = "Normal";
                    Download();  //客户端发送请求消息
                }
                else
                {
                    this.TaskbarState = "Paused";
                    CloseConnect();
                }               
            }           
        }

        public void Download()
        {
            try
            {
                this.FileNameText = "连接服务中...";  
                
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(cx.IP), cx.Port);  //把ip和端口转化为IPEndpoint实例  
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建Socket  
                Client.Connect(ipe);    //连接到服务器 

                if (IsFirstConnect == 1) //通知服务器（客户端是否需要续传）
                {
                    Client.Send(Encoding.Default.GetBytes("1"));
                    if (!File.Exists(TempFile + Path.DirectorySeparatorChar + "ClientFileInfo.xml"))
                    {
                        CreatFileInfoXml();
                    }
                    //CompressDecompress.ZipFileDirectory(SrcDirectory, SrcDirectory+"我是备份的压缩包.zip", FilterFile);
                    SendFile(TempFile + Path.DirectorySeparatorChar + "ClientFileInfo.xml", Client);
                }
                else
                {
                    Client.Send(Encoding.Default.GetBytes("2"));
                }            

                while (true)
                {                  
                    this.backgroundWroker.RunWorkerAsync();
                    break;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("argumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException:{0}", e);
            }
            Console.WriteLine("Press Enter to Exit");
        }

        public void CloseConnect()
        {          
            Pause = true;
            IsFirstConnect = 2;
            Client.Close();
            this.ButtonContent = "开始";
        }

        public static void ReadClientXml()    //读取客户端的配置文件
        {
            XDocument addList = XDocument.Load(".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar
                + "Resource" + Path.DirectorySeparatorChar + "Xml" + Path.DirectorySeparatorChar + "Client.xml");
            var text = from v in addList.Descendants("SrcDirectory") select v;
            foreach (var node in text)
            {
                cx.SrcDirectory = node.Value;
            }
            text = from v in addList.Descendants("Version") select v;
            foreach (var node in text)
            {
                cx.Version = node.Value;
            }
            text = from v in addList.Descendants("IP") select v;
            foreach (var node in text)
            {
                cx.IP = node.Value;
            }
            text = from v in addList.Descendants("Port") select v;
            foreach (var node in text)
            {
                cx.Port = Convert.ToInt32(node.Value);
            }
        }

        public static void CreatFileInfoXml()   //创建客户端文件信息xml
        {
            XDocument addList = XDocument.Load(".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar
                + "Resource" + Path.DirectorySeparatorChar + "Xml" + Path.DirectorySeparatorChar + "FilterFile.xml");
            var text = from v in addList.Descendants("fileInfo")
                       select v;
            foreach (var node in text)
            {
                FilterFile.Add(node.Attribute("file_name").Value);
            }
            QueryFiles.FindFile(SrcDirectory + Path.DirectorySeparatorChar, FilePath,FilterFile);
            string s = SrcDirectory.Substring(SrcDirectory.LastIndexOf(Path.DirectorySeparatorChar));
            foreach (String SrcFilePath in FilePath)
            {
                //拼接xml里文件名的相对路径
                FilesInfo.Add(s + SrcFilePath.Replace(SrcDirectory, ""), QueryFiles.GetMD5HashFromFile(SrcFilePath));
            }
            CreatXml.CreateXML(FilesInfo, TempFile + Path.DirectorySeparatorChar + "ClientFileInfo.xml");  //创建xml文件
        }

        public static void SendFile(string SendFilePath, Socket sock)
        {
            try
            {
                using (FileStream reader = new FileStream(SendFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    long send = 0L, length = reader.Length;
                    string sendStr = "namelength," + Path.GetFileName(SendFilePath) + "," + length.ToString();

                    string fileName = Path.GetFileName(SendFilePath);
                    sock.Send(Encoding.Default.GetBytes(sendStr));

                    int BufferSize = 1024 * 8;
                    byte[] buffer = new byte[32];
                    sock.Receive(buffer);
                    string mes = Encoding.Default.GetString(buffer);

                    if (mes.Contains("OK"))
                    {
                        Console.WriteLine("Sending file:" + fileName + ".Plz wait...");
                        byte[] fileBuffer = new byte[BufferSize];
                        int read, sent;
                        while ((read = reader.Read(fileBuffer, 0, BufferSize)) != 0)
                        {
                            sent = 0;
                            while ((sent += sock.Send(fileBuffer, sent, read, SocketFlags.None)) < read)
                            {
                                send += (long)sent;
                            }
                        }
                        Console.WriteLine("Send finish.\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void ReceiveFile(string path, Socket clientSocket)     //从客户端接收文件
        {           
            try
            {
                int received = 0;
                long receive = 0L;
                FileStream file;
                long LStartPos = 0;

                byte[] buffer = new byte[1024 * 8];
                int count = clientSocket.Receive(buffer);
                string[] command = Encoding.Default.GetString(buffer, 0, count).Split(',');
                fileName = command[1];
                length = Convert.ToInt64(command[2]);
                if (IsFirstConnect == 1)
                {
                    Count += 2;
                }

                if (File.Exists(TempFile + Path.DirectorySeparatorChar + fileName))
                {
                    file = new FileStream((TempFile + Path.DirectorySeparatorChar + fileName), FileMode.Append, FileAccess.Write, FileShare.None);
                    //file = File.OpenWrite(TempFile + Path.DirectorySeparatorChar + fileName);
                    LStartPos = file.Length;
                    file.Position = LStartPos;
                    received = (int)LStartPos;
                }
                else
                {
                    file = new FileStream(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
                }

                clientSocket.Send(Encoding.Default.GetBytes(LStartPos + ""));
                receive += LStartPos;
                Console.WriteLine("Receiveing file:" + fileName + ".Plz wait...");
                int i = 0;
                this.ButtonContent = "暂停";
                List<double> Sample = new List<double>
                {
                    0
                };
                DateTime now = DateTime.Now;    //获取当前电脑时间
                while (receive < length)
                {
                    if (Pause)  //暂停后就断开连接
                    {
                        break;
                    }
                    else if((receive + (1024 * 8)) > length)    //解决粘包问题
                    {
                        byte[] b = new byte[length - receive];
                        received = clientSocket.Receive(b);
                        file.Write(b, 0, received);
                    }   
                    else
                    {
                        received = clientSocket.Receive(buffer);
                        file.Write(buffer, 0, received);
                    }

                    ShowRate(Sample, received,ref now,(int)receive);    //显示下载速率

                    receive += (long)received;
                    if(i++ == 0)
                    {
                        this.FileNameText = "正在下载" + fileName;
                    }
                    
                    this.PBValue = Convert.ToInt32(receive * 100 / length);
                    this.TaskbarValue = PBValue / 100.0;
                    Thread.Sleep(2);
                }
                file.Flush();
                file.Close();
                Console.WriteLine("Receive finish.\n");
            }
            catch
            {
                Console.WriteLine("退出");
            }
        }

        /// <summary>
        /// 显示下载速率
        /// </summary>
        /// <param name="Sample">估测速率的样本</param>
        /// <param name="received">Socket每次接受的字节</param>
        /// <param name="now">上次记录的时间</param>
        /// <param name="receive">客户端一共接收到的字节</param>
        private void ShowRate(List<double> Sample,int received,ref DateTime now,int receive) 
        {
            
            Sample[Sample.Count - 1] += received;
            double Seconds = (DateTime.Now - now).TotalMilliseconds;
            if (Seconds >= 250)
            {
                now = DateTime.Now;
                if (Sample.Count() < 4)
                {
                    Sample.Add(0);
                }
                else
                {

                    double R = Forecast.WeightedMovingAverage(Sample, 1) * 4;
                    int T = Convert.ToInt32((length - receive) / R);
                    if(T >= 60)
                    {
                        Time = "剩余" + (T / 60)+ "分" + (T % 60) + "秒";
                    }
                    else if(T < 60)
                    {
                        Time = "剩余" + T + "秒";
                    }
                    if(R >= 1024 * 0124)
                    {
                        Rate = Math.Round(R / (1024 * 1024), 1) + "MB/S";
                    }
                    else if(R >= 1024)
                    {
                        Rate = Math.Round(R / 1024 , 1) + "KB/S";
                    }
                    else
                    {
                        Rate = Math.Round(R , 1) + "B/S";
                    }
                    Sample.RemoveAt(0);
                }
            }
        }

        public static List<string> QueryXml(string Path)
        {
            List<string> ls = new List<string>();
            XDocument addList = XDocument.Load(@Path);
            var text = from v in addList.Descendants("sysconfig")
                       from x in v.Elements()
                       select x;
            foreach (var node in text)
            {
                ls.Add(node.Attribute("file_name").Value);
            }
            return ls;
        }

        /// <summary>
        /// 参数dirPath为指定的目录
        /// </summary>
        /// <param name="dirPath">根目录的绝对路径</param>
        /// <param name="ls">Delete.xml中需要删除的文件夹和文件</param>
        public static void DeleteFile(string dirPath, List<string> ls) 
        {           
            //在指定目录及子目录下查找文件,在listBox1中列出子目录及文件
            DirectoryInfo Dir = new DirectoryInfo(dirPath);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())//查找子目录
                {
                    if (ls.Contains((dirPath.Replace(Directory,"") + Path.DirectorySeparatorChar + d.ToString()))) 
                    {
                        File.Delete(dirPath + Path.DirectorySeparatorChar + d.ToString());
                    }
                    else
                    {
                        DeleteFile(dirPath + Path.DirectorySeparatorChar + d.ToString(), ls);
                    }
                }
                foreach (FileInfo f in Dir.GetFiles()) //查找文件
                {
                    //文件与Delete.xml匹配（将dirPath绝对路径转为相对路径）
                    if (ls.Contains((dirPath.Replace(Directory, "") + Path.DirectorySeparatorChar + f.ToString())))
                    {
                        File.Delete(dirPath + Path.DirectorySeparatorChar + f.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void DeleteXml()
        {
            File.Delete(TempFile + Path.DirectorySeparatorChar + "DeleteFile.xml");
            File.Delete(TempFile + Path.DirectorySeparatorChar + "Update.zip");
            File.Delete(TempFile + Path.DirectorySeparatorChar + "ClientFileInfo.xml");
            File.Move(TempFile + Path.DirectorySeparatorChar + "ServerFileInfo.xml", TempFile + Path.DirectorySeparatorChar + "ClientFileInfo.xml");

            XElement xele = XElement.Load(".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar
                + "Resource" + Path.DirectorySeparatorChar + "Xml" + Path.DirectorySeparatorChar + "Client.xml");
            var item = (from ele in xele.Elements("Version") select ele).FirstOrDefault();
            if (item != null)
            {
                item.ReplaceWith(new XElement("Version", ServerVersion));
            }
            xele.Save(".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar
                + "Resource" + Path.DirectorySeparatorChar + "Xml" + Path.DirectorySeparatorChar + "Client.xml");
        }

        public void Restore()
        {
            version.Restore();
        }

    }
}
