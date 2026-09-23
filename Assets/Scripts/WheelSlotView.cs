using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening; // EKLE

public class WheelSlotView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    public Image Icon => _icon;

    [SerializeField] private TMP_Text _amountText;
    public TMP_Text AmountText => _amountText;

    [SerializeField] private Image _winGlow;
    public Image WinGlow => _winGlow;

    public void SetData(RewardOutcome outcome)
    {
        _icon.sprite = outcome.Icon;
        _amountText.text = AmountFormatter.Format(outcome.Amount) == "0" ? " " : "x" + AmountFormatter.Format(outcome.Amount);
    }

    public void PlayWinAnimation(System.Action onComplete)
    {
        transform.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.3f, 0.35f).SetEase(Ease.OutQuad));
        if (_winGlow != null)
        {
            seq.Join(_winGlow.DOFade(1f, 0.2f));
        }
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOScale(1f, 0.35f).SetEase(Ease.InQuad));
        if (_winGlow != null)
        {
            seq.Join(_winGlow.DOFade(0f, 0.3f));
        }
        seq.OnComplete(() => onComplete?.Invoke());
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

        if (_winGlow == null)
        {
            Transform glowTransform = transform.Find("ui_vfx_star_glow");
            if (glowTransform != null)
            {
                _winGlow = glowTransform.GetComponent<Image>();
            }
        }
    }
}