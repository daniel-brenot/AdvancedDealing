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
    public class DeadDropExtension
    {
        public readonly DeadDrop DeadDrop;

        private static readonly List<DeadDropExtension> cache = [];

        public DeadDropExtension(DeadDrop deadDrop)
        {
            DeadDrop = deadDrop;
        }

        public static List<DeadDropExtension> GetAllDeadDrops()
        {
            return cache;
        }

        public static List<DeadDrop> GetDeadDropsByDistance(Transform origin)
        {
            List<DeadDrop> deadDrops = [];

            foreach (DeadDropExtension deadDrop in cache)
            {
                deadDrops.Add(deadDrop.DeadDrop);
            }

            deadDrops.Sort((x, y) => (x.transform.position - origin.position).sqrMagnitude.CompareTo((y.transform.position - origin.position).sqrMagnitude));

            return deadDrops;
        }

        public static DeadDropExtension GetDeadDrop(DeadDrop deadDrop) => GetDeadDrop(deadDrop.GUID.ToString());

        public static DeadDropExtension GetDeadDrop(string guid)
        {
            if (guid == null)
            {
                return null;
            }

            return cache.Find(x => x.DeadDrop.GUID.ToString().Contains(guid));
        }

        public static void AddDeadDrop(DeadDrop deadDrop)
        {
            if (!DeadDropExists(deadDrop))
            {
                cache.Add(new(deadDrop));

                Utils.Logger.Debug("DeadDropExtension", $"Extension for dead drop created: {deadDrop.DeadDropName}");
            }
        }

        public static bool DeadDropExists(DeadDrop deadDrop) => DeadDropExists(deadDrop.GUID.ToString());

        public static bool DeadDropExists(string guid)
        {
            if (guid == null)
            {
                return false;
            }

            return cache.Any(x => x.DeadDrop.GUID.ToString().Contains(guid));
        }

        public static void Initialize()
        {
            for (int i = cache.Count - 1; i >= 0; i--)
            {
                cache[i].Destroy();
            }

            foreach (DeadDrop deadDrop in DeadDrop.DeadDrops)
            {
                AddDeadDrop(deadDrop);
            }
        }

        public void Destroy()
        {
            cache.Remove(this);
        }

        public bool IsFull()
        {
            return DeadDrop.Storage.ItemCount >= DeadDrop.Storage.SlotCount;
        }

        public Vector3 GetPosition()
        {
            return DeadDrop.transform.position;
        }
    }
}
