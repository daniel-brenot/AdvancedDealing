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
            if (_dealerManager.ManagedDealer.IsRecruited && !_dealerManager.DealerData.DeliverCash)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            UIModification.SliderPopup.Open($"Cash Threshold ({_dealerManager.ManagedDealer.name})", null, _dealerManager.DealerData.CashThreshold, 0f, 9999f, 0, OnSend, null, "$");
        }

        private void OnSend()
        {
            float value = (float)Math.Round(UIModification.SliderPopup.Slider.value, 0);

            _dealerManager.DealerData.DeliverCash = true;
            _dealerManager.DealerData.CashThreshold = value;
            DealerManager.SendPlayerMessage(_dealerManager.ManagedDealer, $"Yoo, could you deliver your cash to the dead drop? Keep ${value} at max.");
            DealerManager.SendMessage(_dealerManager.ManagedDealer, $"Sure thing boss!", false, true, 2f);
        }
    }
}
