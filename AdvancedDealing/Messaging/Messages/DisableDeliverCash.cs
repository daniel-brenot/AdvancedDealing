using AdvancedDealing.Economy;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class DisableDeliverCash(DealerManager dealerManager) : MessageBase
    {
        private readonly DealerManager m_dealerManager = dealerManager;

        public override string Text => "Stop delivering cash";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            DealerManager dealerManager = DealerManager.GetManager(NPC.GUID.ToString());
            if (dealerManager.DealerData.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            m_dealerManager.DealerData.DeliverCash = false;
            m_dealerManager.SendPlayerMessage("The dead drops are not safe atm... I will meet you to take the cash!");
            m_dealerManager.SendMessage($"Okay", false, true, 3f);
        }
    }
}
