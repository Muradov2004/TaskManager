using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using System.Windows.Interop;
using System.Windows.Controls;

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
            TasksList.SelectedItem = null;
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
            processItems.Where(proc => blackListProcesses.Contains(proc.ProcessName!)).ToList()
                        .ForEach(bproc => Process.GetProcessById(bproc.ProcessId).Kill())
        );
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        processTimer.Start();
        blackListTimer.Start();
    }

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
    }

    private void ButtonClose_Click(object sender, RoutedEventArgs e) => Close();

    bool isMaximum = false, isFull = false;
    Point old_loc, default_loc;
    Size old_size, default_size;

    private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

    private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
    {

        if (isMaximum == false) // app is currently not maximized ; then maximize it!
        {
            old_size = new Size(Width, Height);
            old_loc = new Point(Top, Left);
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            WindowState = WindowState.Normal;
            Top = 0;
            Left = 0;
            Width = x;
            Height = y;
            isMaximum = true;
            isFull = false;
        }

        else // app is currentlly maximized ; then we make it normal
        {
            if (old_size.Width >= SystemParameters.WorkArea.Width ||
                old_size.Height >= SystemParameters.WorkArea.Height)
            {
                Top = default_loc.Y;
                Left = default_loc.X;
                Width = default_size.Width;
                Height = default_size.Height;
            }

            else
            {
                Top = old_loc.Y;
                Left = old_loc.X;
                Width = old_size.Width;
                Height = old_size.Height;
            }
            isMaximum = false;
            isFull = false;
        }

    }

    private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
}
