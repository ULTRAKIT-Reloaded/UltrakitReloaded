namespace ULTRAKIT.Data
{
    public static class InitConfigFile
    {
        public static ConfigFile _initConfig = new ConfigFile()
        {
            RegisterLeviathan = true,
        };
    }

    public class ConfigFile
    {
        public bool RegisterLeviathan;
    }
}
