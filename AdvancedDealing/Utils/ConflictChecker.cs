using MelonLoader;

namespace AdvancedDealing.Utils
{
    public static class ConflictChecker
    {
        public static bool DisableMoreItemSlots { get; private set; } = false;

        public static void CheckForConflicts()
        {
            bool conflictsFound = false;
            if (MelonBase.FindMelon("Bread's Storage Tweak Mod", "BreadCh4n") != null)
            {
                conflictsFound = true;
                DisableMoreItemSlots = true;
                Logger.Msg("ConflictChecker", "Bread's Storage Tweaks found: More item slots feature disabled.");
            }

            if (!conflictsFound)
            {
                Logger.Msg("ConflictChecker", "No known conflicting mods found.");
            }
        }
    }
}
