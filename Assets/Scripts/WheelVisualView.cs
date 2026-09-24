using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WheelVisualView : MonoBehaviour
{
    [SerializeField] private Image _wheelBaseImage;
    public Image WheelBaseImage => _wheelBaseImage;

    [SerializeField] private List<WheelSlotView> _slots;
    public IReadOnlyList<WheelSlotView> Slots => _slots;

    [SerializeField] private Transform _wheelVisualTransform;
    public Transform WheelVisualTransform => _wheelVisualTransform;

    [SerializeField] private Image _premiumShine;
    public Image PremiumShine => _premiumShine;

    [SerializeField] private Button _spinButton;
    public Button SpinButton => _spinButton;

    [SerializeField] private Image _indicatorImage;
    public Image IndicatorImage => _indicatorImage;

    private void OnValidate()
    {
        WheelSlotView[] found = GetComponentsInChildren<WheelSlotView>();
        _slots = new List<WheelSlotView>(found);

        Transform baseImageTransform = transform.Find("ui_wheel_visual/ui_wheel_base_visual/ui_image_wheel_base_value");
        if (baseImageTransform != null)
        {
            _wheelBaseImage = baseImageTransform.GetComponent<Image>();
        }

        _wheelVisualTransform = transform.Find("ui_wheel_visual");

        Transform shineTransform = transform.Find("ui_wheel_background/ui_vfx_premium_shine");
        if (shineTransform != null)
        {
            _premiumShine = shineTransform.GetComponent<Image>();
        }

        Transform spinTransform = transform.Find("ui_spin_button");
        if (spinTransform != null)
        {
            _spinButton = spinTransform.GetComponent<Button>();
        }

        Transform indicatorTransform = transform.Find("ui_indicator");
        if (indicatorTransform != null)
        {
            _indicatorImage = indicatorTransform.GetComponent<Image>();
        }
    }

    public void Populate(WheelTierConfigSO tierConfig, IReadOnlyList<RewardOutcome> outcomes)
    {
        _wheelVisualTransform.rotation = Quaternion.identity;
        _wheelBaseImage.sprite = tierConfig.WheelBaseSprite;

        if (_indicatorImage != null)
        {
            _indicatorImage.sprite = tierConfig.IndicatorSprite;
        }

        for (int i = 0; i < _slots.Count && i < outcomes.Count; i++)
        {
            _slots[i].SetData(outcomes[i]);
        }

        bool isPremium = tierConfig.Group != WheelTierGroups.Bronze;

        if (_premiumShine != null)
        {
            _premiumShine.gameObject.SetActive(isPremium);
            _premiumShine.DOKill();

            if (isPremium)
            {
                Color c = _premiumShine.color;
                c.a = 1f;
                _premiumShine.color = c;
                _premiumShine.DOFade(0.15f, 1.2f).SetLoops(-1, LoopType.Yoyo);
            }
        }
    }

    private void OnDestroy()
    {
        if (_premiumShine != null)
        {
            _premiumShine.DOKill();
        }

        if (_wheelVisualTransform != null)
        {
            _wheelVisualTransform.DOKill();
        }
    }
}