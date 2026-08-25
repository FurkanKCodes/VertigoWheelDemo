using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCardView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    public Image Icon => _icon;

    [SerializeField] private TMP_Text _amountText;
    public TMP_Text AmountText => _amountText;

    [SerializeField] private Button _okButton;
    public Button OkButton => _okButton;

    [SerializeField] private Button _cashOutButton;
    public Button CashOutButton => _cashOutButton;

    [SerializeField] private Button _giveUpButton;
    public Button GiveUpButton => _giveUpButton;

    public event System.Action OnOkClicked;
    public event System.Action OnCashOutClicked;
    public event System.Action OnGiveUpClicked;

    private void OnValidate()
    {
        _icon = transform.Find("CardFrame/RewardIcon").GetComponent<Image>();
        _amountText = transform.Find("CardFrame/AmountText").GetComponent<TMP_Text>();
        _okButton = transform.Find("OkButton").GetComponent<Button>();
        _cashOutButton = transform.Find("CashOutButton").GetComponent<Button>();
        _giveUpButton = transform.Find("GiveUpButton").GetComponent<Button>();
    }

    private void Awake()
    {
        _okButton.onClick.AddListener(() => OnOkClicked?.Invoke());
        _cashOutButton.onClick.AddListener(() => OnCashOutClicked?.Invoke());
        _giveUpButton.onClick.AddListener(() => OnGiveUpClicked?.Invoke());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(RewardOutcome outcome, bool canCashOut)
    {
        gameObject.SetActive(true);
        _icon.sprite = outcome.Icon;

        bool isBomb = outcome.Category == RewardCategoryType.Bomb;

        _amountText.gameObject.SetActive(!isBomb);

        if (!isBomb)
        {
            _amountText.text = "x" + AmountFormatter.Format(outcome.Amount);
        }

        _okButton.gameObject.SetActive(!isBomb);

        _cashOutButton.gameObject.SetActive(!isBomb && canCashOut);

        _giveUpButton.gameObject.SetActive(isBomb);
    }

}
