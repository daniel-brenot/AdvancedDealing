using AdvancedDealing.UI;
using HarmonyLib;
using S1CustomerSelector = Il2CppScheduleOne.UI.Phone.CustomerSelector;

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
                UIBuilder.CustomerSelector.Searchbar.SetText(string.Empty, false);
            }
        }
    }
}
