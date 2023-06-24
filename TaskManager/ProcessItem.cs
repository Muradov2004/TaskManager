using System;

namespace TaskManager;

public class ProcessItem
{
    public int ProcessId { get; set; }
    //public Icon? ProcessIcon { get; set; }
    public string? ProcessName { get; set; }
    public int ProcessThreadCount { get; set; }
    public int Handle { get; set; }
    public override string ToString() => ProcessName!;
}
