using MelonLoader;
using MelonLoader.Preferences;
using MelonLoader.Utils;
using System.IO;

namespace AdvancedDealing
{
    public static class ModConfig
    {
        private static MelonPreferences_Category generalCategory;

        private static MelonPreferences_Category loyalityModeCategory;

        private static bool isInitialized;

        // General
        public static bool Debug
        {
            get => generalCategory.GetEntry<bool>("Debug").Value;
            set => generalCategory.GetEntry<bool>("Debug").Value = value;
        }

        public static bool LoyalityMode
        {
            get => generalCategory.GetEntry<bool>("LoyalityMode").Value;
            set => generalCategory.GetEntry<bool>("LoyalityMode").Value = value;
        }

        public static bool SkipMovement
        {
            get => generalCategory.GetEntry<bool>("SkipMovement").Value;
            set => generalCategory.GetEntry<bool>("SkipMovement").Value = value;
        }

        // Loyality Mode
        public static float ExperienceModifier
        {
            get => loyalityModeCategory.GetEntry<float>("ExperienceModifier").Value;
            set => loyalityModeCategory.GetEntry<float>("ExperienceModifier").Value = value;
        }

        public static int MaxCustomersPerLevel
        {
            get => loyalityModeCategory.GetEntry<int>("MaxCustomersPerLevel").Value;
            set => loyalityModeCategory.GetEntry<int>("MaxCustomersPerLevel").Value = value;
        }

        public static int ItemSlotsPerLevel
        {
            get => loyalityModeCategory.GetEntry<int>("ItemSlotsPerLevel").Value;
            set => loyalityModeCategory.GetEntry<int>("ItemSlotsPerLevel").Value = value;
        }

        public static float SpeedIncreasePerLevel
        {
            get => loyalityModeCategory.GetEntry<float>("SpeedIncreasePerLevel").Value;
            set => loyalityModeCategory.GetEntry<float>("SpeedIncreasePerLevel").Value = value;
        }

        public static float NegotiationSuccessModifier
        {
            get => loyalityModeCategory.GetEntry<float>("NegotiationSuccessModifier").Value;
            set => loyalityModeCategory.GetEntry<float>("NegotiationSuccessModifier").Value = value;
        }

        public static void Initialize()
        {
            if (isInitialized) return;

            generalCategory = MelonPreferences.CreateCategory($"{ModInfo.Name}_01_General", $"{ModInfo.Name} - General Settings", false, true);
            loyalityModeCategory = MelonPreferences.CreateCategory($"{ModInfo.Name}_02_LoyalityMode", $"{ModInfo.Name} - Loyality Mode Settings", false, true);
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, $"{ModInfo.Name}.cfg");

            generalCategory.SetFilePath(path, true, false);
            loyalityModeCategory.SetFilePath(path, true, false);

            CreateEntries();

            if (!File.Exists(path))
            {
                foreach (var entry in generalCategory.Entries)
                {
                    entry.ResetToDefault();
                }

                foreach (var entry in loyalityModeCategory.Entries)
                {
                    entry.ResetToDefault();
                }

                generalCategory.SaveToFile(false);
                loyalityModeCategory.SaveToFile(false);
            }

            isInitialized = true;
        }

        private static void CreateEntries()
        {
            // General
            generalCategory.CreateEntry<bool>
            (
                identifier: "Debug",
                default_value: false,
                display_name: "Enable Debug Mode",
                description: "Enables debugging for this mod",
                is_hidden: false
            );
            generalCategory.CreateEntry<bool>
            (
                identifier: "LoyalityMode",
                default_value: false,
                display_name: "Loyality Mode (WIP)",
                description: "Makes the mod less feel like a cheat",
                is_hidden: false
            );
            generalCategory.CreateEntry<bool>
            (
                identifier: "SkipMovement",
                default_value: false,
                display_name: "Skip Movement (Instant Delivery)",
                description: "Skips all movement actions for dealers",
                is_hidden: false
            );

            // Loyality Mode
            loyalityModeCategory.CreateEntry<float>
            (
                identifier: "ExperienceModifier",
                default_value: 2f,
                display_name: "Experience Modifier (Higher = More XP needed)",
                description: "How hard should it be?",
                is_hidden: false,
                validator: new ValueRange<float>(0.1f, 5f)
            );
            loyalityModeCategory.CreateEntry<int>
            (
                identifier: "MaxCustomersPerLevel",
                default_value: 1,
                display_name: "Max Customers Per Level (Capped at 24 in total)",
                description: "How many customers should be added per level?",
                is_hidden: false,
                validator: new ValueRange<int>(0, 10)
            );
            loyalityModeCategory.CreateEntry<int>
            (
                identifier: "ItemSlotsPerLevel",
                default_value: 1,
                display_name: "Item Slots Per Level",
                description: "How many item slots should be added per level?",
                is_hidden: false,
                validator: new ValueRange<int>(0, 10)
            );
            loyalityModeCategory.CreateEntry<float>
            (
                identifier: "SpeedIncreasePerLevel",
                default_value: 0.15f,
                display_name: "Speed Increase Per Level",
                description: "How much speed should your dealer gain per level?",
                is_hidden: false,
                validator: new ValueRange<float>(0.1f, 1f)
            );
            loyalityModeCategory.CreateEntry<float>
            (
                identifier: "NegotiationSuccessModifier",
                default_value: 50f,
                display_name: "Negotiation Success Modifier (Higher = More Easy)",
                description: "Defines how hard it would be to negotiate with a dealer",
                is_hidden: false,
                validator: new ValueRange<float>(0.1f, 100f)
            );
        }
    }
}
