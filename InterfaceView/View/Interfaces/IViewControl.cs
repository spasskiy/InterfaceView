using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceView.View.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с классами контролов элементов отображения (Сотка2, БКМ и т.д.)
    /// </summary>
    interface IViewControl
    {
        string ViewControlName { get; }
        bool IsActive { get; set; }
    }
}
