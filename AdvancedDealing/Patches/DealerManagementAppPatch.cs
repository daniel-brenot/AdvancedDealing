using AdvancedDealing.Economy;
using AdvancedDealing.Persistence;
using AdvancedDealing.Persistence.Datas;
using AdvancedDealing.UI;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.UI.Phone.Messages;
#elif MONO
using ScheduleOne.Economy;
using ScheduleOne.UI.Phone.Messages;
#endif

namespace AdvancedDealing.Patches
{
    [HarmonyPatch(typeof(DealerManagementApp))]
    public class DealerManagementAppPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SetOpen")]
        public static void SetOpenPrefix(DealerManagementApp __instance)
        {
            if (SaveManager.Instance.SavegameLoaded)
            {
                DealerManagementAppModification.Create(__instance);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetDisplayedDealer")]
        public static void SetDisplayedDealerPostfix(DealerManagementApp __instance, Dealer dealer)
        {
            if (SaveManager.Instance.SavegameLoaded)
            {
                DealerManager dealerManager = DealerManager.GetManager(dealer);

                if (dealerManager == null) return;

                string deadDropName = "None";
                string guid = dealerManager.DealerData.DeadDrop;

                if (guid != null)
                {
                    DeadDrop deadDrop = DealerManager.GetDeadDrop(dealer);
                    deadDropName = deadDrop.name;
                }

                DealerManagementAppModification.SetDeadDropSelectorButtonValue(deadDropName);
                DealerManagementAppModification.ShowAssignButton(__instance, dealerManager);
                DealerManagementAppModification.UpdateCustomerTitle(__instance, dealerManager);
            }
        }
    }
}