using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Linq;

namespace TaskManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Timer processTimer;
    public ObservableCollection<ProcessItem> blackListItems { get; set; }
    public ObservableCollection<ProcessItem> processItems { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        blackListItems = new();
        processItems = new();
        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            processItems.Add(new ProcessItem() { ProcessId = process.Id, ProcessName = process.ProcessName, ProcessThreadCount = process.Threads.Count, Handle = process.HandleCount });
        }
        TasksList.ItemsSource = processItems;
        BlackList.ItemsSource = blackListItems;

        processTimer = new Timer(5000); 
        processTimer.Elapsed += ProcessTimer_Elapsed!;
        processTimer.AutoReset = true;
    }

    private void StopProcess_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var process = Process.GetProcessById(processItems[TasksList.SelectedIndex].ProcessId);
            process.Kill();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Not found process. Process stopped or something went wrong \nError message : " + ex.Message);
        }
    }

    private void RemoveBlackList_Click(object sender, RoutedEventArgs e) => blackListItems.Remove(blackListItems[BlackList.SelectedIndex]);

    private void AddBlackList_Click(object sender, RoutedEventArgs e)
    {
        var process = processItems[TasksList.SelectedIndex];
        blackListItems.Add(process);
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

    private void ProcessTimer_Elapsed(object sender, ElapsedEventArgs e) => Dispatcher.Invoke(() => RefreshProcessList());

    private void Window_Loaded(object sender, RoutedEventArgs e) => processTimer.Start();

    private void RefreshProcessList()
    {
        processItems.Clear();

        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            processItems.Add(new ProcessItem() { ProcessId = process.Id, ProcessName = process.ProcessName, ProcessThreadCount = process.Threads.Count, Handle = process.HandleCount });

        }

        //var blacklistedProcess = blackListItems.FirstOrDefault(p => p.ProcessId == process.Id);
        //if (blacklistedProcess != null)
        //{
        //    var timer = new Timer(5000);
        //    timer.Elapsed += (sender, e) =>
        //    {
        //        timer.Stop();
        //        process.Kill();
        //        Dispatcher.Invoke(() =>
        //        {
        //            LogTextBox.AppendText($"Blacklisted process {process.ProcessName} (PID: {process.Id}) has been terminated.{Environment.NewLine}");
        //        });
        //    };
        //    timer.Start();
        //}

    }


}
