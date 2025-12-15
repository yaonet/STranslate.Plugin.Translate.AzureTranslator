namespace STranslate.Plugin.Translate.AzureTranslator;

public class Settings
{
    /// <summary>
    /// Azure Translator API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Azure region (e.g., eastus, westeurope, etc.)
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Custom endpoint URL (optional, for sovereign clouds)
    /// </summary>
    public string CustomEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use custom endpoint
    /// </summary>
    public bool UseCustomEndpoint { get; set; } = false;
}
