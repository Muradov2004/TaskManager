using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Wpf.Ui.Controls;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        public ObservableCollection<ProcessItem> processItems { get; set; } = new ObservableCollection<ProcessItem>();

        public MainWindow()
        {
            InitializeComponent();
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            uiwindow.MaxHeight = screenHeight - 40;
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                processItems.Add(new() { ProcessIcon = null, ProcessName = process.ProcessName });
            }
            TasksList.ItemsSource = processItems;
        }
    }
}
