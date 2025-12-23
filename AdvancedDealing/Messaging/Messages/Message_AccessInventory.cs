using AdvancedDealing.Economy;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.UI;
#elif MONO
using ScheduleOne.Messaging;
using ScheduleOne.DevUtilities;
using ScheduleOne.Economy;
using ScheduleOne.UI;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class Message_AccessInventory(DealerManager dealerManager) : MessageBase
    {
        private readonly DealerManager _dealerManager = dealerManager;

        public override string Text => "I need to access your inventory";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (ModConfig.RealisticMode)
            {
                return false;
            }
            return true;
        }

        public override void OnSelected()
        {
            Singleton<GameplayMenu>.Instance.SetIsOpen(false);
            typeof(Dealer).GetMethod("TradeItems").Invoke(_dealerManager.ManagedDealer, []);
        }
    }
}
