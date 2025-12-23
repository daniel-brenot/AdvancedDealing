using AdvancedDealing.Economy;
using AdvancedDealing.UI;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class AdjustSettings(DealerManager dealerManager) : MessageBase
    {
        private readonly DealerManager m_dealerManager = dealerManager;

        public override string Text => "Need to adjust settings";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (ModConfig.RealisticMode)
            {
                return false;
            }
            return true;
        }

        public override void OnSelected()
        {
            MessagesAppModification.SettingsPopup.Open(m_dealerManager);
        }
    }
}
