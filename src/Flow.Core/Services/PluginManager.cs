using Flow.Shared.Metadata;
using Flow.Shared.Models;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace Flow.Core.Services;

public sealed class PluginManager : IDisposable
{
    private static PluginManager? _instance;
    private static readonly object Lock = new();
    private static string? _folderPathTemp;

    public ReadOnlyDictionary<PluginMetadata, Plugin> PluginDict { get; private set; }

    public string FolderPath { get; init; }

    public IServiceProvider ServiceProvider { get; init; }

    /// <summary>
    /// Plugins' extension.
    /// *.flap: Flow App Plugin
    /// </summary>
    public const string PluginExtension = ".flap";

    /// <summary>
    /// Hash code file extension.
    /// </summary>
    public const string PluginHashFileExtension = "hashcode";

    /// <summary>
    /// File name of an app plugin.
    /// </summary>
    public const string PluginMetadataFileName = "metadata.json";

    private PluginManager(string path, IServiceProvider serviceProvider)
    {
        _folderPathTemp = path;
        FolderPath = path;
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Factory method for <see cref="PluginManager"/>, which is singleton.
    /// </summary>
    /// <param name="pluginFolderPath"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static PluginManager Create(string pluginFolderPath, IServiceProvider serviceProvider)
    {
        if (_instance is null)
        {
            lock (Lock)
            {
                _instance ??= new PluginManager(pluginFolderPath, serviceProvider);
            }
        }
        else
        {
            if (pluginFolderPath != _folderPathTemp)
                throw new InvalidOperationException("Manager already created with different parameters.");
        }

        return _instance;
    }

    /// <summary>
    /// Load all plugins.
    /// Used to be called at the service started.
    /// </summary>
    /// <returns></returns>
    public Task LoadPlugins()
    {
        if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath))
            return Task.CompletedTask;

        var dict = new Dictionary<PluginMetadata, Plugin>();
        foreach (var pluginFolder in Directory.GetDirectories(FolderPath))
            LoadPlugin(pluginFolder);

        PluginDict = new ReadOnlyDictionary<PluginMetadata, Plugin>(dict);
        return Task.CompletedTask;

        void LoadPlugin(string dirPath)
        {
            // Metadata
            var metadataFile = Path.Combine(dirPath, PluginMetadataFileName);
            var m = TryGetMetadata(metadataFile);
            if (m is not { } metadata)
                return;

            // Main assembly of the plugin
            var a = GetAssembly(metadata.AssemblyPath);
            if (a is not { } assembly)
                return;

            // Get the plugin main class.
            var pluginType = assembly.GetType(metadata.ClassType.ToString());
            if (pluginType is null || !typeof(Plugin).IsAssignableFrom(pluginType))
                return;

            // Reflection
            var i = CreateInstance(pluginType);
            if (i is not Plugin plugin)
                return;

            dict.Add(plugin.Metadata, plugin);
        }
    }

    private static PluginMetadata? TryGetMetadata(string metadataFile)
    {
        if (string.IsNullOrEmpty(metadataFile) || !File.Exists(metadataFile))
            return null;

        try
        {
            var text = File.ReadAllText(metadataFile);
            return JsonSerializer.Deserialize<PluginMetadata>(text);
        }
        catch
        {
            return null;
        }
    }

    private static Assembly? GetAssembly(string assemblyFullPath)
    {
        if (string.IsNullOrEmpty(assemblyFullPath) || !File.Exists(assemblyFullPath))
            return null;

        try
        {
            return Assembly.LoadFile(assemblyFullPath);
        }
        catch
        {
            return null;
        }
    }

    private static object? CreateInstance(Type type)
        => Activator.CreateInstance(type);

    public Task InitializeAll()
    {
        foreach (var plugin in PluginDict)
        {
            for (var i = 0; i < plugin.Value.Metadata.Dependencies.Count(); i++)
            {
                var service = ServiceProvider.GetService(plugin.Value.Metadata.Dependencies.ElementAt(i));
                plugin.Value.Dependencies?[i] = service;
            }

            plugin.Value.Initialize();
        }

        return Task.CompletedTask;
    }

    // TODO: Operations after modifying plugin.
    public Task<bool> InstallPlugin(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path) || !path.EndsWith(PluginExtension))
            return Task.FromResult(false);

        try
        {
            using var zip = ZipFile.OpenRead(path);
            zip.ExtractToDirectory(FolderPath);
        }
        catch (Exception exception)
        {
            return Task.FromException<bool>(exception);
        }

        return Task.FromResult(true);
    }

    public Task<bool> UninstallPlugin(string sourcePath)
    {
        if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath) || !sourcePath.EndsWith(PluginExtension))
            return Task.FromResult(false);

        try
        {
            var name = Path.GetFileName(sourcePath);
            Directory.Delete(sourcePath + '\\' + name, true);
        }
        catch (Exception exception)
        {
            return Task.FromException<bool>(exception);
        }

        return Task.FromResult(true);
    }

    /// <summary>
    /// Dispose manager and managed plugin resources.
    /// </summary>
    public void Dispose()
    {
        foreach (var plugin in PluginDict)
            plugin.Value.Dispose();
    }
}