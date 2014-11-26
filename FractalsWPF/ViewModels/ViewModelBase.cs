using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalsWPF.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public String DisplayName { get; set; }

        #region INotifyPropertyChanged Members

        protected void RaisePropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
