using STranslate.Plugin.Translate.AzureTranslator.View;
using STranslate.Plugin.Translate.AzureTranslator.ViewModel;
using System.Text.Json.Nodes;
using System.Windows.Controls;

namespace STranslate.Plugin.Translate.AzureTranslator;

public class Main : TranslatePluginBase
{
    private Control? _settingUi;
    private SettingsViewModel? _viewModel;
    private Settings Settings { get; set; } = null!;
    private IPluginContext Context { get; set; } = null!;

    public override Control GetSettingUI()
    {
        _viewModel ??= new SettingsViewModel(Context, Settings);
        _settingUi ??= new SettingsView { DataContext = _viewModel };
        return _settingUi;
    }

    /// <summary>
    /// Get source language code for Azure Translator
    /// <see href="https://learn.microsoft.com/en-us/azure/ai-services/translator/language-support"/>
    /// </summary>
    /// <param name="langEnum"></param>
    /// <returns></returns>
    public override string? GetSourceLanguage(LangEnum langEnum) => langEnum switch
    {
        LangEnum.Auto => null, // Azure Translator auto-detects when source is not specified
        LangEnum.ChineseSimplified => "zh-Hans",
        LangEnum.ChineseTraditional => "zh-Hant",
        LangEnum.Cantonese => "yue",
        LangEnum.English => "en",
        LangEnum.Japanese => "ja",
        LangEnum.Korean => "ko",
        LangEnum.French => "fr",
        LangEnum.Spanish => "es",
        LangEnum.Russian => "ru",
        LangEnum.German => "de",
        LangEnum.Italian => "it",
        LangEnum.Turkish => "tr",
        LangEnum.PortuguesePortugal => "pt-pt",
        LangEnum.PortugueseBrazil => "pt",
        LangEnum.Vietnamese => "vi",
        LangEnum.Indonesian => "id",
        LangEnum.Thai => "th",
        LangEnum.Malay => "ms",
        LangEnum.Arabic => "ar",
        LangEnum.Hindi => "hi",
        LangEnum.MongolianCyrillic => "mn-Cyrl",
        LangEnum.MongolianTraditional => "mn-Mong",
        LangEnum.Khmer => "km",
        LangEnum.NorwegianBokmal => "nb",
        LangEnum.NorwegianNynorsk => "nb",
        LangEnum.Persian => "fa",
        LangEnum.Swedish => "sv",
        LangEnum.Polish => "pl",
        LangEnum.Dutch => "nl",
        LangEnum.Ukrainian => "uk",
        _ => null
    };

    /// <summary>
    /// Get target language code for Azure Translator
    /// <see href="https://learn.microsoft.com/en-us/azure/ai-services/translator/language-support"/>
    /// </summary>
    /// <param name="langEnum"></param>
    /// <returns></returns>
    public override string? GetTargetLanguage(LangEnum langEnum) => langEnum switch
    {
        LangEnum.Auto => "en", // Default to English if auto
        LangEnum.ChineseSimplified => "zh-Hans",
        LangEnum.ChineseTraditional => "zh-Hant",
        LangEnum.Cantonese => "yue",
        LangEnum.English => "en",
        LangEnum.Japanese => "ja",
        LangEnum.Korean => "ko",
        LangEnum.French => "fr",
        LangEnum.Spanish => "es",
        LangEnum.Russian => "ru",
        LangEnum.German => "de",
        LangEnum.Italian => "it",
        LangEnum.Turkish => "tr",
        LangEnum.PortuguesePortugal => "pt-pt",
        LangEnum.PortugueseBrazil => "pt",
        LangEnum.Vietnamese => "vi",
        LangEnum.Indonesian => "id",
        LangEnum.Thai => "th",
        LangEnum.Malay => "ms",
        LangEnum.Arabic => "ar",
        LangEnum.Hindi => "hi",
        LangEnum.MongolianCyrillic => "mn-Cyrl",
        LangEnum.MongolianTraditional => "mn-Mong",
        LangEnum.Khmer => "km",
        LangEnum.NorwegianBokmal => "nb",
        LangEnum.NorwegianNynorsk => "nb",
        LangEnum.Persian => "fa",
        LangEnum.Swedish => "sv",
        LangEnum.Polish => "pl",
        LangEnum.Dutch => "nl",
        LangEnum.Ukrainian => "uk",
        _ => null
    };

    public override void Init(IPluginContext context)
    {
        Context = context;
        Settings = context.LoadSettingStorage<Settings>();
    }

    public override void Dispose() => _viewModel?.Dispose();

    public override async Task TranslateAsync(TranslateRequest request, TranslateResult result, CancellationToken cancellationToken = default)
    {
        var sourceStr = GetSourceLanguage(request.SourceLang);
        if (GetTargetLanguage(request.TargetLang) is not string targetStr)
        {
            result.Fail(Context.GetTranslation("UnsupportedTargetLang"));
            return;
        }

        if (string.IsNullOrEmpty(Settings.ApiKey))
        {
            result.Fail(Context.GetTranslation("STranslate_Plugin_Translate_AzureTranslator_ApiKey_Required"));
            return;
        }

        if (string.IsNullOrEmpty(Settings.Region))
        {
            result.Fail(Context.GetTranslation("STranslate_Plugin_Translate_AzureTranslator_Region_Required"));
            return;
        }

        // Build request URL
        var baseUrl = Settings.UseCustomEndpoint && !string.IsNullOrEmpty(Settings.CustomEndpoint)
            ? Settings.CustomEndpoint.TrimEnd('/')
            : Constant.ApiUrl;

        var url = $"{baseUrl}?api-version={Constant.ApiVersion}&to={targetStr}";
        if (!string.IsNullOrEmpty(sourceStr))
        {
            url += $"&from={sourceStr}";
        }

        // Build request body
        var content = new[]
        {
            new { Text = request.Text }
        };

        var option = new Options
        {
            Headers = new Dictionary<string, string>
            {
                { "Ocp-Apim-Subscription-Key", Settings.ApiKey },
                { "Ocp-Apim-Subscription-Region", Settings.Region }
            }
        };

        var response = await Context.HttpService.PostAsync(url, content, option, cancellationToken);

        var jsonNode = JsonNode.Parse(response);
        var translations = jsonNode?[0]?["translations"];
        var translatedText = translations?[0]?["text"]?.ToString() ?? throw new Exception(response);

        result.Success(translatedText);
    }
}
