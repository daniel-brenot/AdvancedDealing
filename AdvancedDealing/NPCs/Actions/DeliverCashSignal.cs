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
    public class DeliverCashSignal(Dealer dealer) : SignalBase
    {
        private readonly Dealer m_dealer = dealer;

        private DeadDrop m_deadDrop;

        private object m_deliveryRoutine;

        private object m_instantDeliveryRoutine;

        private bool m_deadDropIsFull = false;

        protected override string ActionName => "DeliverCash";

        protected override void Awake()
        {
            base.Awake();

            Priority = 90;
        }

        public override void Start()
        {
            m_deadDrop = DealerManager.GetDeadDrop(m_dealer);

            base.Start();

            if (m_deadDrop == null)
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
                    SetDestination(DeadDropManager.GetPosition(m_deadDrop));
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

            DealerManager dealerManager = DealerManager.GetManager(m_dealer);

            if (!IsActive || m_instantDeliveryRoutine != null)
            {
                return;
            }

            if (m_dealer.Cash < dealerManager.DealerData.CashThreshold || m_deadDrop != DealerManager.GetDeadDrop(m_dealer) || !dealerManager.DealerData.DeliverCash || TimeManager.Instance.CurrentTime == 400)
            {
                End();
            }
            else
            {
                if (m_deliveryRoutine != null || Movement.IsMoving)
                {
                    return;
                }

                if (IsAtDestination())
                {
                    if (!DeadDropManager.IsFull(m_deadDrop))
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
                    SetDestination(DeadDropManager.GetPosition(m_deadDrop));
                }
            }
        }

        private void BeginDelivery()
        {
            m_deliveryRoutine ??= MelonCoroutines.Start(DeliveryRoutine());

            IEnumerator DeliveryRoutine()
            {
                DealerManager dealerManager = DealerManager.GetManager(m_dealer);
                float cash = m_dealer.Cash;

                Movement.FaceDirection(DeadDropManager.GetPosition(m_deadDrop));

                yield return new WaitForSeconds(2f);

                m_dealer.SetAnimationTrigger("GrabItem");
                m_deadDrop.Storage.InsertItem(MoneyManager.Instance.GetCashInstance(cash));
                DealerManager.SendMessage(m_dealer, $"I've put ${cash:F0} inside the dead drop at {m_deadDrop.name}.", dealerManager.DealerData.NotifyOnCashDelivery);

                if (dealerManager.DealerData.NotifyOnCashDelivery)
                {
                    DeaddropQuest quest = NetworkSingleton<QuestManager>.Instance.CreateDeaddropCollectionQuest(m_deadDrop.GUID.ToString());

                    if (quest != null)
                    {
                        quest.Description = $"Collect cash at {m_deadDrop.DeadDropDescription}";
                        quest.Entries[0].SetEntryTitle($"{m_dealer.name}'s cash delivery at {m_deadDrop.DeadDropName}");
                    }
                }

                m_dealer.ChangeCash(-cash);

                Utils.Logger.Debug("ScheduleManager", $"Cash from dealer delivered successfully: {m_dealer.GUID}");

                yield return new WaitUntil((Func<bool>)(() => m_dealer.Cash < dealerManager.DealerData.CashThreshold));

                End();
            }
        }

        private void BeginInstantDelivery()
        {
            m_deliveryRoutine ??= MelonCoroutines.Start(InstantDeliveryRoutine());

            IEnumerator InstantDeliveryRoutine()
            {
                DealerManager dealerManager = DealerManager.GetManager(m_dealer);
                float cash = m_dealer.Cash;

                NetworkSingleton<MoneyManager>.Instance.ChangeCashBalance(+cash, true, true);
                DealerManager.SendMessage(m_dealer, $"Sent you ${cash:F0} from my earnings.", dealerManager.DealerData.NotifyOnCashDelivery);
                m_dealer.ChangeCash(-cash);

                yield return new WaitUntil((Func<bool>)(() => m_dealer.Cash < dealerManager.DealerData.CashThreshold));

                End();
            }
        }

        private bool IsAtDestination()
        {
            if (m_deadDrop == null)
            {
                return true;
            }

            return Vector3.Distance(Movement.FootPosition, DeadDropManager.GetPosition(m_deadDrop)) < 2f;
        }

        private void StopRoutines()
        {
            if (m_deliveryRoutine != null)
            {
                MelonCoroutines.Stop(m_deliveryRoutine);
                m_deliveryRoutine = null;
            }

            if (m_instantDeliveryRoutine != null)
            {
                MelonCoroutines.Stop(m_instantDeliveryRoutine);
                m_instantDeliveryRoutine = null;
            }
        }

        public override bool ShouldStart()
        {
            DealerManager dealerManager = DealerManager.GetManager(m_dealer);

            if (!m_dealer.IsRecruited || !dealerManager.DealerData.DeliverCash || m_dealer.ActiveContracts.Count > 0 || m_dealer.Cash < dealerManager.DealerData.CashThreshold || TimeManager.Instance.CurrentTime == 400)
            {
                return false;
            }

            DeadDrop deadDrop = DealerManager.GetDeadDrop(m_dealer);

            if ((deadDrop != null) && DeadDropManager.IsFull(deadDrop))
            {
                if (!m_deadDropIsFull)
                {
                    m_deadDropIsFull = true;
                    DealerManager.SendMessage(m_dealer, $"Could not deliver cash to dead drop {deadDrop.name}. There is no space inside!", dealerManager.DealerData.NotifyOnCashDelivery);
                }
                return false;
            }

            return base.ShouldStart();
        }
    }
}
