using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardRowView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    public Image Icon => _icon;

    [SerializeField] private TMP_Text _nameText;
    public TMP_Text NameText => _nameText;

    [SerializeField] private TMP_Text _amountText;
    public TMP_Text AmountText => _amountText;

    private int _accumulatedAmount;

    private void OnValidate()
    {
        _icon = transform.Find("ui_image_reward_icon_value").GetComponent<Image>();
        _nameText = transform.Find("ui_text_reward_name_value").GetComponent<TMP_Text>();
        _amountText = transform.Find("ui_text_reward_amount_value").GetComponent<TMP_Text>();
    }

    public void Setup(RewardOutcome outcome)
    {
        _icon.sprite = outcome.Icon;
        _amountText.text = "x" + AmountFormatter.Format(outcome.Amount);
        _nameText.text = outcome.ItemName;
        _accumulatedAmount = outcome.Amount;
    }

    public void AddAmount(int amount)
    {
        _accumulatedAmount += amount;
        _amountText.text = "x" + AmountFormatter.Format(_accumulatedAmount);
    }
}
