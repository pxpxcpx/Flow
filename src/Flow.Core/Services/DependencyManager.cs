using Flow.SDK.Dependency.Attributes;
using Flow.Shared.Metadata;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace Flow.Core.Services;

public sealed class DependencyManager
{
    private static DependencyManager? _instance;
    private static readonly object Lock = new();
    private static string? _folderPathTemp;

    /// <summary>
    /// Dependency file extension.
    /// *.fldp: Flow Script Dependency
    /// </summary>
    public const string DependencyExtension = ".fldp";

    /// <summary>
    /// Dependency metadata file name.
    /// </summary>
    public const string DependencyMetadataFileName = "metadata.json";

    /// <summary>
    /// Path to dependency folder.
    /// </summary>
    public string FolderPath { get; init; }

    /// <summary>
    /// Storages dependency metadata and its folder path.
    /// </summary>
    private Dictionary<DependencyMetadata, string> _cache = new();

    private DependencyManager(string path)
    {
        _folderPathTemp = path;
        FolderPath = path;
    }

    /// <summary>
    /// Factory method for <see cref="DependencyManager"/>, which is singleton.
    /// </summary>
    /// <param name="dependencyFolderPath"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public DependencyManager Create(string dependencyFolderPath)
    {
        if (_instance is null)
        {
            lock (Lock)
            {
                _instance ??= new DependencyManager(dependencyFolderPath);
            }
        }
        else
        {
            if (dependencyFolderPath != _folderPathTemp)
                throw new InvalidOperationException("Manager already created with different parameters.");
        }

        return _instance;
    }

    /// <summary>
    /// Refresh all dependencies and their paths to cache.
    /// </summary>
    /// <returns></returns>
    private Task<bool> Refresh()
    {
        if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath))
            return Task.FromResult(false);

        _cache.Clear();
        var success = true;

        foreach (var depPath in Directory.GetDirectories(FolderPath))
        {
            var metadataFile = Path.Combine(FolderPath, DependencyMetadataFileName);
            var m = TryGetMetadata(metadataFile);
            if (m is not { } metadata)
            {
                success = false;
                continue;
            }

            success |= _cache.TryAdd(metadata, metadataFile);
        }

        return Task.FromResult(success);
    }

    private static DependencyMetadata? TryGetMetadata(string metadataFile)
    {
        if (string.IsNullOrEmpty(metadataFile) || !File.Exists(metadataFile))
            return null;

        try
        {
            var text = File.ReadAllText(metadataFile);
            return JsonSerializer.Deserialize<DependencyMetadata>(text);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Load required plugins.
    /// </summary>
    /// <param name="targets">Required plugins</param>
    /// <returns>Dictionary contains metadata and nodes in each dependency.</returns>
    public Task<Dictionary<DependencyMetadata, Type[]?>>? Load(IEnumerable<DependencyMetadata> targets)
    {
        if (_cache is null)
        {
            var b = Refresh().GetAwaiter().GetResult();
            if (!b || _cache is null) // Double check
                return null;
        }

        var result = new Dictionary<DependencyMetadata, Type[]?>();

        foreach (var target in targets)
        {
            if (_cache.TryGetValue(target, out var path))
                LoadDependency(target, path);
        }

        return Task.FromResult(result);

        void LoadDependency(DependencyMetadata metadata, string dirPath)
        {
            // Get the assembly of the dependency, which contains all nodes.
            var a = GetAssembly(metadata.AssemblyPath);
            if (a is not { } assembly)
                return;

            // Get all node types.
            var n = GetAllNodes(assembly);
            if (n is not { } nodes || n.Length == 0)
                return;

            result.Add(metadata, nodes);
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

    private static Type[]? GetAllNodes(Assembly assembly)
    {
        if (assembly is null)
            return null;

        try
        {
            // TODO: Get type implements INode.
            return assembly.GetTypes()
                .Where(type => type.IsDefined(typeof(StaticNodeAttribute), true))
                .ToArray();
        }
        catch
        {
            return null;
        }
    }

    public Task<bool> InstallDependency(string path, bool refresh = true)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path) || !path.EndsWith(DependencyExtension))
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

        Refresh();

        return Task.FromResult(true);
    }

    public Task<bool> UninstallDependency(string sourcePath, bool refresh = true)
    {
        if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath) || !sourcePath.EndsWith(DependencyExtension))
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

        Refresh();

        return Task.FromResult(true);
    }
}
