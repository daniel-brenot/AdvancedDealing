using AdvancedDealing;
using AdvancedDealing.Persistence;
using MelonLoader;
using UnityEngine.Events;
using SaveModifier = AdvancedDealing.Persistence.SaveModifier;
using System;


#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Persistence;
#endif

[assembly: MelonInfo(typeof(AdvancedDealing.AdvancedDealing), $"{ModInfo.Name}", ModInfo.Version, ModInfo.Author, ModInfo.DownloadLink)]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: MelonColor(255, 113, 195, 230)]
#if IL2CPP
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
#elif MONO
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
#endif

namespace AdvancedDealing
{
    public class AdvancedDealing : MelonMod
    {
        public bool IsInitialized { get; private set; }

        public SaveModifier SaveModifier { get; private set; }

        public NetworkSynchronizer NetworkSynchronizer { get; private set; }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        { 
            if (sceneName == "Menu")
            {
                if (!IsInitialized)
                {
                    ModConfig.Initialize();

                    SaveModifier = new();
                    NetworkSynchronizer = new();

                    Utils.Logger.Msg($"{ModInfo.Name} v{ModInfo.Version} initialized");

                    IsInitialized = true;
                }

                if (SaveModifier.SavegameLoaded)
                {
                    SaveModifier.ClearModifications();
                }
            }
            else if (sceneName == "Main")
            {
                SaveModifier.LoadModifications();
            }
        }
    }
}
