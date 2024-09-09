using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public class KeyVaultManager
{
    private readonly string keyVaultUrl;

    public KeyVaultManager(string keyVaultUrl)
    {
        this.keyVaultUrl = keyVaultUrl;
    }

    public async Task<KeyVaultSecret> GetSecretAsync(string secretName)
    {
        var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        KeyVaultSecret secret = await client.GetSecretAsync(secretName);
        return secret;
    }
}