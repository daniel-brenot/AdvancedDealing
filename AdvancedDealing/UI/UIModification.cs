using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace AdvancedDealing.UI
{
    public class UIModification
    {
        public static bool IsLoaded { get; private set; }

        public static SettingsPopup SettingsPopup { get; private set; }

        public static DeadDropSelector DeadDropSelector { get; private set; }

        public static CustomersScrollView CustomersScrollView { get; private set; }

        public static void Load()
        {
            if (IsLoaded) return;

            SettingsPopup ??= new();
            DeadDropSelector ??= new();
            CustomersScrollView ??= new();

            MelonCoroutines.Start(CreateUI());

            IEnumerator CreateUI()
            {
                yield return new WaitUntil((Func<bool>)(() => !PersistentSingleton<LoadManager>.Instance.IsLoading && PersistentSingleton<LoadManager>.Instance.IsGameLoaded));

                SettingsPopup.CreateUI();
                DeadDropSelector.CreateUI();
                CustomersScrollView.CreateUI();

                Utils.Logger.Msg("UI elements created");

                IsLoaded = true;
            }
        }

        public static void Clear()
        {
            SettingsPopup = null;
            DeadDropSelector = null;
            CustomersScrollView = null;

            IsLoaded = false;
        }
    }
}
