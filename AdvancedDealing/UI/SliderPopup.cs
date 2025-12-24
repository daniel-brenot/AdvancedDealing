using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if IL2CPP
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI.Phone.Messages;
#elif MONO
using ScheduleOne;
using ScheduleOne.DevUtilities;
using ScheduleOne.UI.Phone.Messages;
#endif

namespace AdvancedDealing.UI
{
    public class SliderPopup
    {
        private string _valuePrefix;

        private string _valueSuffix;

        private int _digits;

        public GameObject Container;

        public Text TitleLabel;

        public Text SubtitleLabel;

        public Text ValueLabel;

        public Button SendButton;

        public Transform Content;

        public Slider Slider;

        private System.Action _onSend;

        private System.Action _onCancel;

        public bool UICreated { get; private set; }

        public bool IsOpen { get; private set; }

        public SliderPopup()
        {
            GameInput.RegisterExitListener((GameInput.ExitDelegate)RightClick, 4);
        }

        private void RightClick(ExitAction action)
        {
            if (!action.Used && IsOpen)
            {
                action.Used = true;
                Close();
            }
        }

        public void Open(string title, string subtitle, float startValue, float minValue, float maxValue, int digits = 2, System.Action onSendCallback = null, System.Action onCancelCallback = null, string valuePrefix = null, string valueSuffix = null)
        {
            IsOpen = true;
            Container.SetActive(true);

            _onSend = onSendCallback;
            _onCancel = onCancelCallback;
            _valuePrefix = valuePrefix;
            _valueSuffix = valueSuffix;
            _digits = digits;

            TitleLabel.text = title;
            SubtitleLabel.text = subtitle;
            ValueLabel.text = $"{_valuePrefix}{System.Math.Round(startValue, _digits)}{_valueSuffix}";
            Slider.value = startValue;
            Slider.minValue = minValue;
            Slider.maxValue = maxValue;
        }

        public void Close()
        {
            IsOpen = false;
            Container.SetActive(false);
        }

        private void OnSend()
        {
            if (!IsOpen) return;
            
            _onSend?.Invoke();
            Close();
        }

        private void OnCancel()
        {
            _onCancel?.Invoke();
            Close();
        }

        private void OnValueChanged(float value)
        {
            ValueLabel.text = $"{_valuePrefix}{System.Math.Round(value, _digits)}{_valueSuffix}";
        }

        public void CreateUI()
        {
            if (UICreated) return;

            GameObject target = PlayerSingleton<MessagesApp>.Instance.ConfirmationPopup.gameObject;

            Container = Object.Instantiate(target, target.transform.parent);
            Container.name = "NegotiationPopup";
            Container.SetActive(true);

            Content = Container.transform.Find("Shade/Content");
            Content.GetComponent<RectTransform>().sizeDelta = new Vector2(-160f, 40f);

            SubtitleLabel = Content.Find("Subtitle").GetComponent<Text>();
            SubtitleLabel.text = "Current: 0.2%\n\nLast offer: 0%";

            ValueLabel = Object.Instantiate(SubtitleLabel.gameObject, SubtitleLabel.transform.parent).GetComponent<Text>();
            ValueLabel.name = "Value";
            ValueLabel.text = "0%";
            ValueLabel.fontSize = 30;
            ValueLabel.fontStyle = FontStyle.Bold;

            RectTransform transform = ValueLabel.GetComponent<RectTransform>();
            transform.anchorMax = new Vector2(1f, 1f);
            transform.anchorMin = new Vector2(0f, 0f);
            transform.offsetMax = new Vector2(-30f, 200f);
            transform.offsetMin = new Vector2(30f, -280f);

            TitleLabel = Content.Find("Title").GetComponent<Text>();
            TitleLabel.text = "Negotiate Cut %";

            Button[] buttons = Content.GetComponentsInChildren<Button>();
            buttons[0].onClick.RemoveAllListeners();
            buttons[0].onClick.AddListener((UnityAction)OnCancel);

            SendButton = buttons[2];
            SendButton.gameObject.name = "Send";
            SendButton.GetComponentInChildren<Text>().text = "Send";
            SendButton.colors = new()
            {
                normalColor = new Color(0.2941f, 0.6863f, 0.8824f, 1f),
                highlightedColor = new Color(0.4532f, 0.7611f, 0.9151f, 1f),
                pressedColor = new Color(0.5674f, 0.8306f, 0.9623f, 1f),
                selectedColor = new Color(0.9608f, 0.9608f, 0.9608f, 1),
                disabledColor = new Color(0.2941f, 0.6863f, 0.8824f, 1),
                colorMultiplier = 1f,
                fadeDuration = 0f,
            };
            SendButton.onClick.RemoveAllListeners();
            SendButton.onClick.AddListener((UnityAction)OnSend);
            SendButton.GetComponent<Image>().color = Color.white;

            GameObject slider = Object.Instantiate(GameObject.Find("UI/ItemUIManager/AmountSelector/Slider"), Content);
            slider.name = "Slider";
            RectTransform transform2 = slider.GetComponent<RectTransform>();
            transform2.anchoredPosition = Vector2.zero;
            transform2.sizeDelta = new Vector2(280f, 40f);

            Slider = slider.GetComponent<Slider>();
            Slider.maxValue = 1f;
            Slider.minValue = 0f;
            Slider.wholeNumbers = false;
            Slider.normalizedValue = 0.5f;
            Slider.onValueChanged.RemoveAllListeners();
            Slider.onValueChanged.AddListener((UnityAction<float>)OnValueChanged);

            RectTransform handle = transform2.Find("Handle Slide Area/Handle").GetComponent<RectTransform>();
            handle.anchoredPosition = new Vector2(0f, 0f);
            handle.sizeDelta = new Vector2(40f, 0f);

            RectTransform fill = transform2.Find("Fill Area/Fill").GetComponent<RectTransform>();
            fill.anchoredPosition = new Vector2(0f, 0f);
            fill.sizeDelta = new Vector2(40f, 0f);

            slider.SetActive(true);

            Object.Destroy(buttons[1].gameObject);

            Utils.Logger.Debug("NegotiationPopup", "Negotiation popup UI created");

            UICreated = true;
        }
    }
}
