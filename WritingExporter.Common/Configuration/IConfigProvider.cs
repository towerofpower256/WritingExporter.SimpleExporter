using System;

namespace WritingExporter.Common.Configuration
{
    public interface IConfigProvider
    {
        event EventHandler<ConfigSectionChangedEventArgs> OnSectionChanged;

        TSection GetSection<TSection>() where TSection : BaseConfigSection;
        TSection GetSection<TSection>(string sectionName) where TSection : BaseConfigSection;
        void LoadSettings();
        void SaveSettings();
        void SetSection<TSection>(string sectionName, TSection updatedSection) where TSection : BaseConfigSection;
        void SetSection<TSection>(TSection updatedSection) where TSection : BaseConfigSection;
    }
}