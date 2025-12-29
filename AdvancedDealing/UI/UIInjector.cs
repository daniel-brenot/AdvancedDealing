using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Persistence;
#endif

namespace AdvancedDealing.UI
{
    public class UIInjector
    {
        public static bool IsInjected { get; private set; }

        public static SettingsPopup SettingsPopup { get; private set; }

        public static SliderPopup SliderPopup { get; private set; }

        public static DeadDropSelector DeadDropSelector { get; private set; }

        public static CustomersScrollView CustomersScrollView { get; private set; }

        public static void Inject()
        {
            if (!IsInjected)
            {

                SettingsPopup ??= new();
                SliderPopup ??= new();
                DeadDropSelector ??= new();
                CustomersScrollView ??= new();

                MelonCoroutines.Start(InjectUI());

                IEnumerator InjectUI()
                {
                    yield return new WaitUntil((Func<bool>)(() => !PersistentSingleton<LoadManager>.Instance.IsLoading && PersistentSingleton<LoadManager>.Instance.IsGameLoaded));

                    SettingsPopup.CreateUI();
                    SliderPopup.CreateUI();
                    DeadDropSelector.CreateUI();
                    CustomersScrollView.CreateUI();

                    Utils.Logger.Msg("UI elements created");

                    IsInjected = true;
                }
            }
        }

        public static void Reset()
        {
            SettingsPopup = null;
            SliderPopup = null;
            DeadDropSelector = null;
            CustomersScrollView = null;

            IsInjected = false;
        }
    }
}
