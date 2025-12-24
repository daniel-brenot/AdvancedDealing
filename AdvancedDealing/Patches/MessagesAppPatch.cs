using AdvancedDealing.Messaging;
using HarmonyLib;
using AdvancedDealing.Persistence;

#if IL2CPP
using Il2CppScheduleOne.UI.Phone.Messages;
using Il2CppScheduleOne.UI;
#elif MONO
using ScheduleOne.UI.Phone.Messages;
using ScheduleOne.UI;
#endif

namespace AdvancedDealing.Patches
{
    [HarmonyPatch(typeof(App<MessagesApp>))]
    public class MessagesAppPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetOpen")]
        public static void SetOpenPostfix()
        {
            if (SaveManager.Instance.SavegameLoaded)
            {
                foreach (ConversationManager conversation in ConversationManager.GetAllManager())
                {
                    conversation.CreateSendableMessages();
                }
            }
        }
    }
}