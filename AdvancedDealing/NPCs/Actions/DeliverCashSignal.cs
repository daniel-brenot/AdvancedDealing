using AdvancedDealing.Economy;
using AdvancedDealing.Persistence.Datas;
using MelonLoader;
using System.Collections;
using UnityEngine;
using System;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.Quests;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Economy;
using ScheduleOne.GameTime;
using ScheduleOne.Money;
using ScheduleOne.Quests;
#endif

namespace AdvancedDealing.NPCs.Actions
{
    public class DeliverCashSignal(DealerManager dealerManager) : SignalBase
    {
        private readonly DealerManager _dealerManager = dealerManager;

        private DeadDropManager _deadDropManager;

        private object _deliveryRoutine;

        private object _instantDeliveryRoutine;

        private bool _deadDropIsFull = false;

        protected override string ActionName => "DeliverCash";

        protected override void Awake()
        {
            base.Awake();

            Priority = 90;
        }

        public override void Start()
        {
            base.Start();

            _deadDropManager = DeadDropManager.GetInstance(_dealerManager.DeadDrop);

            if (_deadDropManager == null)
            {
                BeginInstantDelivery();
            }
            else
            {
                if (ModConfig.SkipMovement)
                {
                    BeginDelivery();
                }
                else
                {
                    SetDestination(_deadDropManager.GetPosition());
                }
            }
        }

        public override void End()
        {
            base.End();

            StopRoutines();
        }

        public override void MinPassed()
        {
            base.MinPassed();

            if (!IsActive || _instantDeliveryRoutine != null)
            {
                return;
            }

            if (_dealerManager.Dealer.Cash < _dealerManager.CashThreshold || _deadDropManager.DeadDrop.GUID.ToString() != _dealerManager.DeadDrop || !_dealerManager.DeliverCash || TimeManager.Instance.CurrentTime == 400)
            {
                End();
            }
            else
            {
                if (_deliveryRoutine != null || Movement.IsMoving)
                {
                    return;
                }

                if (IsAtDestination())
                {
                    if (!_deadDropManager.IsFull())
                    {
                        BeginDelivery();
                    }
                    else
                    {
                        End();
                    }
                }
                else
                {
                    SetDestination(_deadDropManager.GetPosition());
                }
            }
        }

        private void BeginDelivery()
        {
            _deliveryRoutine ??= MelonCoroutines.Start(DeliveryRoutine());

            IEnumerator DeliveryRoutine()
            {
                float cash = _dealerManager.Dealer.Cash;

                Movement.FaceDirection(_deadDropManager.GetPosition());

                yield return new WaitForSeconds(2f);

                _dealerManager.Dealer.SetAnimationTrigger("GrabItem");
                _deadDropManager.DeadDrop.Storage.InsertItem(MoneyManager.Instance.GetCashInstance(cash));
                _dealerManager.SendMessage($"I've put ${cash:F0} inside the dead drop at {_deadDropManager.DeadDrop.name}.", _dealerManager.NotifyOnCashDelivery);

                if (_dealerManager.NotifyOnCashDelivery)
                {
                    DeaddropQuest quest = NetworkSingleton<QuestManager>.Instance.CreateDeaddropCollectionQuest(_deadDropManager.DeadDrop.GUID.ToString());

                    if (quest != null)
                    {
                        quest.Description = $"Collect cash at {_deadDropManager.DeadDrop.DeadDropDescription}";
                        quest.Entries[0].SetEntryTitle($"{_dealerManager.Dealer.name}'s cash delivery at {_deadDropManager.DeadDrop.DeadDropName}");
                    }
                }

                _dealerManager.Dealer.ChangeCash(-cash);

                Utils.Logger.Debug("ScheduleManager", $"Cash from {_dealerManager.Dealer.fullName} delivered successfully");

                yield return new WaitUntil((Func<bool>)(() => _dealerManager.Dealer.Cash < _dealerManager.CashThreshold));

                End();
            }
        }

        private void BeginInstantDelivery()
        {
            _instantDeliveryRoutine ??= MelonCoroutines.Start(InstantDeliveryRoutine());

            IEnumerator InstantDeliveryRoutine()
            {
                float cash = _dealerManager.Dealer.Cash;

                NetworkSingleton<MoneyManager>.Instance.ChangeCashBalance(+cash, true, true);
                _dealerManager.SendMessage($"Sent you ${cash:F0} from my earnings.", _dealerManager.NotifyOnCashDelivery);
                _dealerManager.Dealer.ChangeCash(-cash);

                yield return new WaitUntil((Func<bool>)(() => _dealerManager.Dealer.Cash < _dealerManager.CashThreshold));

                End();
            }
        }

        private bool IsAtDestination()
        {
            if (_deadDropManager == null)
            {
                return true;
            }

            return Vector3.Distance(Movement.FootPosition, _deadDropManager.GetPosition()) < 2f;
        }

        private void StopRoutines()
        {
            if (_deliveryRoutine != null)
            {
                MelonCoroutines.Stop(_deliveryRoutine);
                _deliveryRoutine = null;
            }

            if (_instantDeliveryRoutine != null)
            {
                MelonCoroutines.Stop(_instantDeliveryRoutine);
                _instantDeliveryRoutine = null;
            }
        }

        public override bool ShouldStart()
        {
            if (!_dealerManager.Dealer.IsRecruited || !_dealerManager.DeliverCash || _dealerManager.Dealer.ActiveContracts.Count > 0 || _dealerManager.Dealer.Cash < _dealerManager.CashThreshold || TimeManager.Instance.CurrentTime == 400)
            {
                return false;
            }

            DeadDropManager deadDropManager = DeadDropManager.GetInstance(_dealerManager.DeadDrop);

            if (deadDropManager != null && deadDropManager.IsFull())
            {
                if (!_deadDropIsFull)
                {
                    _deadDropIsFull = true;
                    _dealerManager.SendMessage($"Could not deliver cash to dead drop {deadDropManager.DeadDrop.DeadDropName}. There is no space inside!", _dealerManager.NotifyOnCashDelivery);
                }
                return false;
            }

            return base.ShouldStart();
        }
    }
}
