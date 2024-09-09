using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace ModbusRTUScanner.View.Style
{
    public partial class CustomWindow : ResourceDictionary
    {
        /// <summary>
        /// Статический конструктор для регистрации команд управления окном
        /// </summary>
        static CustomWindow()
        {
            CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(MinimizeCommand, MinimizeExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(MaximizeRestoreCommand, MaximizeRestoreExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(CloseCommand, CloseExecuted));
        }

        /// <summary>
        /// Команда для минимизации окна
        /// </summary>
        public static readonly RoutedCommand MinimizeCommand = new("Minimize", typeof(WindowStyle));

        /// <summary>
        /// Команда для максимизации или восстановления окна
        /// </summary>
        public static readonly RoutedCommand MaximizeRestoreCommand = new("MaximizeRestore", typeof(WindowStyle));

        /// <summary>
        /// Команда для закрытия окна
        /// </summary>
        public static readonly RoutedCommand CloseCommand = new("Close", typeof(WindowStyle));

        /// <summary>
        /// Обработчик выполнения команды минимизации окна
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private static void MinimizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = (Window)sender;
            window.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Обработчик выполнения команды максимизации или восстановления окна
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private static void MaximizeRestoreExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = (Window)sender;
            if (window.WindowState == WindowState.Normal)
            {
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Обработчик выполнения команды закрытия окна
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private static void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = (Window)sender;
            window.Close();
        }
    }
}
