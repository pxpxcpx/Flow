using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flow.Engine.Models;

namespace Flow.Engine.Abstractions;

public interface IVariableConnection
{
    VariablePosition From { get; set; }

    VariablePosition To { get; set; }

    Type VariableType { get; set; }

    object? Value { get; set; }
}
