using Flow.Shared.Abstractions;

namespace Flow.Shared.Metadata;

public readonly record struct ProcessPointMetadata(
    int Index, string Name, string Description)
    : IRecognizable
{
    public int Index { get; } = Index;

    public string Name { get; } = Name;

    public string Description { get; } = Description;
}