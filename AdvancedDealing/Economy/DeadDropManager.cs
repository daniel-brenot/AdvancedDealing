using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if IL2CPP
using Il2CppScheduleOne.Economy;
#elif MONO
using ScheduleOne.Economy;
#endif

namespace AdvancedDealing.Economy
{
    public class DeadDropManager
    {
        private readonly DeadDrop m_deadDrop;

        private static readonly List<DeadDropManager> s_cache = [];

        public DeadDropManager(DeadDrop deadDrop)
        {
            m_deadDrop = deadDrop;
        }

        public static DeadDropManager GetManager(DeadDrop deadDrop)
        {
            return s_cache.Find(x => x.m_deadDrop == deadDrop);
        }

        public static DeadDropManager GetManager(string deadDropGuid)
        {
            return s_cache.Find(x => x.m_deadDrop.GUID.ToString().Contains(deadDropGuid));
        }

        public static void AddDeadDrop(DeadDrop deadDrop)
        {
            if (DeadDropExists(deadDrop)) return;

            DeadDropManager manager = new(deadDrop);
            s_cache.Add(manager);

            Utils.Logger.Debug("DeadDropManager", $"Dead drop added: {deadDrop.GUID}");
        }

        public static DeadDrop GetDeadDrop(string deadDropGuid)
        {
            DeadDropManager manager = s_cache.Find(x => x.m_deadDrop.GUID.ToString().Contains(deadDropGuid));
            if (manager == null)
            {
                Utils.Logger.Debug("DeadDropManager", $"Could not find dead drop: {deadDropGuid}");

                return null;
            }

            return manager.m_deadDrop;
        }

        public static List<DeadDrop> GetAllDeadDrops()
        {
            List<DeadDrop> deadDrops = [];
            foreach (DeadDropManager manager in s_cache)
            {
                deadDrops.Add(manager.m_deadDrop);
            }

            return deadDrops;
        }

        public static bool DeadDropExists(DeadDrop deadDrop)
        {
            return s_cache.Any(x => x.m_deadDrop == deadDrop);
        }

        public static List<DeadDrop> GetAllByDistance(Transform origin)
        {
            List<DeadDrop> deadDrops = GetAllDeadDrops();
            deadDrops.Sort((x, y) => (x.transform.position - origin.position).sqrMagnitude.CompareTo((y.transform.position - origin.position).sqrMagnitude));

            return deadDrops;
        }

        public static bool IsFull(DeadDrop deadDrop)
        {
            return (deadDrop.Storage.ItemCount >= deadDrop.Storage.SlotCount);
        }

        public static Vector3 GetPosition(DeadDrop deadDrop)
        {
            return deadDrop.transform.position;
        }

        public static void Load()
        {
            s_cache.Clear();

            foreach (DeadDrop deadDrop in DeadDrop.DeadDrops)
            {
                AddDeadDrop(deadDrop);
            }
        }
    }
}
