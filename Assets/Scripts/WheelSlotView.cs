using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WheelSlotView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    public Image Icon => _icon;

    [SerializeField] private TMP_Text _amountText;
    public TMP_Text AmountText => _amountText;

    public void SetData(RewardOutcome outcome)
    {
        _icon.sprite = outcome.Icon;
        _amountText.text = AmountFormatter.Format(outcome.Amount) == "0" ? " ": "x" + AmountFormatter.Format(outcome.Amount);
    }
    private void OnValidate()
    {
        if (_icon == null)
        {
            _icon = GetComponentInChildren<Image>();
        }

        if (_amountText == null)
        {
            _amountText = GetComponentInChildren<TMP_Text>();
        }
    }
}   