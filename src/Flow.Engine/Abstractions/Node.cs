using Flow.API;
using Flow.Engine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flow.API.Node;

namespace Flow.Engine.Abstractions;

internal class Node : INode
{
    public Guid Id { get; init; }
    public string Description { get; set; }
    public string Name { get; set; }
    
    public NodeStatus Status { get; set; }
    
    public Dictionary<int, string> Inputs { get; set; }
    public Dictionary<int, string> Outputs { get; set; }
    
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
