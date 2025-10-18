using System.Collections.Concurrent;

namespace Flow.Runtime.ContextManager;

public class ContextManager<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// A dictionary that holds context objects categorized by context names and their types.
    /// </summary>
    /// <remarks>
    /// Key: Instance of <see cref="TKey"/>,
    /// value: Dictionary of <see cref="Object">context object</see> and its <see cref="Type"/>
    /// </remarks>
    public ConcurrentDictionary<TKey, ContextItem?> Contexts { get; }

    public ContextManager()
        : this(new ConcurrentDictionary<TKey, ContextItem?>()) { }

    public ContextManager(ConcurrentDictionary<TKey, ContextItem?> contexts)
        => Contexts = contexts ?? throw new ArgumentNullException(nameof(contexts), "Contexts cannot be null.");

    #region CRUD Operations
    
    /// <summary>
    /// Try to add a <see cref="ContextItem"/> to the context manager.
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="contextItem">Value</param>
    /// <param name="force">Forced to add item, may result in data overwriting.</param>
    /// <returns>True if added successfully, otherwise false</returns>
    public bool TryAddContext(TKey key, ContextItem? contextItem, bool force = false)
    {
        if (force)
            Contexts.TryRemove(key, out _);
        
        return Contexts.TryAdd(key, contextItem);
    }

    /// <inheritdoc cref="TryAddContext(TKey,ContextItem?,bool)"/>
    public bool TryAddContext(TKey key, Type contextType, object contextValue, bool force = false)
    {
        var contextItem = new ContextItem(contextType, contextValue);
        return TryAddContext(key, contextItem, force);
    }
    
    /// <summary>
    /// Find a <see cref="ContextItem"/> by its key.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Throws when the key does not exist in context.</exception>
    private ContextItem? FindContextOrThrow(TKey key)
    {
        if (!Contexts.TryGetValue(key, out var item))
            throw new KeyNotFoundException($"[WARN] CM : Context {key} not found.");

        return item;
    }
    
    /// <summary>
    /// Try to find a <see cref="ContextItem"/> by its key.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public ContextItem? TryFindContextItem(TKey key) => FindContextOrThrow(key);
        
    /// <summary>
    /// Try to find a <see cref="ContextItem.Type"/> by its key.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public Type? TryFindContextType(TKey key) => FindContextOrThrow(key)?.Type;

    /// <summary>
    /// Try to find a <see cref="ContextItem.Value"/> by its key.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    public object? TryFindContextObject(TKey key) => FindContextOrThrow(key)?.Value;

    /// <summary>
    /// Update <see cref="ContextItem"/>by its key.
    /// </summary>
    /// <returns>True if updated successfully, otherwise false</returns>
    public bool UpdateContent(TKey key, ContextItem? newItem)
    {
        var oldItem = FindContextOrThrow(key);
        return Contexts.TryUpdate(key, newItem, oldItem);
    }

    /// <inheritdoc cref="UpdateContent(TKey,ContextItem?)"/>
    public bool UpdateContent(TKey key, Type type, object value)
    {
        var newItem = new ContextItem(type, value);
        return UpdateContent(key, newItem);
    }

    /// <summary>
    /// Update key.
    /// </summary>
    /// <returns>True if updated successfully, otherwise false</returns>
    public bool UpdateKey(TKey oldKey, TKey newKey)
    {
        var item = FindContextOrThrow(oldKey);
        
        if (TryRemoveContext(oldKey))
            return Contexts.TryAdd(newKey, item);
        
        return false;
    }
    
    /// <summary>
    /// Remove <see cref="ContextItem"/> by its key.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>True if removed successfully, otherwise false</returns>
    public bool TryRemoveContext(TKey key) => Contexts.TryRemove(key, out _);
    
    #endregion

    public bool DisposeObject(TKey key)
    {
        var item = FindContextOrThrow(key);
        if (item?.Value is IDisposable disposable)
        {
            disposable.Dispose();
            return true;
        }
        return false;
    }
}