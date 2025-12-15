using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace STranslate.Plugin.Translate.AzureTranslator.ViewModel;

public partial class SettingsViewModel : ObservableObject, IDisposable
{
    private readonly IPluginContext Context;
    private readonly Settings Settings;

    public SettingsViewModel(IPluginContext context, Settings settings)
    {
        Context = context;
        Settings = settings;

        ApiKey = Settings.ApiKey;
        Region = Settings.Region;
        CustomEndpoint = Settings.CustomEndpoint;
        UseCustomEndpoint = Settings.UseCustomEndpoint;

        PropertyChanged += PropertyChangedHandler;
    }

    private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ApiKey):
                Settings.ApiKey = ApiKey;
                break;
            case nameof(Region):
                Settings.Region = Region;
                break;
            case nameof(CustomEndpoint):
                Settings.CustomEndpoint = CustomEndpoint;
                break;
            case nameof(UseCustomEndpoint):
                Settings.UseCustomEndpoint = UseCustomEndpoint;
                break;
            default:
                break;
        }
        Context.SaveSettingStorage<Settings>();
    }

    public void Dispose() => PropertyChanged -= PropertyChangedHandler;

    [ObservableProperty] public partial string ApiKey { get; set; }
    [ObservableProperty] public partial string Region { get; set; }
    [ObservableProperty] public partial string CustomEndpoint { get; set; }
    [ObservableProperty] public partial bool UseCustomEndpoint { get; set; }
}
