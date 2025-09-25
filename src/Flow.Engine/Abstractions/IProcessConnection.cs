using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flow.Engine.Models;

namespace Flow.Engine.Abstractions;

public interface IProcessConnection
{
    NodePosition From { get; set; }

    NodePosition To { get; set; }
}
