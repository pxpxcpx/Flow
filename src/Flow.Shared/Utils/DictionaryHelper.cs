namespace Flow.Shared.Utils;

/// <summary>
/// Quick operations for <see cref="Dictionary{TKey,TValue}"/>.
/// </summary>
/// <remarks>NOT applicable to ConcurrentDictionary.</remarks>
public static class DictionaryHelper
{
    /// <summary>
    /// Update a key in the dict while keeping its value unchanged.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="newKey"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">The key doesn't exist.</exception>
    public static Dictionary<TKey, TValue> UpdateKey<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, TKey key, TKey newKey)
        where TKey : notnull
    {
        if (!dictionary.Remove(key, out var value))
            throw new KeyNotFoundException($"Key {key} not found.");

        dictionary.Add(newKey, value);
        
        return dictionary;
    }

    /// <summary>
    /// Update a key in the dict using <see cref="updateFunc"/> while keeping its value unchanged.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="updateFunc"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">The key doesn't exist.</exception>
    public static Dictionary<TKey, TValue> UpdateKey<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TKey> updateFunc)
        where TKey : notnull
    {
        if (!dictionary.Remove(key, out var value))
            throw new KeyNotFoundException($"Key {key} not found.");

        var newKey = updateFunc(key);
        dictionary.Add(newKey, value);
        
        return dictionary;
    }

    /// <summary>
    /// Update the value with its key.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">The key doesn't exist.</exception>
    public static Dictionary<TKey, TValue> UpdateValue<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        if (!dictionary.ContainsKey(key))
            throw new KeyNotFoundException($"Key {key} not found.");

        dictionary[key] = value;

        return dictionary;
    }

    /// <summary>
    /// Update a value with <see cref="updateFunc"/>.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="updateFunc"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">The key doesn't exist.</exception>
    public static Dictionary<TKey, TValue> UpdateValue<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> updateFunc)
        where TKey : notnull
    {
        if (!dictionary.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"Key {key} not found.");

        var newValue = updateFunc(value);
        dictionary[key] = newValue;

        return dictionary;
    }
}