using Flow.Shared.Abstractions;

namespace Flow.Shared.Metadata;

public record struct ProcessPointMetadata : IRecognizable
{
    public ProcessPointMetadata(int Index, string Name, string Description)
    {
        this.Index = Index;
        this.Name = Name;
        this.Description = Description;
    }

    public int Index { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public readonly void Deconstruct(out int index, out string name, out string description)
    {
        index = Index;
        name = Name;
        description = Description;
    }
}