using Flow.Shared.Abstractions;

namespace Flow.Shared.Metadata;

/// <summary>
/// Metadata of the plugin.
/// </summary>
/// <example>
/// To create a metadata for a plugin:
/// <br/>
/// 1. Add a pair of GUID and <see cref="NodeMetadata"/> to a dictionary.
/// <code>
/// var dict = new Dictionary&lt;Guid, NodeMetadata&gt;
/// {
///     {Guid.Parse("Guid 1"), Metadata1},
///     {Guid.Parse("Guid 2"), Metadata2},
///     {Guid.Parse("Guid 3"), Metadata3},
///     ...
/// };
/// </code>
/// or use TODO: Utils to create metadata dictionary.
/// <code>
/// var dict = PluginHelper.CollectNodeMetadata(Assembly.GetExecutingAssembly());
/// </code>
/// <br/>
/// 2. Create a metadata like this:
/// <code>
/// var metadata = new PluginMetadata
/// {
///     Guid = Guid.Parse("Your GUID here");
///     Version = new Version(0,0,0,0);
///     Name = "Your plugin name";
///     Description = "Description to the plugin";
///     Nodes = dict;
///     Author = "Flow";
///     PluginPath = "./Plugin.dll";
///     AssetsPath = "./Assets/"
/// };
/// </code>
/// <br/>
/// 3. Parse this metadata into JSON and save it as "Plugin.json" in the root directory.
/// </example>
public record PluginMetadata : IRecognizable
{
    /// <summary>
    /// ID of the plugin.
    /// <b>Do NOT regenerate</b> or modify during the plugin lifecycle.
    /// </summary>
    public required Guid Guid { get; set; }
    
    /// <summary>
    /// Version of the plugin.
    /// </summary>
    public required Version Version { get; set; }
    
    /// <summary>
    /// Name of the plugin.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Description of the plugin.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Author of the plugin.
    /// </summary>
    public string Author { get; set; } = string.Empty;
    
    /// <summary>
    /// Plugins that the plugin required and depends on.
    /// </summary>
    public required PluginMetadata[] Dependencies { get; set; }
    
    /// <summary>
    /// Dictionary to pair a node and its GUID.
    /// </summary>
    public required Dictionary<Guid, NodeMetadata> Nodes { get; set; } = new();
    
    /// <summary>
    /// Path to the plugin assembly file (*.dll).
    /// </summary>
    public required string PluginPath { get; set; }
    
    /// <summary>
    /// Path to the plugin's assets' folder.
    /// </summary>
    public required string AssetsPath { get; set; }
}