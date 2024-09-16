using InterfaceView.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для ItemTreeViewControl.xaml
    /// </summary>
    public partial class ItemTreeViewControl : UserControl
    {
        public static readonly DependencyProperty CanvasProperty =
            DependencyProperty.Register("Canvas", typeof(Canvas), typeof(ItemTreeViewControl), new PropertyMetadata(null));

        public Canvas Canvas
        {
            get { return (Canvas)GetValue(CanvasProperty); }
            set { SetValue(CanvasProperty, value); }
        }

        public ItemTreeViewControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void BuildTreeView()
        {
            if (Canvas == null) return;

            // Извлекаем корневой элемент
            var rootControl = GetRootControl(Canvas);

            if (rootControl == null) return;

            // Строим иерархию только для корневого элемента и его детей
            var hierarchy = BuildHierarchy(rootControl);

            // Отображаем иерархию в TreeView
            ControlTreeView.ItemsSource = hierarchy;


        }

        private IViewControl GetRootControl(Canvas canvas)
        {
            foreach (var child in canvas.Children)
            {
                if (child is IViewControl control && control.Parent == null)
                {
                    return control;
                }
            }
            return null;
        }

        private ObservableCollection<IViewControl> BuildHierarchy(IViewControl rootControl)
        {
            var hierarchy = new ObservableCollection<IViewControl>();
            hierarchy.Add(rootControl);
            BuildHierarchyRecursive(rootControl, hierarchy);
            return hierarchy;
        }

        private void BuildHierarchyRecursive(IViewControl parent, ObservableCollection<IViewControl> hierarchy)
        {
            foreach (var child in parent.Elements)
            {
                //hierarchy.Add(child);
                BuildHierarchyRecursive(child, hierarchy);
            }
        }



    }
}