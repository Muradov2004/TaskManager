using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager;

public class ProcessItem
{
    public Icon? ProcessIcon { get; set; }
    public string? ProcessName { get; set; }
    public override string ToString() => $"{ProcessName}";
}
