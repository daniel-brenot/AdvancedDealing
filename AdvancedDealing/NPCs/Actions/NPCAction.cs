using UnityEngine;
using System;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.NPCs;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.GameTime;
using ScheduleOne.NPCs;
#endif

namespace AdvancedDealing.NPCs.Actions
{
    public abstract class NPCAction
    {
        public const int MAX_CONSECUTIVE_PATHING_FAILURES = 5;

        protected int priority;

        public int StartTime;

        protected NPC npc;

        protected Schedule schedule;

        protected NPCScheduleManager scheduleManager;

        public Action onEnded;

        protected int consecutivePathingFailures;

        protected virtual string ActionName =>
            "ActionName";

        protected virtual string ActionType =>
            "NPCAction";

        public bool IsActive { get; protected set; }

        public bool HasStarted { get; protected set; }

        public virtual int Priority =>
            priority;

        protected NPCMovement Movement =>
            npc.Movement;

        public NPCAction()
        {
            Awake();
        }

        protected virtual void Awake()
        {
        }

        public virtual void SetReferences(NPC npc, Schedule schedule, NPCScheduleManager scheduleManager, int StartTime = 0)
        {
            this.npc = npc;
            this.schedule = schedule;
            this.scheduleManager = scheduleManager;

            if (StartTime != 0)
            {
                this.StartTime = StartTime;
            }
        }

        public virtual void Start()
        {
            scheduleManager.DisableSchedule();

            NetworkSingleton<TimeManager>.Instance.onMinutePass -= new Action(MinPassed);
            NetworkSingleton<TimeManager>.Instance.onMinutePass += new Action(MinPassed);

            Utils.Logger.Debug("ScheduleManager", $"{ActionType} \"{ActionName}\" for {npc.name} started.");

            IsActive = true;
            schedule.ActiveAction = this;
            HasStarted = true;
        }

        public void Destroy()
        {
            if (schedule.PendingActions.Contains(this))
            {
                schedule.PendingActions.Remove(this);
            }

            if (HasStarted)
            {
                IsActive = false;

                if (schedule.ActiveAction == this)
                {
                    schedule.ActiveAction = null;
                }

                HasStarted = false;
            }

            if (NetworkSingleton<TimeManager>.InstanceExists)
            {
                NetworkSingleton<TimeManager>.Instance.onMinutePass -= new Action(MinPassed);
            }
        }

        public virtual void End()
        {
            scheduleManager.EnableSchedule();

            Utils.Logger.Debug("ScheduleManager", $"{ActionType} \"{ActionName}\" for {npc.name} ended.");

            IsActive = false;
            schedule.ActiveAction = null;
            HasStarted = false;

            onEnded?.Invoke();
        }

        public virtual void Interrupt()
        {
            scheduleManager.EnableSchedule();

            Utils.Logger.Debug("ScheduleManager", $"{ActionType} \"{ActionName}\" for {npc.name} interrupted.");

            IsActive = false;
            schedule.ActiveAction = null;

            if (!schedule.PendingActions.Contains(this))
            {
                schedule.PendingActions.Add(this);
            }
        }

        public virtual void Resume()
        {
            scheduleManager.DisableSchedule();

            Utils.Logger.Debug("ScheduleManager", $"{ActionType} \"{ActionName}\" for {npc.name} resumed.");

            IsActive = true;
            schedule.ActiveAction = this;

            if (schedule.PendingActions.Contains(this))
            {
                schedule.PendingActions.Remove(this);
            }
        }

        public virtual void MinPassed() { }

        public virtual bool ShouldStart() 
        { 
            return true;
        }

        public void SetDestination(Vector3 pos, bool teleportIfFail = true)
        {
            if (teleportIfFail && consecutivePathingFailures >= MAX_CONSECUTIVE_PATHING_FAILURES && !Movement.CanGetTo(pos))
            {
                Utils.Logger.Debug("ScheduleManager", $"Too many pathing failures for {npc.name}. Warping to {pos}.");

                Movement.Warp(pos);
                WalkCallback(NPCMovement.WalkResult.Success);
            }
            else
            {
                Movement.SetDestination(pos, (Action<NPCMovement.WalkResult>)(res => WalkCallback(res)), maximumDistanceForSuccess: 1f);
            }
        }

        protected virtual void WalkCallback(NPCMovement.WalkResult res)
        {
            if (IsActive)
            {
                if (res == NPCMovement.WalkResult.Failed)
                {
                    consecutivePathingFailures++;
                }
                else
                {
                    consecutivePathingFailures = 0;
                }
            }
        }
    }
}
