using AdvancedDealing.Persistence.Datas;
using Newtonsoft.Json;
using System;

#if IL2CPP
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Networking;
using Il2CppSteamworks;
using Il2CppSystem.Text;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Economy;
using ScheduleOne.Networking;
using Steamworks;
using System.Text;
#endif

namespace AdvancedDealing.Persistence
{
    public class SyncManager
    {
        private bool m_isRunning;

        private bool m_isHost;

        private CSteamID m_lobbySteamID;

        protected Callback<LobbyChatMsg_t> LobbyChatMsgCallback;

        protected Callback<LobbyDataUpdate_t> LobbyDataUpdateCallback;

        public static bool IsActive => (Instance.m_isRunning && SaveManager.Instance.SavegameLoaded);

        public static bool NoSyncOrActiveAndHost => (!IsActive || (IsActive && Instance.m_isHost));

        public static SyncManager Instance { get; private set; }

        public SyncManager()
        {
            if (Instance == null)
            {
                Singleton<Lobby>.Instance.onLobbyChange += new Action(OnLobbyChange);

                LobbyChatMsgCallback = Callback<LobbyChatMsg_t>.Create((Callback<LobbyChatMsg_t>.DispatchDelegate)OnLobbyChatMsg);
                LobbyDataUpdateCallback = Callback<LobbyDataUpdate_t>.Create((Callback<LobbyDataUpdate_t>.DispatchDelegate)OnLobbyDataUpdate);

                Instance = this;
            }
        }

        private void Start()
        {
            m_isHost = false;
            m_lobbySteamID = Singleton<Lobby>.Instance.LobbySteamID;
            m_isRunning = true;

            Utils.Logger.Msg("SyncManager", "Synchronization started");
        }

        private void Stop()
        {
            m_isHost = false;
            m_lobbySteamID = CSteamID.Nil;
            m_isRunning = false;

            Utils.Logger.Msg("SyncManager", "Synchronization stopped");
        }

        public void SetAsHost() =>
            m_isHost = true;

        public void PushUpdate()
        {
            if (!IsActive) return;

            if (m_isHost)
            {
                SyncDataAsServer();
            }
            else
            {
                SyncDataAsClient();
            }
        }

        private void SyncDataAsServer(string dataString = null)
        {
            if (!IsActive || !m_isHost) return;

            string key = ModInfo.k_Name;
            dataString ??= JsonConvert.SerializeObject(SaveManager.Instance.SaveData);

            SteamMatchmaking.SetLobbyData(m_lobbySteamID, key, dataString);

            Utils.Logger.Debug("SyncManager", "Sent data update to clients");
        }

        private void SyncDataAsClient()
        {
            if (!IsActive || m_isHost) return;

            string key = ModInfo.k_Name;
            string dataString = JsonConvert.SerializeObject(SaveManager.Instance.SaveData);

            SteamMatchmaking.SetLobbyMemberData(m_lobbySteamID, key, dataString);

            Utils.Logger.Debug("SyncManager", "Sent data update to server");
        }

        private void OnLobbyDataUpdate(LobbyDataUpdate_t res)
        {
            if (!IsActive) return;

            string data;

            if (res.m_ulSteamIDMember == res.m_ulSteamIDLobby)
            {
                if (m_isHost) return;

                Utils.Logger.Debug("SyncManager", "Receiving data update from server ...");

                data = SteamMatchmaking.GetLobbyData(m_lobbySteamID, ModInfo.k_Name);
            }
            else
            {
                if (!m_isHost) return;

                Utils.Logger.Debug("SyncManager", "Receiving data update from client ...");

                data = SteamMatchmaking.GetLobbyMemberData(m_lobbySteamID, (CSteamID)res.m_ulSteamIDMember, ModInfo.k_Name);
            }

            if (data == null)
            {
                Utils.Logger.Error("SyncManager", "Could not fetch data");
                return;
            }

            string currentData = JsonConvert.SerializeObject(SaveManager.Instance.SaveData);

            if (currentData != data)
            {
                SaveManager.Instance.UpdateSaveData(JsonConvert.DeserializeObject<SaveData>(data));

                if (m_isHost)
                {
                    SyncDataAsServer(data);
                }

                Utils.Logger.Debug("SyncManager", "Data synchronised");
            }
        }

        public bool FetchDataFromLobby()
        {
            string data = SteamMatchmaking.GetLobbyData(m_lobbySteamID, ModInfo.k_Name);

            if (data == null)
            {
                return false;
            }

            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(data);
            SaveManager.Instance.UpdateSaveData(saveData);

            return true;
        }

        public void SendDataUpdateRequest()
        {
            string text = $"{ModInfo.k_Name}__data_request";
#if IL2CPP
            Il2CppStructArray<byte> bytes = Encoding.ASCII.GetBytes(text);
#elif MONO
            byte[] bytes = Encoding.ASCII.GetBytes(text);
#endif

            SteamMatchmaking.SendLobbyChatMsg(m_lobbySteamID, bytes, bytes.Length);
        }

        private void OnLobbyChatMsg(LobbyChatMsg_t res)
        {
            if (!IsActive) return;

#if IL2CPP
            Il2CppStructArray<byte> bytes = new byte[4096];
#elif MONO
            byte[] bytes = new byte[4096];
#endif

            SteamMatchmaking.GetLobbyChatEntry(m_lobbySteamID, (int)res.m_iChatID, out CSteamID userSteamID, bytes, bytes.Length, out _);

            string text = Encoding.ASCII.GetString(bytes);
            text = text.TrimEnd(new char[1]);
            string[] data = text.Split("__");

            if (data[0] == ModInfo.k_Name)
            {
                Utils.Logger.Debug("SyncManager", $"Received msg: {data[1]}");

                switch (data[1])
                {
                    case "data_request":
                        if (m_isHost)
                        {
                            PushUpdate();
                        }
                        break;
                }
            }
        }

        private void OnLobbyChange()
        {
            if (Singleton<Lobby>.Instance.IsInLobby && m_isRunning && Singleton<Lobby>.Instance.LobbySteamID != m_lobbySteamID)
            {
                m_lobbySteamID = Singleton<Lobby>.Instance.LobbySteamID;
            }
            else if (Singleton<Lobby>.Instance.IsInLobby && !m_isRunning)
            {
                Start();
            }
            else if (!Singleton<Lobby>.Instance.IsInLobby && m_isRunning)
            {
                Stop();
            }
        }
    }
}
