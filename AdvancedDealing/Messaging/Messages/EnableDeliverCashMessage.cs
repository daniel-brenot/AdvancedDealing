using AdvancedDealing.Economy;
using AdvancedDealing.UI;
using System;
using System.Globalization;


#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class EnableDeliverCashMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => "Please deliver cash";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited && !_dealer.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            UIBuilder.SliderPopup.Open($"Cash Threshold ({_dealer.Dealer.name})", null, _dealer.CashThreshold, 0f, 9999f, 50f, 0, OnSend, null, "C0", CultureInfo.GetCultureInfo("en-US"));
        }

        private void OnSend(float value)
        {
            _dealer.DeliverCash = true;
            _dealer.CashThreshold = value;

            _dealer.SendPlayerMessage($"Yoo, could you deliver your cash to the dead drop? Keep ${value} at max.");
            _dealer.SendMessage($"Sure thing boss!", false, true, 2f);
        }
    }
}
