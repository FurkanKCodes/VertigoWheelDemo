using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ResultCardView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    public Image Icon => _icon;

    [SerializeField] private TMP_Text _amountText;
    public TMP_Text AmountText => _amountText;

    [SerializeField] private Button _okButton;
    public Button OkButton => _okButton;

    [SerializeField] private Button _giveUpButton;
    public Button GiveUpButton => _giveUpButton;

    [SerializeField] private TMP_Text _nameText;
    public TMP_Text NameText => _nameText;

    [SerializeField] private TMP_Text _congratsText;
    public TMP_Text CongratsText => _congratsText;

    public event System.Action OnOkClicked;
    public event System.Action OnGiveUpClicked;

    [SerializeField] private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup => _canvasGroup;

    [SerializeField] private Image _starFlash;
    public Image StarFlash => _starFlash;

    [SerializeField] private float _openDuration = 0.3f;
    [SerializeField] private float _closeDuration = 0.2f;

    private void OnValidate()
    {
        _icon = transform.Find("ui_card_frame/ui_reward_icon_value").GetComponent<Image>();
        _amountText = transform.Find("ui_card_frame/ui_amount_text_value").GetComponent<TMP_Text>();
        _nameText = transform.Find("ui_card_frame/ui_text_reward_name_value").GetComponent<TMP_Text>();
        _congratsText = transform.Find("ui_card_frame/ui_congrats_text_value").GetComponent<TMP_Text>();
        _okButton = transform.Find("ui_ok_button").GetComponent<Button>();
        _giveUpButton = transform.Find("ui_give_up_button").GetComponent<Button>();

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        _starFlash = transform.Find("ui_card_frame/ui_vfx_star_flash").GetComponent<Image>();
    }

    private void Awake()
    {
        _okButton.onClick.AddListener(() => OnOkClicked?.Invoke());
        _giveUpButton.onClick.AddListener(() => OnGiveUpClicked?.Invoke());
    }

    public void Hide()
    {
        _canvasGroup.DOKill();
        transform.DOKill();

        _canvasGroup.DOFade(0f, _closeDuration)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void Show(RewardOutcome outcome)
    {
        gameObject.SetActive(true);
        _icon.sprite = outcome.Icon;

        bool isBomb = outcome.Category == RewardCategoryType.Bomb;

        _amountText.gameObject.SetActive(!isBomb);

        if (!isBomb)
        {
            _amountText.text = "x" + AmountFormatter.Format(outcome.Amount);
            _nameText.text = outcome.ItemName;
            _congratsText.text = "CONGRATS";
        }
        else
        {
            _nameText.text = "BOMB";
            _congratsText.text = "BOMB EXPLODED";

        }

        _okButton.gameObject.SetActive(!isBomb);
        _giveUpButton.gameObject.SetActive(isBomb);

        _canvasGroup.DOKill();
        transform.DOKill();

        _canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        _canvasGroup.DOFade(1f, _openDuration);
        transform.DOScale(1f, _openDuration).SetEase(Ease.OutBack);

        if (_starFlash != null)
        {
            _starFlash.gameObject.SetActive(!isBomb);
            if (!isBomb)
            {
                _starFlash.transform.localScale = Vector3.zero;
                _starFlash.transform.DOScale(1f, _openDuration).SetEase(Ease.OutBack);
            }
        }
    }

}
