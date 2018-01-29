using CommonDemo.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomaticUpdate.ViewModel
{
    class VersionViewMode : NotifyChangedBase
    {
        public RelayCommand ShowCommand { get; set; }

        private void ExecuteShowCommandJ()
        {
            Version version = new Version();
            version.WindowStartupLocation = WindowStartupLocation.Manual;
            version.Left = 0;
            version.Top = 0;
        }
    }
}
