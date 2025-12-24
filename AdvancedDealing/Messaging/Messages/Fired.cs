using AdvancedDealing.Economy;
using System;
using AdvancedDealing.Persistence;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.UI.Phone.Messages;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Economy;
using ScheduleOne.Messaging;
using ScheduleOne.UI.Phone.Messages;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class Fired(DealerManager dealerManager) : MessageBase
    {
        private readonly DealerManager _dealerManager = dealerManager;

        public override string Text => "You are no longer my dealer";

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealerManager.ManagedDealer.IsRecruited && SyncManager.IsNoSyncOrActiveAndHost)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            PlayerSingleton<MessagesApp>.Instance.ConfirmationPopup.Open("Are you sure?", $"Calling off the cooperation could make a dealer really mad.\n\nHe maybe will become hostile.\n(In future updates)", S1Conversation, new Action<ConfirmationPopup.EResponse>(OnConfirmationResponse));
        }

        private void OnConfirmationResponse(ConfirmationPopup.EResponse response)
        {
            if (response == ConfirmationPopup.EResponse.Confirm)
            {
                DealerManager.Fire(_dealerManager.ManagedDealer);
            }
        }
    }
}
