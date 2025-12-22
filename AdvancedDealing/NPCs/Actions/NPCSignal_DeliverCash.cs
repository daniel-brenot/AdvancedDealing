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
    public class NPCSignal_DeliverCash(Dealer dealer) : NPCSignal
    {
        private readonly Dealer _dealer = dealer;

        private DeadDrop _deadDrop;

        private object _deliveryRoutine;

        private object _instantDeliveryRoutine;

        private bool _deadDropIsFull = false;

        protected override string ActionName =>
            "DeliverCash";

        protected override void Awake()
        {
            base.Awake();

            priority = 90;
        }

        public override void Start()
        {
            _deadDrop = DealerManager.GetDeadDrop(_dealer);

            base.Start();

            if (_deadDrop == null)
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
                    SetDestination(DeadDropManager.GetPosition(_deadDrop));
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

            DealerData dealerData = DealerManager.GetData(_dealer);

            if (!IsActive || _instantDeliveryRoutine != null)
            {
                return;
            }

            if (_dealer.Cash < dealerData.CashThreshold || _deadDrop != DealerManager.GetDeadDrop(_dealer) || !dealerData.DeliverCash || TimeManager.Instance.CurrentTime == 400)
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
                    if (!DeadDropManager.IsFull(_deadDrop))
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
                    SetDestination(DeadDropManager.GetPosition(_deadDrop));
                }
            }
        }

        private void BeginDelivery()
        {
            _deliveryRoutine ??= MelonCoroutines.Start(DeliveryRoutine());

            IEnumerator DeliveryRoutine()
            {
                DealerData dealerData = DealerManager.GetData(_dealer);
                float cash = _dealer.Cash;

                Movement.FaceDirection(DeadDropManager.GetPosition(_deadDrop));

                yield return new WaitForSeconds(2f);

                _dealer.SetAnimationTrigger("GrabItem");
                _deadDrop.Storage.InsertItem(MoneyManager.Instance.GetCashInstance(cash));
                DealerManager.SendMessage(_dealer, $"I've put ${cash:F0} inside the dead drop at {_deadDrop.name}.", dealerData.NotifyOnCashDelivery);

                if (dealerData.NotifyOnCashDelivery)
                {
                    DeaddropQuest quest = NetworkSingleton<QuestManager>.Instance.CreateDeaddropCollectionQuest(_deadDrop.GUID.ToString());

                    if (quest != null)
                    {
                        quest.Description = $"Collect cash at {_deadDrop.DeadDropDescription}";
                        quest.Entries[0].SetEntryTitle($"{_dealer.name}'s cash delivery at {_deadDrop.DeadDropName}");
                    }
                }

                _dealer.ChangeCash(-cash);

                Utils.Logger.Debug("ScheduleManager", $"Cash from {_dealer.name} delivered successfully.");

                yield return new WaitUntil((Func<bool>)(() => _dealer.Cash < dealerData.CashThreshold));

                End();
            }
        }

        private void BeginInstantDelivery()
        {
            _deliveryRoutine ??= MelonCoroutines.Start(InstantDeliveryRoutine());

            IEnumerator InstantDeliveryRoutine()
            {
                DealerData dealerData = DealerManager.GetData(_dealer);
                float cash = _dealer.Cash;

                NetworkSingleton<MoneyManager>.Instance.ChangeCashBalance(+cash, true, true);
                DealerManager.SendMessage(_dealer, $"Sent you ${cash:F0} from my earnings.", dealerData.NotifyOnCashDelivery);
                _dealer.ChangeCash(-cash);

                yield return new WaitUntil((Func<bool>)(() => _dealer.Cash < dealerData.CashThreshold));

                End();
            }
        }

        private bool IsAtDestination()
        {
            if (_deadDrop == null)
            {
                return true;
            }

            return Vector3.Distance(Movement.FootPosition, DeadDropManager.GetPosition(_deadDrop)) < 2f;
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
            DealerData dealerData = DealerManager.GetData(_dealer);

            if (!_dealer.IsRecruited || !dealerData.DeliverCash || _dealer.ActiveContracts.Count > 0 || _dealer.Cash < dealerData.CashThreshold || TimeManager.Instance.CurrentTime == 400)
            {
                return false;
            }

            DeadDrop deadDrop = DealerManager.GetDeadDrop(_dealer);

            if ((deadDrop != null) && DeadDropManager.IsFull(deadDrop))
            {
                if (!_deadDropIsFull)
                {
                    _deadDropIsFull = true;
                    DealerManager.SendMessage(_dealer, $"Could not deliver cash to dead drop {deadDrop.name}. There is no space inside!", dealerData.NotifyOnCashDelivery);
                }
                return false;
            }

            return base.ShouldStart();
        }
    }
}
