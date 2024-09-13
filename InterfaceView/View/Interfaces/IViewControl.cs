using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceView.View.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с классами контролов элементов отображения (Сотка2, БКМ и т.д.)
    /// </summary>
    public interface IViewControl : INotifyPropertyChanged
    {
        string ViewControlType { get; }
        string ViewControlName { get; }
        bool IsActive { get; set; }
        ObservableCollection<IViewControl> Elements { get; set; }
        IViewControl Parent { get; set; }
        void AddChildren(IViewControl control);
    }
}
