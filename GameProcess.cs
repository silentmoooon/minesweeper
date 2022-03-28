using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper
{
    class GameProcess : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private int remainderCount;
        public int RemainderCount { get { return remainderCount; } set { remainderCount = value;
                OnPropertyChanged("RemainderCount");
            } }

        private int time;
        public int Time { get { return time; } set { time = value;OnPropertyChanged("Time"); } }   
    }
}
