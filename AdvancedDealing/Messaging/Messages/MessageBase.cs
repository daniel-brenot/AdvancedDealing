#if IL2CPP
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
#elif MONO
using ScheduleOne.Messaging;
using ScheduleOne.NPCs;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public abstract class MessageBase
    {
        public virtual string Text => "Text";

        protected NPC NPC;

        protected Conversation Conversation;

        protected MSGConversation S1Conversation => NPC.MSGConversation;

        public virtual bool DisableDefaultSendBehaviour => false;

        public virtual void SetReferences(NPC npc, Conversation conversation)
        {
            NPC = npc;
            Conversation = conversation;
        }

        public virtual bool ShouldShowCheck(SendableMessage sMsg)
        {
            return true;
        }

        public virtual void OnSelected() { }

        public virtual void OnSent() { }
    }
}
