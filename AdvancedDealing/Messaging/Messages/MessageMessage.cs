#if IL2CPP
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
#elif MONO
using ScheduleOne.Messaging;
using ScheduleOne.NPCs;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public abstract class MessageMessage
    {
        public virtual string Text => "Text";

        protected NPC NPC;

        protected MSGConversation S1Conversation;

        protected ConversationManager Conversation;

        public virtual bool DisableDefaultSendBehaviour => false;

        public virtual void SetReferences(NPC npc, ConversationManager conversation, MSGConversation originalConversation)
        {
            NPC = npc;
            Conversation = conversation;
            S1Conversation = originalConversation;
        }

        public virtual bool ShouldShowCheck(SendableMessage sMsg)
        {
            return true;
        }

        public virtual void OnSelected() { }

        public virtual void OnSent() { }
    }
}
