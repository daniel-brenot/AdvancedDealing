using AdvancedDealing.Economy;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Dialogue;
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Dialogue;
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Patches
{
    [HarmonyPatch(typeof(MSGConversation))]
    public class MSGConversationPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SendMessage")]
        public static void SendMessagePostfix(MSGConversation __instance, Message message)
        {
            // Loyality Mode
            if (DealerExtension.DealerExists(__instance.sender.GUID.ToString()))
            {
                DealerExtension dealer = DealerExtension.GetDealer(__instance.sender.GUID.ToString());

                if (message.text == __instance.sender.DialogueHandler.Database.GetLine(EDialogueModule.Dealer, "dealer_rob_partially_defended"))
                {
                    dealer?.ChangeLoyality(-5f);
                }
                else if (message.text == __instance.sender.DialogueHandler.Database.GetLine(EDialogueModule.Dealer, "dealer_rob_loss"))
                {
                    dealer?.ChangeLoyality(-10f);
                }
            }
        }
    }
}