using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticUpdate
{

    public class DeTailFile : INotifyPropertyChanged
    {
        public long _FValue = 0;
        public string _IsCompleted = "下载中...";
        public string _IsCompletedColor = "white";
        public DeTailFile()
        {
            Capacity = 1;
        }
        public string FileMD5 { get; set; } 
        public long Capacity { get; set; }
        public string Name { get; set; }
        public long FValue
        {
            get { return _FValue; }
            set
            {
                if (_FValue != value)
                {
                    _FValue = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FValue"));
                }
            }
        }
        public string IsCompleted
        {
            get { return _IsCompleted; }
            set
            {
                if (_IsCompleted != value)
                {
                    _IsCompleted = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
                }
            }
        }
        public string IsCompletedColor
        {
            get { return _IsCompletedColor; }
            set
            {
                if (_IsCompletedColor != value)
                {
                    _IsCompletedColor = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsCompletedColor"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
