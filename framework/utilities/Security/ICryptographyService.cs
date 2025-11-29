namespace SmartServices.Framework.Utilities.Security;

/// <summary>
/// Provides encryption and decryption services using Dapr cryptography block.
/// Enables secure handling of sensitive data in microservices.
/// </summary>
public interface ICryptographyService
{
    /// <summary>
    /// Encrypts the specified plaintext using the configured encryption algorithm.
    /// </summary>
    /// <param name="plainText">The plaintext to encrypt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The encrypted data (base64 encoded).</returns>
    Task<string> EncryptAsync(string plainText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts the specified encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data (base64 encoded).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The decrypted plaintext.</returns>
    Task<string> DecryptAsync(string encryptedData, CancellationToken cancellationToken = default);
}
