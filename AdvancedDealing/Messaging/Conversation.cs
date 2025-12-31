using AdvancedDealing.Messaging.Messages;
using System;
using System.Collections.Generic;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
#elif MONO
using ScheduleOne.Messaging;
using ScheduleOne.NPCs;
#endif

namespace AdvancedDealing.Messaging
{
    public class Conversation
    {
        public readonly NPC NPC;

        public bool UIPatched;

        private static readonly List<Conversation> cache = [];

        public MSGConversation S1Conversation => NPC.MSGConversation;

        private readonly List<MessageBase> _sendableMessages = [];

        private readonly List<MessageBase> _patchedMessages = [];

        public Conversation(NPC npc)
        {
            NPC = npc;

            Utils.Logger.Debug("Conversation", $"Conversation created: {npc.fullName}");

            cache.Add(this);
        }

        public void PatchSendableMessages()
        {
            if (NPC == null || S1Conversation == null) return;

            foreach (MessageBase msg in _sendableMessages)
            {
#if IL2CPP
                bool exists = S1Conversation.Sendables.Exists((Func<SendableMessage, bool>)(x => x.Text == msg.Text));
#elif MONO
                bool exists = S1Conversation.Sendables.Exists(x => x.Text == msg.Text);
#endif
                if (!_patchedMessages.Contains(msg) && !exists)
                {
                    SendableMessage sMsg = S1Conversation.CreateSendableMessage(msg.Text);
#if IL2CPP
                    sMsg.ShouldShowCheck = (SendableMessage.BoolCheck)msg.ShouldShowCheck;
#elif MONO
                    sMsg.ShouldShowCheck = msg.ShouldShowCheck;
#endif
                    sMsg.disableDefaultSendBehaviour = msg.DisableDefaultSendBehaviour;
                    sMsg.onSelected = new Action(msg.OnSelected);
                    sMsg.onSent = new Action(msg.OnSent);

                    _patchedMessages.Add(msg);
                }
            }

            if (!UIPatched)
            {
                NPC.ConversationCanBeHidden = false;

                S1Conversation.EnsureUIExists();
                S1Conversation.SetEntryVisibility(true);

                UIPatched = true;
            }
        }

        public void Destroy()
        {
            if (NPC != null)
            {
                NPC.ConversationCanBeHidden = true;
            }

            UIPatched = false;
            cache.Remove(this);
        }

        public void AddSendableMessage(MessageBase message)
        {
            Type type = message.GetType();

            if (_sendableMessages.Exists(a => a.GetType() == type)) return;

            message.SetReferences(NPC, this);
            _sendableMessages.Add(message);
        }

        public static Conversation GetConversation(string npcGuid)
        {
            Conversation conversation = cache.Find(x => x.NPC.GUID.ToString().Contains(npcGuid));

            if (conversation == null)
            {
                Utils.Logger.Error("Conversation", $"Could not find conversation for: {npcGuid}");
                return null;
            }

            return conversation;
        }

        public static List<Conversation> GetAllConversations()
        {
            return cache;
        }

        public static void ClearAll()
        {
            cache.Clear();

            Utils.Logger.Debug("Conversation", "Conversations deinitialized");
        }

        public static bool ConversationExists(string npcName)
        {
            Conversation instance = cache.Find(x => x.NPC.name.Contains(npcName));

            return instance != null;
        }
    }
}
