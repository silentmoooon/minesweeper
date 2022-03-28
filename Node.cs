 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace minesweeper
{
    class Node : INotifyPropertyChanged
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


        public readonly Node[] nodes = new Node[8];

        private bool displayStatus;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool DisplayStatus
        {
            get { return displayStatus; }
            set
            {
                displayStatus = value;
                OnPropertyChanged("Color");
                OnPropertyChanged("Text");
            }
        }
     
        public bool IsMine { get; set; }


        private bool isMaybeMine;
        public bool IsMaybeMine { get { return isMaybeMine; } set { isMaybeMine = value; OnPropertyChanged("Color"); OnPropertyChanged("Text"); } }

       
        public int MineCount { get; set; }

        public int Row { get; set; }
        public int Col { get; set; }

        public Brush Color { get {
                if (IsMaybeMine)
                {
                    return Brushes.CadetBlue;
                }
                return DisplayStatus ? Brushes.LightGray : Brushes.RoyalBlue; 
            } }

        public string Text { get
            {
                if (!displayStatus) { 
                    if (isMaybeMine)
                    {
                        return "?";
                    }
                
                    return "";
                }
                if (IsMine)
                {
                    return "雷";
                }
                if (MineCount == 0)
                {
                    return "";
                }
                return MineCount.ToString();
            } 
        }
 


    }
}
