using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InterfaceView.Model
{
    [Serializable]
    public class ViewControlData
    {
        public string Name { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public bool IsActive { get; set; }
        public string ParentName { get; set; }
        public List<ViewControlData> Children { get; set; } = new List<ViewControlData>();
    }

    [Serializable]
    public class CanvasData
    {
        public List<ViewControlData> Controls { get; set; } = new List<ViewControlData>();
    }
}