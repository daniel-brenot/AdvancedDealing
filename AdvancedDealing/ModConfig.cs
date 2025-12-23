using MelonLoader;
using MelonLoader.Utils;
using System.IO;

namespace AdvancedDealing
{
    public static class ModConfig
    {
        private static MelonPreferences_Category s_category;

        private static bool s_isCreated;

        public static bool Debug
        {
            get => s_category.GetEntry<bool>("Debug").Value;
            set => s_category.GetEntry<bool>("Debug").Value = value;
        }

        public static bool RealisticMode
        {
            get => s_category.GetEntry<bool>("RealisticMode").Value;
            set => s_category.GetEntry<bool>("RealisticMode").Value = value;
        }

        public static bool SettingsPerDealer
        {
            get => s_category.GetEntry<bool>("SettingsPerDealer").Value;
            set => s_category.GetEntry<bool>("SettingsPerDealer").Value = value;
        }

        public static bool SkipMovement
        {
            get => s_category.GetEntry<bool>("SkipMovement").Value;
            set => s_category.GetEntry<bool>("SkipMovement").Value = value;
        }

        public static void Initialize()
        {
            if (s_isCreated) return;

            s_category = MelonPreferences.CreateCategory($"{ModInfo.k_Name}_01_General", $"{ModInfo.k_Name} - General Settings", false, true);
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, $"{ModInfo.k_Name}.cfg");

            s_category.SetFilePath(path, true);
            CreateEntries();

            if (!File.Exists(path))
            {
                foreach (var entry in s_category.Entries)
                {
                    entry.ResetToDefault();
                }

                s_category.SaveToFile(false);
            }

            s_isCreated = true;
        }

        private static void CreateEntries()
        {
            s_category?.CreateEntry<bool>
            (
                identifier: "Debug",
                default_value: false,
                display_name: "Enable Debug Mode",
                description: "Enables debugging for this mod",
                is_hidden: true
            );
            s_category?.CreateEntry<bool>
            (
                identifier: "RealisticMode",
                default_value: false,
                display_name: "Enable Realistic Mode",
                description: "Makes the mod less feel like a cheat",
                is_hidden: false
            );
            s_category?.CreateEntry<bool>
            (
                identifier: "SettingsPerDealer",
                default_value: false,
                display_name: "Settings Per Dealer",
                description: "Enable seperate modification for each dealer",
                is_hidden: false
            );
            s_category?.CreateEntry<bool>
            (
                identifier: "SkipMovement",
                default_value: false,
                display_name: "Skip Movement (Instant Delivery & Pickup)",
                description: "Skips all movement actions for dealers",
                is_hidden: false
            );
        }
    }
}
