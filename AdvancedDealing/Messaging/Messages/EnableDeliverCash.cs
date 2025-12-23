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
        private readonly DealerManager _dealerManager = dealerManager;

        public override string Text => "Please deliver cash";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealerManager.ManagedDealer.IsRecruited && !_dealerManager.DealerData.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            _dealerManager.DealerData.DeliverCash = true;
            DealerManager.SendPlayerMessage(_dealerManager.ManagedDealer, $"Yoo, could you deliver your cash to the dead drop? Keep ${"1500"} at max.");
            DealerManager.SendMessage(_dealerManager.ManagedDealer, $"Sure thing boss!", false, true, 2f);
        }
    }
}
