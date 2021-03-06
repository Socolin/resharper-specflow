namespace ReSharperPlugin.SpecflowRiderPlugin.Caching.SpecflowJsonSettings
{
    public class SpecflowSettings
    {
        public SpecflowSettingsLanguage Language { get; } = new SpecflowSettingsLanguage();
        public SpecflowSettingsBindingCulture BindingCulture { get; } = new SpecflowSettingsBindingCulture();

        public SpecflowSettings()
        {
            Language.Feature = "en";
            Language.Tool = "en";
            BindingCulture.Name = "en";
        } 
    }
}