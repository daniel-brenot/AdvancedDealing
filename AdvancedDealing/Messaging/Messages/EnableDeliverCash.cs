using AdvancedDealing.Economy;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class EnableDeliverCash(DealerManager dealerManager) : MessageBase
    {
        private readonly DealerManager m_dealerManager = dealerManager;

        public override string Text => "Please deliver cash";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            DealerManager dealerManager = DealerManager.GetManager(NPC.GUID.ToString());
            if (!dealerManager.DealerData.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            m_dealerManager.DealerData.DeliverCash = true;
            m_dealerManager.SendPlayerMessage($"Yoo, could you deliver your cash to the dead drop? Keep ${"1500"} at max.");
            m_dealerManager.SendMessage($"Sure thing boss!", false, true, 3f);
        }
    }
}
