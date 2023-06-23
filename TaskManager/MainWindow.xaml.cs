using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace TaskManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public ObservableCollection<ProcessItem> processItems { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        processItems = new();
        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            processItems.Add(new ProcessItem() { ProcessId = process.Id, ProcessName = process.ProcessName, ProcessThreadCount = process.Threads.Count, Handle = process.HandleCount });
        }
        TasksList.ItemsSource = processItems;
    }

    private void StopProcess_Click(object sender, RoutedEventArgs e)
    {

    }

    private void RemoveBlackList_Click(object sender, RoutedEventArgs e)
    {

    }

    private void AddBlackList_Click(object sender, RoutedEventArgs e)
    {

    }

    private void StartProcessButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(ProcessNameTxtBox.Text);
        }
        catch (Exception ex)
        {

            MessageBox.Show("INCORRECT NAME \nError message : " + ex.Message);
        }
        finally
        {
            ProcessNameTxtBox.Clear();
        }
    }
}
