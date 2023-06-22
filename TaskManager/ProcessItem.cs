namespace TaskManager;

public class ProcessItem
{
    public int ProcessId { get; set; }
    //public Icon? ProcessIcon { get; set; }
    public string? ProcessName { get; set; }
    public override string ToString() => $"{ProcessId} {ProcessName}";
}
