using AdvancedDealing.Economy;
using AdvancedDealing.Persistence;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI.Phone.Messages;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Economy;
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI.Phone.Messages;
#endif

namespace AdvancedDealing.UI
{
    public class DeadDropSelector
    {
        public GameObject Container;

        public Text TitleLabel;

        public Transform Content;

        public GameObject Button;

        public Text ButtonLabel;

        private readonly List<GameObject> _selectables = [];

        private DealerExtension _dealer;

        private GameObject _selectableTemplate;

        public bool UICreated { get; private set; }

        public bool IsOpen { get; private set; }

        public void Open(DealerExtension dealerExtension)
        {
            IsOpen = true;
            _dealer = dealerExtension;

            foreach (DeadDrop deadDrop in DeadDropExtension.GetDeadDropsByDistance(Player.Local.transform))
            {
                GameObject selectable = _selectables.Find(x => x.transform.Find("Name").GetComponent<Text>().text == deadDrop.DeadDropName);
                selectable.transform.SetAsLastSibling();
            }

            Container.SetActive(true);
        }

        public void Close()
        {
            IsOpen = false;
            Container.SetActive(false);
        }

        private void OnSelected(string guid, string name)
        {
            _dealer.DeadDrop = guid;
            ButtonLabel.text = name;

            Utils.Logger.Debug("DeadDropSelector", $"Dead drop for {_dealer.Dealer.fullName} selected: {guid}");

            if (NetworkSynchronizer.IsSyncing)
            {
                NetworkSynchronizer.Instance.SendData(_dealer.FetchData());
            }

            Close();
        }

        public void BuildUI()
        {
            if (UICreated) return;

            GameObject target = PlayerSingleton<DealerManagementApp>.Instance.CustomerSelector.gameObject;

            Container = Object.Instantiate(target, target.transform.parent);
            Container.name = "DeadDropSelector";
            Container.SetActive(true);

            CreateSelectableTemplate();

            RectTransform oldContent = Container.transform.Find("Shade/Content/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

            GameObject content = new("Content");
            RectTransform transform = content.AddComponent<RectTransform>();
            transform.SetParent(oldContent.parent, false);
            transform.anchorMin = oldContent.anchorMin;
            transform.anchorMax = oldContent.anchorMax;
            transform.pivot = oldContent.pivot;
            transform.anchoredPosition = oldContent.anchoredPosition;
            transform.sizeDelta = oldContent.sizeDelta;

            TitleLabel = Container.transform.Find("Shade/Content/Title").GetComponent<Text>();
            TitleLabel.text = "Select Dead Drop";

            Content = transform;

            Object.Destroy(oldContent.gameObject);

            Container.transform.Find("Shade/Content/Scroll View").gameObject.GetComponent<ScrollRect>().content = transform;

            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childScaleHeight = false;
            layoutGroup.childScaleWidth = false;

            CreateSelectable(null, "None");

            for (int i = 0; i <= DeadDrop.DeadDrops.Count - 1; i++)
            {
                CreateSelectable(DeadDrop.DeadDrops[i].GUID.ToString(), DeadDrop.DeadDrops[i].DeadDropName);
            }

            CreateButton();

            Utils.Logger.Debug("DeadDropSelector", "Dead drop selector UI created");

            UICreated = true;
        }

        private void CreateButton()
        {
            GameObject target = PlayerSingleton<DealerManagementApp>.Instance.transform.Find("Container/Background/Content/Home").gameObject;

            Button = Object.Instantiate(target, target.transform.parent);
            Button.SetActive(true);
            Button.name = "DeadDropSelectorButton";
            Button.transform.Find("Title").GetComponent<Text>().text = "Dead Drop";

            RectTransform transform = Button.transform.Find("Value").GetComponent<RectTransform>();
            transform.offsetMax = new Vector2(transform.offsetMax.x, 60f);
            transform.offsetMin = new Vector2(transform.offsetMin.x, 15f);
            transform.sizeDelta = new Vector2(transform.sizeDelta.x, 40f);

            ButtonLabel = transform.GetComponent<Text>();
            ButtonLabel.text = "None";
            ButtonLabel.color = new Color(0.6f, 1f, 1f, 1f);

            Button.AddComponent<Button>().onClick.AddListener((UnityAction)OpenSelector);
            
            void OpenSelector()
            {
                Open(DealerExtension.GetExtension(PlayerSingleton<DealerManagementApp>.Instance.SelectedDealer));
            }
        }

        private void CreateSelectableTemplate()
        {
            GameObject template = Object.Instantiate(Container.transform.Find("Shade/Content/Scroll View/Viewport/Content").GetChild(0).gameObject);
            template.SetActive(false);
            template.name = "SelectableTemplate";

            Object.Destroy(template.transform.Find("Mugshot").gameObject);

            template.transform.Find("Name").GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);

            _selectableTemplate = template;
        }

        private void CreateSelectable(string deadDropGuid, string name)
        {
            GameObject selectable = Object.Instantiate(_selectableTemplate, Content);
            selectable.SetActive(true);
            selectable.name = "Selectable";

            selectable.transform.Find("Name").GetComponent<Text>().text = name;
            selectable.GetComponent<Button>().onClick.AddListener((UnityAction)Selected);

            void Selected()
            {
                OnSelected(deadDropGuid, name);
            }

            _selectables.Add(selectable);
        }
    }
}
