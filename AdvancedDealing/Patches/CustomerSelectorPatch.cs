using AdvancedDealing.UI;
using HarmonyLib;

#if IL2CPP
using S1CustomerSelector = Il2CppScheduleOne.UI.Phone.CustomerSelector;
#elif MONO
using S1CustomerSelector = ScheduleOne.UI.Phone.CustomerSelector;
#endif

namespace AdvancedDealing.Patches
{
    [HarmonyPatch(typeof(S1CustomerSelector))]
    public class CustomerSelectorPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Open")]
        public static void OpenPostfix()
        {
            if (UIBuilder.CustomerSelector.UICreated)
            {
                UIBuilder.CustomerSelector.SortCustomers();
                UIBuilder.CustomerSelector.Searchbar.text = string.Empty;
            }
        }
    }
}
