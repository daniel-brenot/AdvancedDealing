namespace AdvancedDealing.UI
{
    public class MessagesAppModification
    {
        public static SettingsPopup SettingsPopup { get; private set; }

        public static void Create()
        {
            SettingsPopup ??= new();
        }

        public static void Clear()
        {
            SettingsPopup = null;
        }
    }
}
