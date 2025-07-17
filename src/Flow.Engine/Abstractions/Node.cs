using Flow.API;
using Flow.Engine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Engine.Abstractions;

internal class Node : INode
{
    public Guid Id { get; init; }
    public string Description { get; set; }
    public string Name { get; set; }
    public NodeStatus Status { get; set; }
    public IExecutable Executable { get; set; }

    public Node():this(string.Empty)
    {
    }

    public Node(string description)
    {
        Id = Guid.NewGuid();
        Description = description;
    }

    public void Execute()
    {
        throw new NotImplementedException();
    }
}
