using AdvancedDealing.Economy;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class DisableDeliverCashMessage(DealerManager dealerManager) : MessageMessage
    {
        private readonly DealerManager _dealerManager = dealerManager;

        public override string Text => "Stop delivering cash";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealerManager.Dealer.IsRecruited && _dealerManager.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            _dealerManager.DeliverCash = false;

            _dealerManager.SendPlayerMessage("The dead drops are not safe atm... I will meet you to take the cash!");
            _dealerManager.SendMessage($"Okay", false, true, 2f);
        }
    }
}
