using System;
using System.Reflection;

#if IL2CPP
using Il2CppFishNet.Broadcast;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
#elif MONO
using FishNet.Broadcast;
#endif

namespace AdvancedDealing.Persistence.Datas
{
    public abstract class Data(string identifier)
    {
        public virtual string DataType => GetType().Name;

        public string ModVersion = ModInfo.Version;

        public string Identifier = identifier;

        public virtual void SetDefaults() { }

        public virtual bool HasChanges(object other)
        {
            if (!GetType().Equals(other.GetType()))
            {
                throw new Exception($"Tried to compare {GetType()} with {other.GetType()}");
            }

            FieldInfo[] fields = GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.GetValue(this) != field.GetValue(other))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
