using System.Security.Cryptography;
using System.Text;

namespace Flow.Shared.Utils;

// AI generated, not yet reviewed
/// <summary>
/// Provides static methods for computing hashes from various sources and merging hash values.
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Computes the hash of a string using the specified algorithm (default SHA256).
    /// </summary>
    /// <param name="input">The input string (UTF8 encoding).</param>
    /// <param name="algorithm">The hash algorithm to use; if null, SHA256 is used.</param>
    /// <returns>The computed hash as a byte array.</returns>
    public static byte[] Hash(string input, HashAlgorithm? algorithm = null)
    {
        ArgumentNullException.ThrowIfNull(input);

        using var alg = algorithm ?? SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        return alg.ComputeHash(inputBytes);
    }

    /// <summary>
    /// Computes the hash of a byte array using the specified algorithm (default SHA256).
    /// </summary>
    /// <param name="input">The input byte array.</param>
    /// <param name="algorithm">The hash algorithm to use; if null, SHA256 is used.</param>
    /// <returns>The computed hash as a byte array.</returns>
    public static byte[] Hash(byte[] input, HashAlgorithm? algorithm = null)
    {
        ArgumentNullException.ThrowIfNull(input);

        using var alg = algorithm ?? SHA256.Create();
        return alg.ComputeHash(input);
    }

    /// <summary>
    /// Computes the hash of a stream using the specified algorithm (default SHA256).
    /// </summary>
    /// <param name="input">The input stream (must be readable and seekable).</param>
    /// <param name="algorithm">The hash algorithm to use; if null, SHA256 is used.</param>
    /// <returns>The computed hash as a byte array.</returns>
    public static byte[] Hash(Stream input, HashAlgorithm? algorithm = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (!input.CanRead)
            throw new ArgumentException("Stream must be readable.", nameof(input));

        using var alg = algorithm ?? SHA256.Create();
        return alg.ComputeHash(input);
    }

    /// <summary>
    /// Computes the hash of a file using the specified algorithm (default SHA256).
    /// </summary>
    /// <param name="filePath">The full path to the file.</param>
    /// <param name="algorithm">The hash algorithm to use; if null, SHA256 is used.</param>
    /// <returns>The computed hash as a byte array.</returns>
    public static byte[] HashFile(string filePath, HashAlgorithm? algorithm = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        using var alg = algorithm ?? SHA256.Create();
        using var stream = File.OpenRead(filePath);
        return alg.ComputeHash(stream);
    }

    /// <summary>
    /// Computes the combined hash of all files in a folder (optionally recursive) using the specified algorithm (default SHA256).
    /// Files are processed in a deterministic order (sorted by relative path) to ensure consistent results.
    /// </summary>
    /// <param name="folderPath">The path to the folder.</param>
    /// <param name="algorithm">The hash algorithm to use; if null, SHA256 is used.</param>
    /// <param name="recursive">If true, includes files in subfolders.</param>
    /// <returns>The computed hash as a byte array, which is the merged hash of all individual file hashes.</returns>
    public static byte[] HashFolder(string folderPath, HashAlgorithm? algorithm = null, bool recursive = true)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("Folder path cannot be null or empty.", nameof(folderPath));
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = Directory.GetFiles(folderPath, "*", searchOption);

        // Sort files by relative path to ensure deterministic order
        var sortedFiles = files
            .Select(f => new { FullPath = f, RelativePath = Path.GetRelativePath(folderPath, f) })
            .OrderBy(x => x.RelativePath, StringComparer.Ordinal)
            .Select(x => x.FullPath)
            .ToList();

        // Compute hash for each file
        var fileHashes = new List<byte[]>(sortedFiles.Count);
        foreach (var file in sortedFiles)
        {
            fileHashes.Add(HashFile(file, algorithm));
        }

        // Merge all file hashes into one final hash
        return MergeHashes(fileHashes.ToArray(), algorithm);
    }

    /// <summary>
    /// Merges multiple hash values into a single hash by concatenating them and then hashing the concatenated result.
    /// This is useful for combining hashes of individual items (e.g., files in a folder) into one overall hash.
    /// </summary>
    /// <param name="hashes">The hash values to merge (each as a byte array).</param>
    /// <param name="algorithm">The hash algorithm to use for the final merge; if null, SHA256 is used.</param>
    /// <returns>The merged hash as a byte array.</returns>
    public static byte[] MergeHashes(byte[][]? hashes, HashAlgorithm algorithm) 
        => MergeHashes(algorithm, hashes);

    /// <summary>
    /// Merges multiple hash values into a single hash by concatenating them and then hashing the concatenated result.
    /// This overload allows specifying the algorithm for the final merge.
    /// </summary>
    /// <param name="algorithm">The hash algorithm to use for the final merge; if null, SHA256 is used.</param>
    /// <param name="hashes">The hash values to merge (each as a byte array).</param>
    /// <returns>The merged hash as a byte array.</returns>
    public static byte[] MergeHashes(HashAlgorithm? algorithm, params byte[][]? hashes)
    {
        if (hashes == null || hashes.Length == 0)
            throw new ArgumentException("At least one hash must be provided.", nameof(hashes));

        // Concatenate all hash byte arrays
        int totalLength = hashes.Sum(h => h?.Length ?? 0);
        var combined = new byte[totalLength];
        int offset = 0;
        foreach (var hash in hashes)
        {
            if (hash == null)
                continue;
            Buffer.BlockCopy(hash, 0, combined, offset, hash.Length);
            offset += hash.Length;
        }

        using var alg = algorithm ?? SHA256.Create();
        return alg.ComputeHash(combined);
    }

    // ----- Convenience overloads returning hex strings -----

    /// <summary>
    /// Computes the hash of a string and returns it as a lowercase hexadecimal string.
    /// </summary>
    public static string HashString(string input, HashAlgorithm? algorithm = null)
        => ToHexString(Hash(input, algorithm));

    /// <summary>
    /// Computes the hash of a byte array and returns it as a lowercase hexadecimal string.
    /// </summary>
    public static string HashString(byte[] input, HashAlgorithm? algorithm = null)
        => ToHexString(Hash(input, algorithm));

    /// <summary>
    /// Computes the hash of a stream and returns it as a lowercase hexadecimal string.
    /// </summary>
    public static string HashString(Stream input, HashAlgorithm? algorithm = null)
        => ToHexString(Hash(input, algorithm));

    /// <summary>
    /// Computes the hash of a file and returns it as a lowercase hexadecimal string.
    /// </summary>
    public static string HashFileString(string filePath, HashAlgorithm? algorithm = null)
        => ToHexString(HashFile(filePath, algorithm));

    /// <summary>
    /// Computes the combined hash of all files in a folder and returns it as a lowercase hexadecimal string.
    /// </summary>
    public static string HashFolderString(string folderPath, HashAlgorithm? algorithm = null, bool recursive = true)
        => ToHexString(HashFolder(folderPath, algorithm, recursive));

    /// <summary>
    /// Merges multiple hash values with a specified algorithm and returns the result as a lowercase hexadecimal string.
    /// </summary>
    public static string MergeHashesString(HashAlgorithm? algorithm, params byte[][] hashes)
        => ToHexString(MergeHashes(algorithm, hashes));

    /// <summary>
    /// Converts a byte array to a lowercase hexadecimal string.
    /// </summary>
    private static string ToHexString(byte[] bytes)
    {
        if (bytes == null)
            return string.Empty;
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}
