namespace Flow.Tests.Feasibility.StaticNodeV1;

public class AddNode
{
    public static readonly Dictionary<string, Type> ParamTypesDictionary = new()
    {
        {nameof(ParamA), typeof(int)},
        {nameof(ParamB), typeof(int)},
    };

    #region Inputs & Output

    public int ParamA { get; private set; }
    
    public int ParamB { get; private set; }
    
    public int Result { get; private set; }
    
    #endregion
    
    public void Invoke()
    {
        Result = StaticNodeExample.Add(ParamA, ParamB);
    }

    public void SetValue<T>(string name, T value)
    {
        if (!ParamTypesDictionary.TryGetValue(name, out var type)) return;
        if (typeof(T) != type) return;

        var nodeType = typeof(AddNode);
        
        foreach (var prop in nodeType.GetProperties())
        {
            if (prop.Name != name) continue;
            prop.SetValue(this, value);
        }
    }
}