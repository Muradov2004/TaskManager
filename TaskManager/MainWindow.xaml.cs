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
    private Timer blackListTimer;
    public ObservableCollection<string> blackListProcesses { get; set; }
    public ObservableCollection<ProcessItem> processItems { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        blackListProcesses = new();
        processItems = new();
        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            processItems.Add(new ProcessItem() { ProcessId = process.Id, ProcessName = process.ProcessName, ProcessThreadCount = process.Threads.Count, Handle = process.HandleCount });
        }
        TasksList.ItemsSource = processItems;
        BlackList.ItemsSource = blackListProcesses;

        processTimer = new Timer(1000);
        processTimer.Elapsed += ProcessTimer_Elapsed!;
        processTimer.AutoReset = true;
        blackListTimer = new Timer(2000);
        blackListTimer.Elapsed += blackListTimer_Elapsed!;
        blackListTimer.AutoReset = true;
    }

    private void StopProcess_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var process = Process.GetProcessById(processItems[TasksList.SelectedIndex].ProcessId);
            process.Kill();
            TasksList.SelectedItem = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Process not found. Process stopped or something went wrong \nError message : " + ex.Message);
        }
    }

    private void RemoveBlackList_Click(object sender, RoutedEventArgs e) => blackListProcesses.Remove(blackListProcesses[BlackList.SelectedIndex]);

    private void AddBlackList_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var process = processItems[TasksList.SelectedIndex];
            blackListProcesses.Add(process.ProcessName!);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Process not found. Process stopped or something went wrong \nError message : " + ex.Message);
        }
    }
    private void Unselect_Click(object sender, RoutedEventArgs e)
    {
        TasksList.SelectedItem = null;
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

    private void blackListTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        Dispatcher.Invoke(() =>
            processItems.Where(proc => blackListProcesses.Contains(proc.ProcessName!)).ToList().ForEach(bproc => Process.GetProcessById(bproc.ProcessId).Kill())
        );
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) => processTimer.Start();

    private void RefreshProcessList()
    {
        if (TasksList.SelectedItem is null)
        {

            processItems.Clear();

            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                processItems.Add(new ProcessItem() { ProcessId = process.Id, ProcessName = process.ProcessName, ProcessThreadCount = process.Threads.Count, Handle = process.HandleCount });

            }
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
