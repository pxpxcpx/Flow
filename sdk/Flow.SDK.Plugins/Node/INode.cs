namespace Flow.SDK.Plugins.Node;

public interface INode : IExecutable, IRecognizable
{
    Guid Guid { get; }
    
    Dictionary<string, Type> ParamTypes { get; }
    
    void SetValue<T>(string param, T value);
}
