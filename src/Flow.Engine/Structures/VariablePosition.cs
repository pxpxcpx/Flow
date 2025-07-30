using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Engine.Structures;

public record struct VariablePosition
{
    public int Position { get; set; }

    public Guid Node { get; set; }
}
