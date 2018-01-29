using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Server
{
    class Server
    {
        static string TempFile = ".." + Path.DirectorySeparatorChar+ ".." + Path.DirectorySeparatorChar + "temp";
        static List<String> UpdateFile = new List<String>();
        static List<String> ListDirectory = new List<String> { };
        static Dictionary<string, DeTailFile> ListDF = new Dictionary<string, DeTailFile>();
        static Dictionary<string, string> filesInfo = new Dictionary<string, string>();
        static ServerXml sx = new ServerXml();  //服务端配置文件信息
        static List<string> FilterFile = new List<string>();    //需要进行过滤的文件
        static CompressDecompress cd = new CompressDecompress();
        static Dictionary<string, string> IsFirstConnect = new Dictionary<string, string>();
        

        static void Main(string[] args)
        {
            ReadServerXml();
           
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(sx.IP), sx.Port);  //用指定的端口和ip初始化IPEndPoint类的新实例 
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(ipe);    //绑定EndPoint对像（2000端口和ip地址）  
            s.Listen(1);    //开始监听  
            if (!File.Exists(TempFile + Path.DirectorySeparatorChar + "ServerFileInfo.xml"))
            {
                ReadFilterFileXml();
                CreatServerFileXml();
            }
            while (true)
            {
                Socket client = s.Accept();
                if (client.Connected)
                {
                    Thread cThread = new Thread(new ParameterizedThreadStart(MyClient))
                    {
                        IsBackground = true
                    };
                    cThread.Start(client);
                    //Console.WriteLine(client.RemoteEndPoint.ToString());
                }
            }
        }   
        public static void MyClient(object Client)  //与客户端进行文件传输
        {
            Socket S = (Socket)Client;
            string ClientIP = S.RemoteEndPoint.ToString();
            ClientIP = ClientIP.Substring(0,ClientIP.IndexOf(":"));

            //从客户端接收消息判断连接的次数
            byte[] buffer = new byte[1];
            int count = S.Receive(buffer);
            string b = Encoding.Default.GetString(buffer);
            if (IsFirstConnect.ContainsKey(ClientIP))
            {
                IsFirstConnect[ClientIP] = b;
            }
            else
            {
                IsFirstConnect.Add(ClientIP, b);
            }

            if (IsFirstConnect[ClientIP].Equals("0"))   //判断是否是第一次询问版本
            {
                S.Send(Encoding.Default.GetBytes(sx.Version));
                byte[] buffer1 = new byte[1024];
                int count1 = S.Receive(buffer1);
                string IsUpdate = Encoding.Default.GetString(buffer1, 0, count1);
                if (IsUpdate.Equals("y"))
                {
                    S.Send(Encoding.Default.GetBytes(sx.VersionItem));
                }
            }
            else
            {
                if (IsFirstConnect[ClientIP].Equals("1"))   //判断是不是需要压缩
                {
                    ReceiveFile(TempFile + Path.DirectorySeparatorChar + ClientIP, S);
                    XmlDiff(ClientIP);
                }
                SendFile(TempFile + Path.DirectorySeparatorChar + ClientIP + Path.DirectorySeparatorChar + "DeleteFile.xml", S);
                SendFile(TempFile + Path.DirectorySeparatorChar + "ServerFileInfo.xml", S);
                SendFile(TempFile + Path.DirectorySeparatorChar + ClientIP + Path.DirectorySeparatorChar + "Update.zip", S);
                //DeleteUpdateFile(sx.SrcDirectory);
            }
        }
        public static void CreatServerFileXml() //创建服务端文件信息xml
        {
            ListDF = QueryFiles.FindFile(sx.SrcDirectory+ Path.DirectorySeparatorChar , ListDF, ListDirectory);    //搜索要复制的文件夹和文件
            string s = sx.SrcDirectory.Substring(sx.SrcDirectory.LastIndexOf(Path.DirectorySeparatorChar)); //软件的根目录
            foreach (String SrcFilePath in ListDF.Keys)
            {
                if (!FilterFile.Contains(sx.SrcDirectory + SrcFilePath.Replace(sx.SrcDirectory, "")))
                {
                    //拼接xml里文件名的相对路径
                    filesInfo.Add(s + SrcFilePath.Replace(sx.SrcDirectory, ""), QueryFiles.GetMD5HashFromFile(SrcFilePath));
                }
            }
            CreatXml.CreateXML(filesInfo, TempFile + Path.DirectorySeparatorChar + "ServerFileInfo.xml",sx.Version);  //创建xml文件           
        }
        public static void XmlDiff(string ClientIP)    //对比客户端与服务端xml文件的差异，生成需要删除的文件和更新文件的压缩包
        {
            List<String> DeleteFile = new List<string>();
            List<DeTailFile> ClientLDF = QueryXml.QueryXmlMain(TempFile + Path.DirectorySeparatorChar +ClientIP + Path.DirectorySeparatorChar + "ClientFileInfo.xml");
            List<DeTailFile> ServerLDF = QueryXml.QueryXmlMain(TempFile + Path.DirectorySeparatorChar + "ServerFileInfo.xml");
            int[] ClientCount = new int[ClientLDF.Count()];
            int[] ServerCount = new int[ServerLDF.Count()];
            string s = sx.SrcDirectory.Substring(0, sx.SrcDirectory.LastIndexOf(Path.DirectorySeparatorChar));
            for (int i = 0; i < ClientLDF.Count(); i++)
            {
                for (int j = 0; j < ServerLDF.Count(); j++)
                {
                    if (ClientLDF[i].Name.Equals(ServerLDF[j].Name))
                    {
                        ClientCount[i]++;
                        ServerCount[j]++;
                        if (!ClientLDF[i].FileMD5.Equals(ServerLDF[j].FileMD5))
                        {
                            UpdateFile.Add(s + ServerLDF[i].Name);
                        }
                    }
                }
            }
            for (int i = 0; i < ServerLDF.Count(); i++)
            {
                if (ServerCount[i] == 0)
                {
                    UpdateFile.Add(s + ServerLDF[i].Name);
                }
            }
            cd.ZipFileDirectory(sx.SrcDirectory, TempFile + Path.DirectorySeparatorChar +ClientIP + Path.DirectorySeparatorChar + "Update.zip", UpdateFile); //压缩需要更新的文件
            //添加需要删除的文件名
            for (int i = 0; i < ClientLDF.Count(); i++)
            {
                if (ClientCount[i] == 0)
                {
                    DeleteFile.Add(ClientLDF[i].Name);
                }
            }
            CreatXml.CreateXML(DeleteFile, TempFile + Path.DirectorySeparatorChar +ClientIP + Path.DirectorySeparatorChar + "DeleteFile.xml");
        }     
        public static void ReceiveFile(string ReceiveFilePath, Socket clientSocket) //从客户端接收文件
        {
            string clientName = clientSocket.RemoteEndPoint.ToString();
            Console.WriteLine("新来一个客户:" + clientName);
            try
            {
                byte[] buffer = new byte[1024 * 8];
                int count = clientSocket.Receive(buffer);
                Console.WriteLine("收到" + clientName + ":" + Encoding.Default.GetString(buffer, 0, count));
                string[] command = Encoding.Default.GetString(buffer, 0, count).Split(',');
                string fileName;
                long length;
                if (command[0] == "namelength")
                {
                    fileName = command[1];
                    length = Convert.ToInt64(command[2]);
                    clientSocket.Send(Encoding.Default.GetBytes("OK"));
                    long receive = 0L;
                    Console.WriteLine("Receiveing file:" + fileName + ".Plz wait...");
                    using (FileStream writer = new FileStream(Path.Combine(ReceiveFilePath, fileName), FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        int received;
                        while (receive < length)
                        {
                            received = clientSocket.Receive(buffer);
                            writer.Write(buffer, 0, received);
                            writer.Flush();
                            receive += (long)received;
                        }
                    }
                    Console.WriteLine("Receive finish.\n");
                    
                }
            }
            catch
            {
                Console.WriteLine("客户:" + clientName + "退出");
            }
        }    
        public static void SendFile(string SendFilePath, Socket sock)   //发送文件
        {
            try
            {
                using (FileStream reader = new FileStream(SendFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    int read;
                    long length = reader.Length;
                    string sendStr = "namelength," + Path.GetFileName(SendFilePath) + "," + length.ToString();
                    string fileName = Path.GetFileName(SendFilePath);
                    sock.Send(Encoding.Default.GetBytes(sendStr));

                    int BufferSize = 1024 * 8;
                    byte[] buffer = new byte[1024];
                    int count = sock.Receive(buffer);
                    long LStartPos = Convert.ToInt64(Encoding.Default.GetString(buffer, 0,count));  //客户端已下载的文件长度
                    if (LStartPos != 0)
                    {
                        reader.Position = LStartPos;
                    }

                    Console.WriteLine("Sending file:" + fileName + ".Plz wait...");
                    byte[] fileBuffer = new byte[BufferSize];
                    int a = 0;
                    while ((read = reader.Read(fileBuffer, 0, BufferSize)) != 0) 
                    {                           
                        sock.Send(fileBuffer, 0, read, SocketFlags.None);
                        a += read;
                    }
                    reader.Flush();
                    reader.Close();
                    Console.WriteLine("Send finish.\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }      
        public static void DeleteUpdateFile(string FilePath)     //删除更新时所用的xml和压缩包
        {
            File.Delete(FilePath + Path.DirectorySeparatorChar + "DeleteFile.xml");
            File.Delete(FilePath + Path.DirectorySeparatorChar + "ServerFileInfo.xml");
            File.Delete(FilePath + Path.DirectorySeparatorChar + "Update.zip");
        }    
        public static void ReadServerXml()    //读取服务端的配置文件
        {
            XDocument addList = XDocument.Load(@".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Server.xml");
            var text = from v in addList.Descendants("SrcDirectory") select v;
            foreach (var node in text)
            {
                sx.SrcDirectory = node.Value.Replace("\\", Path.DirectorySeparatorChar.ToString());
            }
            text = from v in addList.Descendants("VersionNumber") select v;
            foreach (var node in text)
            {
                sx.Version = node.Value;
            }
            text = from v in addList.Descendants("VersionItem") select v;
            int n = 0;
            foreach (var node in text)
            {            
                sx.VersionItem += node.Value;
                if(n != text.Count() - 1)
                {
                    sx.VersionItem += ",";
                }
                n++;
            }
            text = from v in addList.Descendants("IP") select v;
            foreach (var node in text)
            {
                sx.IP = node.Value;
            }
            text = from v in addList.Descendants("Port") select v;
            foreach (var node in text)
            {
                sx.Port = Convert.ToInt32(node.Value);
            }
        }
        public static void ReadFilterFileXml()
        {
            XDocument addList = XDocument.Load(@".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Server.xml");
            var text = from v in addList.Descendants("fileInfo") select v;
            foreach (var node in text)
            {
                FilterFile.Add(node.Attribute("file_name").Value);
            }
        }

    }
}
