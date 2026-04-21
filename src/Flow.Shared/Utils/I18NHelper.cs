using System.Globalization;
using System.Text.Json;

namespace Flow.Shared.Utils;

/// <summary>
/// Helper for text resources' i18n.
/// <example>
/// Here's an example that the text <c>Hello, world</c> will be marked and translated to another language:
/// <code>
/// var helper = new I18NHelper("(path to your resource file)", identifier = "$$I18N:");
/// var text = helper.Translate("$$I18N:Hello, World");
/// </code>
/// "Hello, world" is a key in this example.
/// </example>
/// </summary>
/// <seealso cref="I18NHelper.Translate(string, CultureInfo?)"/>
public class I18NHelper
{
    public string I18NFilePath { get; init; }

    /// <summary>
    /// Used to identify the following text should be translated.
    /// </summary>
    public string Identifier { get; init; }

    public string TargetCulture { get; init; }

    public Dictionary<string, Dictionary<string, string>> I18NDict { get; private set; }

    public I18NHelper(string i18NFilePath, string identifier, CultureInfo? targetCulture)
    {
        I18NFilePath = i18NFilePath;
        I18NDict = new Dictionary<string, Dictionary<string, string>>();
        Identifier = identifier;
        TargetCulture = targetCulture is null ? CultureInfo.CurrentCulture.Name : targetCulture.Name;
    }

    public I18NHelper(Dictionary<string, Dictionary<string, string>> i18NDict, string identifier, string? targetCulture)
    {
        I18NFilePath = "";
        I18NDict = i18NDict;
        Identifier = identifier;
        TargetCulture = targetCulture ?? CultureInfo.CurrentCulture.Name;
    }

    public Task BuildResourceDict(bool refresh = true)
    {
        if (!refresh && I18NDict.Count <= 0 || !File.Exists(I18NFilePath))
            return Task.FromException(new FileNotFoundException($"I18N resource file: {I18NFilePath}"));

        var res = File.ReadAllText(I18NFilePath);
        try
        {
            I18NDict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(res) ?? I18NDict;
        }
        catch
        {
            return Task.FromException(new FileLoadException($"Failed to read I18N resource file {I18NFilePath}"));
        }

        return Task.CompletedTask;
    }

    // Or here is ietf language tag?
    public string Translate(string key, CultureInfo? targetCulture)
        => Translate(key, targetCulture?.Name);

    public string Translate(string key, string? targetCulture)
    {
        if (!I18NDict.TryGetValue(key, out var strRes))
            return key;

        return strRes.GetValueOrDefault(targetCulture ?? TargetCulture, key);
    }
}