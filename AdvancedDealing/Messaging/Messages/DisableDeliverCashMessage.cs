using AdvancedDealing.Economy;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class DisableDeliverCashMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => "Stop delivering cash";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited && _dealer.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            _dealer.DeliverCash = false;

            _dealer.SendPlayerMessage("The dead drops are not safe atm... I will meet you to take the cash!");
            _dealer.SendMessage($"Okay", false, true, 2f);
        }
    }
}
