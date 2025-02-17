﻿using System;
using System.Collections.Generic;

namespace InterfaceView.Model
{
    [Serializable]
    public class ViewControlData
    {
        public string ViewControlType { get; set; } // Изменено на ViewControlType
        public string ViewControlName { get; set; } // Добавлено ViewControlName
        public double Left { get; set; }
        public double Top { get; set; }
        public bool IsActive { get; set; }
        public string ParentName { get; set; }
        public string IPAddress { get; set; }
        public List<ViewControlData> Children { get; set; } = new List<ViewControlData>();
        public List<NodeParamData> NodeParams { get; set; } = new List<NodeParamData>();
    }

    [Serializable]
    public class CanvasData
    {
        public List<ViewControlData> Controls { get; set; } = new List<ViewControlData>();
    }

    [Serializable]
    public class NodeParamData
    {
        public string ParamName { get; set; }
        public double ParamValue { get; set; }
        public string MeasureUnit { get; set; }
    }
}