using Flow.Shared.Abstractions;

namespace Flow.Shared.Metadata;

public record struct ProcessPointMetadata : IDescribable
{
    public ProcessPointMetadata(int Index, string Name, string Description)
    {
        this.Index = Index;
        this.Identifier = Name;
        this.Description = Description;
    }

    public int Index { get; set; }

    public string Identifier { get; set; }

    public string Description { get; set; }

    public readonly void Deconstruct(out int index, out string name, out string description)
    {
        index = Index;
        name = Identifier;
        description = Description;
    }
}