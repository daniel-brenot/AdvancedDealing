using AdvancedDealing.Economy;
using AdvancedDealing.UI;
using System;

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
            if (_dealerManager.Dealer.IsRecruited && !_dealerManager.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            UIModification.SliderPopup.Open($"Cash Threshold ({_dealerManager.Dealer.name})", null, _dealerManager.CashThreshold, 0f, 9999f, 0, OnSend, null, "$");
        }

        private void OnSend()
        {
            float value = (float)Math.Round(UIModification.SliderPopup.Slider.value, 0);

            _dealerManager.DeliverCash = true;
            _dealerManager.CashThreshold = value;

            _dealerManager.SendPlayerMessage($"Yoo, could you deliver your cash to the dead drop? Keep ${value} at max.");
            _dealerManager.SendMessage($"Sure thing boss!", false, true, 2f);
        }
    }
}
