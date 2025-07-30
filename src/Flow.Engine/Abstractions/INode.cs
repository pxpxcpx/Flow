using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flow.API;
using Flow.API.Node;
using Flow.Engine.Enums;

namespace Flow.Engine.Abstractions;

public interface INode : IExecutable, IDescribable
{
    Guid Id { get; init; }

    NodeStatus Status { get; set; }
    
    Dictionary<int, string> Inputs { get; set; }
    
    Dictionary<int, string> Outputs { get; set; }

    IExecutable Executable { get; set; }
}
