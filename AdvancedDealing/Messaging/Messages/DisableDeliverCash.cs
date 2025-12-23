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
        private readonly DealerManager _dealerManager = dealerManager;

        public override string Text => "Stop delivering cash";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealerManager.ManagedDealer.IsRecruited && _dealerManager.DealerData.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            _dealerManager.DealerData.DeliverCash = false;
            DealerManager.SendPlayerMessage(_dealerManager.ManagedDealer, "The dead drops are not safe atm... I will meet you to take the cash!");
            DealerManager.SendMessage(_dealerManager.ManagedDealer, $"Okay", false, true, 2f);
        }
    }
}
