using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.Linq;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Map;
using Il2CppScheduleOne.UI.Phone.Messages;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Economy;
using ScheduleOne.Map;
using ScheduleOne.UI.Phone.Messages;
#endif

namespace AdvancedDealing.UI
{
    public class CustomerSelector
    {
        public RectTransform Content;

        public InputField Searchbar;

        private Dictionary<EMapRegion, (GameObject gameObject, List<RectTransform> entries)> _categories = [];

        public bool UICreated { get; private set; }

        public void BuildUI()
        {
            if (UICreated) return;

            Transform customerSelector = PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector.transform;
            Content = PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector.EntriesContainer;

            RectTransform transform = customerSelector.Find("Shade/Content/Scroll View")?.GetComponent<RectTransform>();
            transform.sizeDelta = new Vector2(transform.sizeDelta.x, transform.sizeDelta.y - 100f);

            GameObject searchbar = Object.Instantiate(PlayerSingleton<MessagesApp>.Instance.CounterofferInterface.transform.Find("Shade/Content/Selection/SearchInput").gameObject, transform.parent);
            searchbar.SetActive(true);
            searchbar.name = "Searchbar";
            
            Searchbar = searchbar.GetComponent<InputField>();
            Searchbar.onEndEdit.RemoveAllListeners();
            Searchbar.onValueChanged.RemoveAllListeners();
            Searchbar.onValueChanged.AddListener((UnityAction<string>)OnSearchValueChanged);
            Searchbar.contentType = InputField.ContentType.Standard;

            RectTransform transform2 = Searchbar.GetComponent<RectTransform>();
            transform2.offsetMax = new Vector2(-25f, -80f);
            transform2.offsetMin = new Vector2(25f, -130f);
            Text placeholder = transform2.Find("Text Area/Placeholder").GetComponent<Text>();
            placeholder.text = "Search customers...";

            GameObject textTemplate = Content.GetChild(0).Find("Name").gameObject;

            foreach (EMapRegion mapRegion in Enum.GetValues(typeof(EMapRegion)))
            {
                GameObject region = Object.Instantiate(textTemplate, Content);
                region.name = "Region";

                RectTransform transform3 = region.GetComponent<RectTransform>();
                transform3.offsetMax = new Vector2(0f, 0f);
                transform3.offsetMin = new Vector2(0f, 0f);
                transform3.sizeDelta = new Vector2(495f, 60f);

                Text text = region.GetComponent<Text>();
                text.text = Enum.GetName(typeof(EMapRegion), mapRegion);
                text.fontStyle = FontStyle.Bold;
                text.alignment = TextAnchor.MiddleCenter;

                _categories.Add(mapRegion, (region, new List<RectTransform>()));
            }

            UICreated = true;
        }

        public void SortCustomers()
        {
#if IL2CPP
            for (int i = 0; i < PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector.customerEntries.Count; i++)
            {
                RectTransform entry = PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector.customerEntries[i];
                Customer customer = PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector.entryToCustomer[entry];
#elif MONO
            List<RectTransform> customerEntries = (List<RectTransform>)typeof(CustomerSelector).GetField("customerEntries", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector);
            Dictionary<RectTransform, Customer> entryToCustomer = (Dictionary<RectTransform, Customer>)typeof(CustomerSelector).GetField("entryToCustomer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector);

            for (int i = 0; i < customerEntries.Count; i++)
            {
                RectTransform entry = customerEntries[i];
                Customer customer = entryToCustomer[entry];
#endif

                if (customer.AssignedDealer != null)
                {
                    _categories[customer.NPC.Region].entries.Remove(entry);
                }
                else
                {
                    _categories[customer.NPC.Region].entries.Add(entry);
                }
            }

            foreach (KeyValuePair<EMapRegion, (GameObject gameObject, List<RectTransform> entries)> category in _categories)
            {
                category.Value.gameObject.transform.SetAsLastSibling();

                List<RectTransform> sortedList = category.Value.entries.OrderBy(e => e.Find("Name").GetComponent<Text>().text).ToList();
                category.Value.entries.Clear();
                category.Value.entries.AddRange(sortedList);

                int activeCount = 0;

                for (int i = 0; i < category.Value.entries.Count; i++)
                {
                    RectTransform entry = category.Value.entries[i];
                    entry.SetAsLastSibling();

                    if (entry.gameObject.activeSelf)
                    {
                        activeCount++;
                    }
                }

                if (activeCount == 0)
                {
                    category.Value.gameObject.SetActive(false);
                }
                else
                {
                    category.Value.gameObject.SetActive(true);
                }
            }
        }

        private void OnSearchValueChanged(string value)
        {
            foreach (KeyValuePair<EMapRegion, (GameObject gameObject, List<RectTransform> entries)> category in _categories)
            {
                int activeCount = 0;

                for (int i = 0; i < category.Value.entries.Count; i++)
                {
                    RectTransform entry = category.Value.entries[i];
                    Text name = entry.Find("Name").GetComponent<Text>();

                    if (name.text.Contains(value, StringComparison.OrdinalIgnoreCase) || value == null || value == string.Empty)
                    {
                        entry.gameObject.SetActive(true);
                        activeCount++;
                    }
                    else
                    {
                        entry.gameObject.SetActive(false);
                    }
                }

                if (activeCount == 0)
                {
                    category.Value.gameObject.SetActive(false);
                }
                else
                {
                    category.Value.gameObject.SetActive(true);
                }
            }
        }
    }
}
