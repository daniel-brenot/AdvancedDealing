using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI.Phone.Messages;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.UI.Phone.Messages;
#endif

namespace AdvancedDealing.UI
{
    public class CustomersScrollView
    {
        public const int MAX_ENTRIES = 24;

        public GameObject Container;

        public GameObject Viewport;

        public GameObject AssignButton;

        public List<GameObject> CustomerEntries = [];

        public Text TitleLabel;

        public bool UICreated { get; private set; }

        public void BuildUI()
        {
            Container = PlayerSingleton<DealerManagementApp>.Instance.transform.Find("Container/Background/Content/Container").gameObject;

            GameObject scroll = new("Scroll");
            RectTransform transform = scroll.AddComponent<RectTransform>();
            transform.SetParent(Container.transform.parent, false);
            transform.anchorMin = new Vector2(0f, 0f);
            transform.anchorMax = new Vector2(1f, 1f);
            transform.pivot = new Vector2(0.5f, 0.5f);
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = new Vector2(0f, 0f);

            Viewport = new("Viewport");
            RectTransform transform2 = Viewport.AddComponent<RectTransform>();
            transform2.SetParent(transform, false);
            transform2.anchorMin = new Vector2(0f, 0f);
            transform2.anchorMax = new Vector2(1f, 1f);
            transform2.pivot = new Vector2(0.5f, 0.5f);
            transform2.anchoredPosition = Vector2.zero;
            transform2.sizeDelta = new Vector2(0f, 0f);
            transform2.offsetMax = new Vector2(0f, -90f);
            Viewport.AddComponent<Mask>().showMaskGraphic = false;
            Viewport.AddComponent<Image>();

            RectTransform transform3 = Container.GetComponent<RectTransform>();
            transform3.SetParent(transform2, true);
            transform3.pivot = new Vector2(0.5f, 1f);

            VerticalLayoutGroup verticalLayoutGroup = Container.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.padding = new RectOffset(40, 40, 0, 20);

            ContentSizeFitter contentSizeFitter = Container.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scrollRect = scroll.AddComponent<ScrollRect>();
            scrollRect.viewport = transform2;
            scrollRect.content = transform3;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.inertia = true;
            scrollRect.elasticity = 0.1f;
            scrollRect.verticalNormalizedPosition = 1f;
            scrollRect.scrollSensitivity = 8f;

            TitleLabel = Container.transform.Find("CustomerTitle").GetComponent<Text>();
            AssignButton = PlayerSingleton<DealerManagementApp>.Instance.AssignCustomerButton.gameObject;

            CreateCustomerEntries();

            Utils.Logger.Debug("CustomersScrollView", "Customers scroll view UI created");

            UICreated = true;
        }

        private void CreateCustomerEntries()
        {
#if IL2CPP
            Il2CppReferenceArray<RectTransform> currentEntries = PlayerSingleton<DealerManagementApp>.Instance.CustomerEntries;
            Il2CppReferenceArray<RectTransform> entries = new(MAX_ENTRIES);
#elif MONO
            RectTransform[] currentEntries = PlayerSingleton<DealerManagementApp>.Instance.CustomerEntries;
            RectTransform[] entries = new RectTransform[MAX_ENTRIES];
#endif
            int currentCount = currentEntries.Length;

            if (currentCount == MAX_ENTRIES) return;

            for (int i = 0; i < MAX_ENTRIES; i++)
            {
                if (i < currentCount)
                {
                    entries[i] = currentEntries[i];
                    CustomerEntries.Add(currentEntries[i].gameObject);
                }
                else
                {
                    RectTransform last = currentEntries.Last<RectTransform>();

                    RectTransform newEntry = Object.Instantiate<RectTransform>(last, last.parent);
                    newEntry.name = $"CustomerEntry ({i})";
                    newEntry.gameObject.SetActive(false);

                    entries[i] = newEntry;
                    CustomerEntries.Add(newEntry.gameObject);
                }
            }

            PlayerSingleton<DealerManagementApp>.Instance.CustomerEntries = entries;
        }
    }
}
