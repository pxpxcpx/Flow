using Flow.Engine.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Engine.Abstractions;

public interface IVariableConnection
{
    VariblePosition From { get; set; }

    VariblePosition To { get; set; }

    Type VariableType { get; set; }

    object? Value { get; set; }
}
