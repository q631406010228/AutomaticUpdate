using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDemo.Utility;
using GalaSoft.MvvmLight.Command;

namespace AutomaticUpdate.ViewModel
{
    class UpdateViewMode : NotifyChangedBase
    {
        #region 属性

        private List<DeTailFile> _DetailFileList;
        public List<DeTailFile> DetailFileList
        {
            get { return _DetailFileList; }
            set
            {
                this._DetailFileList = value;
                RaisePropertyChanged("DetailFileList");
            }
        }
        #endregion

        BackgroundWorker backgroundWroker;

        public UpdateViewMode()
        {
            backgroundWroker = new BackgroundWorker();
            backgroundWroker.DoWork += BackgroundWorker_DoWork;
            backgroundWroker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            ShowFileList();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void BackgroundWorker_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {

        }

        private void ShowFileList()
        {
            List<DeTailFile> s = new List<DeTailFile>();
            QueryFiles.FindFile("E:\\客户端\\QuantShell", s);
            this.DetailFileList = s;
        }
    }
}
