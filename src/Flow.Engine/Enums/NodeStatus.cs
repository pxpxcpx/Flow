using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Engine.Enums;

public enum NodeStatus
{
    Ready = 0,
    Running = 1,
    Completed = 2,
    Failed = 3,
    Skipped = 4,
    Canceled = 5
}
