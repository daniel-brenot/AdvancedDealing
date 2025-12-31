using AdvancedDealing.Economy;
using AdvancedDealing.Persistence;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Economy;
#elif MONO
using ScheduleOne.Economy;
#endif

namespace AdvancedDealing.Patches
{
    [HarmonyPatch(typeof(Dealer))]
    public class DealerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CustomerContractEnded")]
        public static void CustomerContractEndedPostfix(Dealer __instance)
        {
            if (DealerExtension.DealerExists(__instance) && NetworkSynchronizer.IsNoSyncOrHost)
            {
                DealerExtension dealer = DealerExtension.GetDealer(__instance);
                dealer.DailyContractCount++;

                if (dealer.DailyContractCount > 6)
                {
                    dealer.ChangeLoyality(0f - 10f);
                }
            }
        }
    }
}
