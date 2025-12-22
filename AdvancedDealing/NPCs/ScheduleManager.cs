using AdvancedDealing.NPCs.Actions;
using System;
using System.Collections.Generic;

#if IL2CPP
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Networking;
using Il2CppScheduleOne.NPCs;
#elif MONO
using FishNet;
using ScheduleOne.DevUtilities;
using ScheduleOne.GameTime;
using ScheduleOne.Networking;
using ScheduleOne.NPCs;
#endif

namespace AdvancedDealing.NPCs
{
    public class ScheduleManager
    {
        private static readonly List<ScheduleManager> _cache = [];

        private readonly List<NPCAction> _actionList = [];

        public readonly NPC npc;

        public bool IsEnabled { get; protected set; }

        public NPCAction ActiveAction { get; set; }

        public List<NPCAction> PendingActions { get; set; } = [];

        private List<NPCAction> ActionsAwaitingStart { get; set; } = [];

        private NPCScheduleManager _originalSchedule;

        public ScheduleManager(NPC npc)
        {
            this.npc = npc;
            _originalSchedule = npc.GetComponentInChildren<NPCScheduleManager>();

            Utils.Logger.Debug("ScheduleManager", $"Schedule created: {npc.GUID}");

            _cache.Add(this);
        }

        public void Start()
        {
            Enable();

            NetworkSingleton<TimeManager>.Instance.onMinutePass -= new System.Action(MinPassed);
            NetworkSingleton<TimeManager>.Instance.onMinutePass += new System.Action(MinPassed);
        }

        public void Enable()
        {
            IsEnabled = true;

            MinPassed();
        }

        public void Disable()
        {
            IsEnabled = false;

            MinPassed();

            if (npc.Movement.IsMoving)
            {
                npc.Movement.Stop();
            }
        }

        public void Destroy()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
            }

            if (NetworkSingleton<TimeManager>.InstanceExists)
            {
                NetworkSingleton<TimeManager>.Instance.onMinutePass -= new System.Action(MinPassed);
            }
        }

        protected void MinPassed()
        {
            if ((!InstanceFinder.IsServer && !NetworkSingleton<ReplicationQueue>.Instance.ReplicationDoneForLocalPlayer) || !npc.IsSpawned)  return;

            if (!IsEnabled)
            {
                if (ActiveAction != null)
                {
                    ActiveAction.Interrupt();
                }

                return;
            }

            List<NPCAction> actionsToStart = GetActionsToStart();

            if (actionsToStart.Count > 0)
            {
                NPCAction nPCAction = actionsToStart[0];
                if (ActiveAction != nPCAction)
                {
                    if (ActiveAction != null && nPCAction.Priority > ActiveAction.Priority)
                    {
                        ActiveAction.Interrupt();
                    }

                    if (ActiveAction == null)
                    {
                        if (_originalSchedule.ActiveAction == null || (_originalSchedule.ActiveAction != null && nPCAction.Priority > _originalSchedule.ActiveAction.Priority))
                        {
                            StartAction(nPCAction);
                        }
                    }
                }

                foreach (NPCAction action in actionsToStart)
                {
                    if (!action.HasStarted && !ActionsAwaitingStart.Contains(action))
                    {
                        ActionsAwaitingStart.Add(action);
                    }
                }
            }
        }

        private List<NPCAction> GetActionsToStart()
        {
            List<NPCAction> list = [];

            foreach (NPCAction action in _actionList)
            {
                if (!(action == null) && action.ShouldStart())
                {
                    list.Add(action);
                }
            }

            list.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            return list;
        }

        private void StartAction(NPCAction action)
        {
            if (ActiveAction != null) return;

            if (ActionsAwaitingStart.Contains(action))
            {
                ActionsAwaitingStart.Remove(action);
            }

            if (action.HasStarted)
            {
                action.Resume();
            }
            else
            {
                action.Start();
            }
        }

        public void AddAction(NPCAction action, int StartTime = 0)
        {
            Type type = action.GetType();

            if (_actionList.Exists(a => a.GetType() == type)) return;

            action.SetReferences(npc, this, _originalSchedule, StartTime);
            _actionList.Add(action);
        }

        public void RemoveAction(NPCAction action)
        {
            Type type = action.GetType();

            if (_actionList.Exists(a => a.GetType() == type))
            {
                _actionList.Remove(action);
            }
        }

        public static ScheduleManager GetManager(string npcGuid)
        {
            ScheduleManager manager = _cache.Find(x => x.npc.GUID.ToString().Contains(npcGuid));

            if (manager == null)
            {
                Utils.Logger.Debug("ScheduleManager", $"Could not find schedule for: {npcGuid}");
                return null;
            }

            return manager;
        }

        public static void ClearAll()
        {
            foreach (ScheduleManager schedule in _cache)
            {
                foreach (NPCAction action in schedule._actionList)
                {
                    action.Destroy();
                }
                schedule.Destroy();
            }

            _cache.Clear();

            Utils.Logger.Debug("ScheduleManager", "Schedules deinitialized");
        }

        public static bool ScheduleExists(string npcName)
        {
            ScheduleManager instance = _cache.Find(x => x.npc.name.Contains(npcName));

            return instance != null;
        }
    }
}
