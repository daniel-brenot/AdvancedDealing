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

        private readonly List<RectTransform> _entries = [];

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

            Utils.Logger.Debug("CustomerSelector", "Customer selector UI created");

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
            List<RectTransform> customerEntries = (List<RectTransform>)typeof(ScheduleOne.UI.Phone.CustomerSelector).GetField("customerEntries", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector);
            Dictionary<RectTransform, Customer> entryToCustomer = (Dictionary<RectTransform, Customer>)typeof(ScheduleOne.UI.Phone.CustomerSelector).GetField("entryToCustomer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector);

            for (int i = 0; i < customerEntries.Count; i++)
            {
                RectTransform entry = customerEntries[i];
                Customer customer = entryToCustomer[entry];
#endif

                if (customer.AssignedDealer != null)
                {
                    _entries.Remove(entry);
                }
                else
                {
                    _entries.Add(entry);
                }
            }
            
            List<RectTransform> sortedList = _entries.OrderBy(e => e.Find("Name").GetComponent<Text>().text).ToList();
            _entries.Clear();
            _entries.AddRange(sortedList);

            for (int i = 0; i < _entries.Count; i++)
            {
                RectTransform entry = _entries[i];
                entry.SetAsLastSibling();
            }
        }

        private void OnSearchValueChanged(string value)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                RectTransform entry = _entries[i];
                Text name = entry.Find("Name").GetComponent<Text>();

                if (name.text.Contains(value, StringComparison.OrdinalIgnoreCase) || value == null || value == string.Empty)
                {
                    entry.gameObject.SetActive(true);
                }
                else
                {
                    entry.gameObject.SetActive(false);
                }
            }
        }
    }
}
