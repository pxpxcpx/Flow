using Flow.Engine.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Engine.Abstractions;

public interface IProcessConnection
{
    NodePosition From { get; set; }

    NodePosition To { get; set; }
}
