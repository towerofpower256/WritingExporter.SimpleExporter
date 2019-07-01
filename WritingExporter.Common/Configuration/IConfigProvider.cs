﻿namespace WritingExporter.Common.Configuration
{
    public interface IConfigProvider
    {
        T GetSection<T>();
        T GetSection<T>(string sectionName);
        void LoadSettings();
        void SaveSettings();
        void SetSection<T>(string sectionName, T updatedSection);
        void SetSection<T>(T updatedSection);
    }
}