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
    public class ConversationManager
    {
        private static readonly List<ConversationManager> s_cache = [];

        private readonly List<MessageBase> m_messageList = [];

        private readonly List<MessageBase> m_sendableMessages = [];

        public readonly NPC NPC;

        public readonly MSGConversation Conversation;

        private bool m_uiPatched;

        public ConversationManager(NPC npc)
        {
            NPC = npc;
            Conversation = npc.MSGConversation;

            Utils.Logger.Debug("ConversationManager", $"Conversation created: {npc.GUID}");

            s_cache.Add(this);
        }

        public void CreateSendableMessages()
        {
            foreach (MessageBase msg in m_messageList)
            {
                if (!m_sendableMessages.Contains(msg))
                {
                    SendableMessage sMsg = Conversation.CreateSendableMessage(msg.Text);
#if IL2CPP
                    sMsg.ShouldShowCheck = (SendableMessage.BoolCheck)msg.ShouldShowCheck;
#elif MONO
                    sMsg.ShouldShowCheck = msg.ShouldShowCheck;
#endif
                    sMsg.disableDefaultSendBehaviour = msg.DisableDefaultSendBehaviour;
                    sMsg.onSelected = new Action(msg.OnSelected);
                    sMsg.onSent = new Action(msg.OnSent);

                    m_sendableMessages.Add(msg);
                }
            }

            if (!m_uiPatched)
            {
                NPC.ConversationCanBeHidden = false;

                Conversation.EnsureUIExists();
                Conversation.SetEntryVisibility(true);

                m_uiPatched = true;
            }
        }

        public void AddMessage(MessageBase message)
        {
            Type type = message.GetType();

            if (m_messageList.Exists(a => a.GetType() == type)) return;

            message.SetReferences(NPC, this, Conversation);
            m_messageList.Add(message);
        }

        public static ConversationManager GetManager(string npcGuid)
        {
            ConversationManager manager = s_cache.Find(x => x.NPC.GUID.ToString().Contains(npcGuid));

            if (manager == null)
            {
                Utils.Logger.Debug("ConversationManager", $"Could not find conversation for: {npcGuid}");
                return null;
            }

            return manager;
        }

        public static List<ConversationManager> GetAllManager()
        {
            return s_cache;
        }

        public static void ClearAll()
        {
            s_cache.Clear();

            Utils.Logger.Debug("ConversationManager", "Conversations deinitialized");
        }

        public static bool ScheduleExists(string npcName)
        {
            ConversationManager instance = s_cache.Find(x => x.NPC.name.Contains(npcName));

            return instance != null;
        }
    }
}
