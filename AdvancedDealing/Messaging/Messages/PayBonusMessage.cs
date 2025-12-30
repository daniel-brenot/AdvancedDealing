using AdvancedDealing.Economy;
using AdvancedDealing.UI;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Money;
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class PayBonusMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => "I will pay a bonus";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited && ModConfig.LoyalityMode)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            UIBuilder.SliderPopup.Open($"Pay Bonus ({_dealer.Dealer.name})", "This will push the dealers loyality!", 0f, 0f, 9999f, 50f, 0, OnSend, null, "${0:0}");
        }

        private void OnSend(float value)
        {
            NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction($"Bonus payment for {_dealer.Dealer.fullName}", 0f - value, 1f, string.Empty);

            float amount;

            if (value < 50f)
            {
                amount = 1f;
            }
            else if (value < 100f)
            {
                amount = 5f;
            }
            else if (value < 500f)
            {
                amount = 20f;
            }
            else if (value < 1000f)
            {
                amount = 40f;
            }
            else
            {
                amount = 60f;
            }

            _dealer.ChangeLoyality(amount);
            _dealer.SendPlayerMessage($"I will send you a bonus of ${value}. Good work!");
            _dealer.SendMessage($"Thanks boss!", false, true, 2f);
        }
    }
}
