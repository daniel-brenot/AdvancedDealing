using AdvancedDealing.Economy;
using AdvancedDealing.NPCs;
using AdvancedDealing.Persistence.Datas;
using AdvancedDealing.UI;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;



#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Networking;
using S1SaveManager = Il2CppScheduleOne.Persistence.SaveManager;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Economy;
using ScheduleOne.Networking;
using S1SaveManager = ScheduleOne.Persistence.SaveManager;
#endif

namespace AdvancedDealing.Persistence
{
    public class SaveManager
    {
        public static SaveManager Instance { get; private set; }

        public SaveData SaveData { get; private set; }

        public bool SavegameLoaded { get; private set; }

        public SaveManager()
        {
            if (Instance == null)
            {
                Singleton<S1SaveManager>.Instance.onSaveComplete.AddListener((UnityAction)OnSaveComplete);
                Instance = this;
            }
        }

        public void LoadSavegame()
        {
            Utils.Logger.Msg("Preparing savegame modifications...");

            if (Singleton<Lobby>.Instance.IsInLobby && !Singleton<Lobby>.Instance.IsHost)
            {
                MelonCoroutines.Start(ClientLoadRoutine());

                IEnumerator ClientLoadRoutine()
                {
                    SaveData = new("temporary");
                    SaveData.SetDefaults();

                    DeadDropManager.Initialize();
                    DealerManager.Initialize();

                    yield return new WaitForSecondsRealtime(2f);

                    SavegameLoaded = true;

                    SyncManager.Instance.SendMessage("data_request");

                    UIInjector.Inject();

                    Utils.Logger.Msg("Savegame modifications successfully injected");
                }
            }
            else
            {
                MelonCoroutines.Start(LoadRoutine());

                IEnumerator LoadRoutine()
                {
                    SaveData = null;
                    SaveData = FileManager.LoadFromFile();

                    while (SaveData == null)
                    {
                        yield return new WaitForSecondsRealtime(2f);
                    }

                    DeadDropManager.Initialize();
                    DealerManager.Initialize();

                    yield return new WaitForSecondsRealtime(2f);

                    SavegameLoaded = true;

                    if (SyncManager.IsSyncing)
                    {
                        SyncManager.Instance.SetAsHost();
                    }

                    UIInjector.Inject();

                    Utils.Logger.Msg("Savegame modifications successfully injected");
                }
            }
        }

        public void ClearSavegame()
        {
            UIInjector.Reset();
            ScheduleManager.ClearAll();

            SaveData = null;
            SavegameLoaded = false;

            Utils.Logger.Msg($"Savegame modifications cleared");
        }

        public void UpdateSaveData(SaveData saveData, bool isSaveDataRequest = false)
        {
            if (!isSaveDataRequest)
            {
                foreach (DealerData dealerData in saveData.Dealers)
                {
                    DealerManager manager = DealerManager.GetInstance(dealerData.Identifier);
                    manager.PatchData(dealerData);
                    manager.HasChanged = true;
                }
            }

            SaveData = saveData;
        }

        public void CollectData()
        {
            foreach (Dealer dealer in Dealer.AllPlayerDealers)
            {
                if (DealerManager.DealerExists(dealer))
                {
                    DealerManager dealerManager = DealerManager.GetInstance(dealer);
                    DealerData dealerData = SaveData.Dealers.Find(x => x.Identifier.Contains(dealer.GUID.ToString()));

                    if (dealerData != null)
                    {
                        SaveData.Dealers.Remove(dealerData);
                    }

                    SaveData.Dealers.Add(dealerManager.FetchData());
                }
            }
        }

        public void UpdateData(DealerData dealerData = null)
        {
            if (dealerData != null)
            {
                DealerData oldDealerData = SaveData.Dealers.Find(x => x.Identifier.Contains(dealerData.Identifier));

                if (oldDealerData != null)
                {
                    SaveData.Dealers.Remove(oldDealerData);
                }

                SaveData.Dealers.Add(dealerData);

                Utils.Logger.Debug("SaveManager", $"Dealer data updated: {dealerData.Identifier}");
            }
        }

        private void OnSaveComplete()
        {
            if (SyncManager.IsNoSyncOrHost)
            {
                CollectData();
                FileManager.SaveToFile(SaveData);
            }
        }
    }
}
