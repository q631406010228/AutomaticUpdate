using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticUpdate.Model
{
    public class LocationEventArgs : EventArgs
    {
        public LocationEventArgs()
        {
            //
        }
        public LocationEventArgs(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        private double _x;
        private double _y;

        public double x
        {
            get { return _x; }
            set { _x = value; }
        }
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }
    }
}
