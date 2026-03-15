using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Models;

namespace Flow.Shared.Metadata;

public record AppPluginMetadata : IRecognizable
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
    /// Use <b>MD5</b> of the assembly file.
    /// </summary>
    public required string Hash { get; set; }
    
    /// <summary>
    /// Describes what kind of contents are contained. 
    /// </summary>
    public required AppPluginType AppPluginType { get; set; }

    /// <summary>
    /// Author of the plugin.
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Path to the plugin assembly file (*.dll).
    /// </summary>
    public required string AssemblyPath { get; set; } = "./Assembly/Plugin.dll";

    /// <summary>
    /// Path to the plugin's assets' folder.
    /// </summary>
    public required string AssetsPath { get; set; } = "./Assets/";

    /// <summary>
    /// Path to the settings JSON file.
    /// </summary>
    public required string SettingsFilePath { get; set; } = "./Settings/Settings.json";
}