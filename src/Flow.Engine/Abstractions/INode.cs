using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flow.API;
using Flow.Engine.Enums;

namespace Flow.Engine.Abstractions;

public interface INode : IExecutable
{
    Guid Id { get; init; }

    string Name { get; set; }

    NodeStatus Status { get; set; }

    IExecutable Executable { get; set; }

    string Description { get; set; }
}
