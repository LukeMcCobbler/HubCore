namespace HubCore.Infrastructure
{
    public interface ISettingsManager
    {
        string GetSetting(string Application, string Section, string Key, string DefaultValue);
        string GetSetting(string Application, string Section, string Key);
        void SaveSetting(string Application, string Section, string Key, string ValueToSave,SettingSaveLocation WhereToSave);

    }
}