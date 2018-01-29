using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomaticUpdate.ViewModel
{
    class QueryFiles : MainWindow 
    {
        public static void FindFile(string dirPath, List<DeTailFile> FilePath) //参数dirPath为指定的目录
        {
            //在指定目录及子目录下查找文件,在listBox1中列出子目录及文件
            DirectoryInfo Dir = new DirectoryInfo(dirPath);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())//查找子目录
                {
                    FindFile(d.FullName, FilePath);
                }
                foreach (FileInfo f in Dir.GetFiles()) //查找文件
                {
                    DeTailFile d = new DeTailFile
                    {
                        Name = f.FullName
                    };
                    FilePath.Add(d);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        //查询文件信息
        public static Dictionary<string, DeTailFile> FindFile(string dirPath, Dictionary<string, DeTailFile> ListDF, List<String> ListDirectory) //参数dirPath为指定的目录
        {
            //在指定目录及子目录下查找文件,在listBox1中列出子目录及文件
            DirectoryInfo Dir = new DirectoryInfo(dirPath);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())//查找子目录
                {
                    ListDirectory.Add(Dir + d.ToString());
                    FindFile(Dir + d.ToString() + "\\",ListDF,ListDirectory);
                }
                foreach (FileInfo f in Dir.GetFiles()) //查找文件
                {
                    DeTailFile df = new DeTailFile();
                    if (f.Length <= 1024)
                    {
                        df.Capacity = 1;
                    }
                    else
                    {
                        df.Capacity = f.Length / 1024;
                    }
                    df.FileMD5 = "aaaa";
                    df.Name = f.ToString();
                    ListDF.Add(Dir + f.ToString(), df);
                }                              
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return ListDF;
        }

        public static void FindFile(string dirPath,List<string> FilePath,List<string> FilterFile) //参数dirPath为指定的目录
        {
            //在指定目录及子目录下查找文件,在listBox1中列出子目录及文件
            DirectoryInfo Dir = new DirectoryInfo(dirPath);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())//查找子目录
                {
                    if(!FilterFile.Contains(Dir + d.Name))
                    {
                        FindFile(Dir + d.Name + Path.DirectorySeparatorChar, FilePath, FilterFile);
                    }
                }
                foreach (FileInfo f in Dir.GetFiles()) //查找文件
                {
                    if (!FilterFile.Contains(Dir + f.Name))
                    {
                        FilePath.Add(Dir + f.Name);
                    }                 
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //查询文件信息
        public static Dictionary<string, DeTailFile> FindFileXml(string dirPath, Dictionary<string, DeTailFile> ListDF, List<String> ListDirectory) //参数dirPath为指定的目录
        {
            //在指定目录及子目录下查找文件,在listBox1中列出子目录及文件
            DirectoryInfo Dir = new DirectoryInfo(dirPath);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())//查找子目录
                {
                    ListDirectory.Add(Dir + d.ToString());
                    FindFile(Dir + d.ToString() + "\\", ListDF, ListDirectory);
                }
                foreach (FileInfo f in Dir.GetFiles()) //查找文件
                {
                    DeTailFile df = new DeTailFile();
                    if (f.Length <= 1024)
                    {
                        df.Capacity = 1;
                    }
                    else
                    {
                        df.Capacity = f.Length / 1024;
                    }
                    df.FileMD5 = "aaaa";
                    df.Name = f.ToString();
                    ListDF.Add(Dir + f.ToString(), df);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return ListDF;
        }

        //文件的MD5码
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}
