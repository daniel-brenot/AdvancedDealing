namespace AdvancedDealing
{
    public static class ModInfo
    {
#if IL2CPP
        public const string NAME = "AdvancedDealing.Il2Cpp";
#elif MONO
        public const string NAME = "AdvancedDealing.Mono";
#endif

        public const string VERSION = "1.0.0";

        public const string AUTHOR = "ManZune";

        public const string DOWNLOAD_LINK = "https://github.com/manzune/AdvancedDealing";
    }
}
