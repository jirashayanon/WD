using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PhotoLibrary.Helpers
{
    public class Points
    {

        int Touch_id;

        public Point P_after { get; set; }

        private Point _p_before;
        public Point P_before
        {
            get
            {
                return _p_before;
            }
            set
            {
                _p_before = value;
            }
        }

        public void setOriginal(TouchPoint p)
        {
            P_before = p.Position;
            Touch_id = p.TouchDevice.Id;
        }
    }

}
