using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Models;

namespace Flow.Shared.Utils;

public static class NodeExtensions
{
    // So why is this indentation so wired? XD
    extension(INode node)
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

        public void MarkAs(NodeStatus status)
            => node.Status = status;
    }
}