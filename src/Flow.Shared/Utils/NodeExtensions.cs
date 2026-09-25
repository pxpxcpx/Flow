using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Models;

namespace Flow.Shared.Utils;

public static class NodeExtensions
{
    /// <summary>
    /// Find the state by related binary/int code.
    /// </summary>
    public static NodeStates ExtractNodeState(int code)
        => code switch
        {
            1 => NodeStates.Idle,
            2 => NodeStates.Ready,
            4 => NodeStates.Running,
            8 => NodeStates.Suspended,
            16 => NodeStates.Finished,
            32 => NodeStates.Unspecified,
            64 => NodeStates.Faulted,
            128 => NodeStates.Successful,
            _ => NodeStates.None
        };

    /// <summary>
    /// Find the event by related binary/int code.
    /// </summary>
    public static NodeEvents ExtractNodeEvent(int code)
        => code switch
        {
            1 => NodeEvents.Initialize,
            2 => NodeEvents.Start,
            3 => NodeEvents.Suspend,
            4 => NodeEvents.Pause,
            5 => NodeEvents.Resume,
            6 => NodeEvents.Cancel,
            7 => NodeEvents.Complete,
            8 => NodeEvents.Reset,
            16 => NodeEvents.Unspecified,
            32 => NodeEvents.Success,
            48 => NodeEvents.Fail,
            _ => NodeEvents.None
        };
    
    /// <summary>
    /// Default state transition of common nodes.
    /// </summary>
    /// <param name="pr">Current state(before the transision).</param>
    /// <param name="event">Event received.</param>
    /// <returns></returns>
    public static NodeStates DefaultStateTransform(NodeStates pr, NodeEvents @event)
    {
        NodeStates state;
        var lifeCycleEvent = @event.ExtractFieldIn(NodeEvents.LifecycleMask, ExtractNodeEvent);
        var procEvent = @event.ExtractFieldIn(NodeEvents.ResultMask, ExtractNodeEvent);
        var lifecycleState = pr.ExtractFieldIn(NodeStates.LifecycleMask, ExtractNodeState);

        if (lifeCycleEvent == NodeEvents.Reset)
        {
            state = lifecycleState.HasFlag(NodeStates.Idle)
                ? NodeStates.Idle | NodeStates.Unspecified // Not initialized yet
                : NodeStates.Ready | NodeStates.Unspecified; // Already initialized
            return state;
        }

        var l = (lifecycleState, lifeCycleEvent) switch
        {
            // * --None--> *
            (_, NodeEvents.None) => pr,
            // Idle -> *
            (NodeStates.Idle, NodeEvents.Initialize) => NodeStates.Ready,
            // Ready -> *
            (NodeStates.Ready, NodeEvents.Start) => NodeStates.Running,
            (NodeStates.Ready, NodeEvents.Suspend) => NodeStates.Suspended,
            // Running -> *
            (NodeStates.Running, NodeEvents.Pause) => NodeStates.Suspended,
            (NodeStates.Running, NodeEvents.Suspend) => NodeStates.Suspended,
            (NodeStates.Running, NodeEvents.Cancel) => NodeStates.Finished,
            (NodeStates.Running, NodeEvents.Complete) => NodeStates.Finished,
            // Suspended -> *
            (NodeStates.Suspended, NodeEvents.Resume) => NodeStates.Running,
            (NodeStates.Suspended, NodeEvents.Cancel) => NodeStates.Finished,
            _ => NodeStates.None
        };

        var r = procEvent switch
        {
            NodeEvents.Unspecified => NodeStates.Unspecified,
            NodeEvents.Success => NodeStates.Successful,
            NodeEvents.Fail => NodeStates.Faulted,
            _ => NodeStates.Unspecified
        };

        state = l | r;
        return state;
    }

    // So why is this indentation so wired? XD
    /// <summary>
    /// Static extension methods for <see cref="INode"/> objects.
    /// </summary>
    /// <param name="node">Node object implements <see cref="INode"/>.</param>
    /// <typeparam name="TNode">Specific type of node.</typeparam>
    extension<TNode>(TNode node)
        where TNode : INode
    {
        /// <summary>
        /// Get the snapshot of input arguments of node at the specific time.
        /// </summary>
        /// <returns></returns>
        public object?[]? Snapshot()
            => node.Inputs?.Clone() as object?[];

        public object? GetInput(int index)
            => Node.IsNullOrIndexOutOfRange(node.Inputs, index) ? null : node.Inputs![index];

        /// <summary>
        /// Get the input and convert to specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns>True if success, otherwise false.</returns>
        public bool GetInput<T>(int index, out T? value)
        {
            var o = GetInput(node, index);

            try
            {
                value = (T?)o;
                return true;
            }
            catch (InvalidCastException)
            {
                value = default;
            }

            return false;
        }

        public object? GetOutput(int index)
            => Node.IsNullOrIndexOutOfRange(node.Outputs, index) ? null : node.Outputs![index];

        public bool GetOutput<T>(int index, out T? value)
        {
            var o = GetOutput(node, index);

            try
            {
                value = (T?)o;
                return true;
            }
            catch (InvalidCastException)
            {
                value = default;
            }

            return false;
        }

        /// <summary>
        /// Pass a parameter to the input of the node.
        /// </summary>
        /// <param name="index">Input parameter's index.</param>
        /// <param name="value">Value to pass.</param>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>True if successful, otherwise, false.</returns>
        public bool Assign<TValue>(int index, TValue? value)
        {
            if (Node.IsNullOrIndexOutOfRange(node.InputVariableMetadata, index) ||
                Node.IsNullOrIndexOutOfRange(node.Inputs, index))
                return false;

            var md = node.InputVariableMetadata![index];
            var type = md.Type;
            if (typeof(TValue) != type)
                return false;

            node.Inputs![index] = value;
            return true;
        }

        /// <summary>
        /// Pass a parameter to the input of the node.
        /// </summary>
        /// <remarks>Will NOT check the type of the value.</remarks>
        /// <param name="index">Input parameter's index.</param>
        /// <param name="value">Value to pass.</param>
        /// <returns>True if successful, otherwise, false.</returns>
        public bool Assign(int index, object? value)
        {
            if (Node.IsNullOrIndexOutOfRange(node.InputVariableMetadata, index) ||
                Node.IsNullOrIndexOutOfRange(node.Inputs, index))
                return false;

            node.Inputs![index] = value;
            return true;
        }

        public bool SetToDefaultValue(int index)
        {
            if (Node.IsNullOrIndexOutOfRange(node.InputVariableMetadata, index) ||
                Node.IsNullOrIndexOutOfRange(node.Inputs, index))
                return false;

            var md = node.InputVariableMetadata![index];
            if (md.DefaultValue is not { } dv)
                return false;

            node.Inputs![index] = dv;
            return true;
        }

        public IEnumerable<ParameterMetadata> GetUnfilledRequiredValues()
        {
            if (node.InputVariableMetadata is null || node.Inputs is null)
                yield break;

            for (var i = 0; i < node.InputVariableMetadata.Length; i++)
            {
                var metadata = node.InputVariableMetadata[i];

                if (!metadata.IsRequired)
                    continue;

                if (node.Inputs[i] is null && metadata.DefaultValue is null)
                    yield return metadata;
            }
        }
    }
}