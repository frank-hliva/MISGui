namespace MISGui
{
    public interface IBasicStorage
    {
        string GetValue(string key);
        void SetValue(string key, string value);
        RegistryStorage With(string defaultStore);
    }
}