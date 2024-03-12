namespace MISGui
{
    public interface IBasicStorage
    {
        string Read(string key);
        RegistryStorage With(string defaultStore);
        void Write(string key, string value);
    }
}