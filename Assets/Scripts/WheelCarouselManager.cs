using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class WheelCarouselManager : MonoBehaviour
{
    [SerializeField] private WheelVisualView _wheelVisualPrefab;
    [SerializeField] private RectTransform _carouselRoot;

    [SerializeField] private RectTransform _anchorPrevious;
    [SerializeField] private RectTransform _anchorCurrent;
    [SerializeField] private RectTransform _anchorNext;
    [SerializeField] private RectTransform _anchorEnter;
    [SerializeField] private RectTransform _anchorExit;

    [SerializeField] private float _slideDuration = 0.5f;

    private WheelVisualView _previousInstance;
    private WheelVisualView _currentInstance;
    private WheelVisualView _nextInstance;

    private UnityAction _spinHandler;

    public WheelVisualView CurrentInstance => _currentInstance;

    public void SetSpinHandler(UnityAction handler)
    {
        _spinHandler = handler;
    }

    private WheelVisualView SpawnAt(RectTransform anchor, WheelTierConfigSO tierConfig, IReadOnlyList<RewardOutcome> outcomes)
    {
        WheelVisualView instance = Instantiate(_wheelVisualPrefab, _carouselRoot);
        RectTransform rt = instance.GetComponent<RectTransform>();
        rt.anchoredPosition = anchor.anchoredPosition;
        rt.localScale = anchor.localScale;
        instance.Populate(tierConfig, outcomes);

        if (instance.SpinButton != null)
        {
            if (_spinHandler != null)
            {
                instance.SpinButton.onClick.AddListener(_spinHandler);
            }
            instance.SpinButton.interactable = false;
        }

        return instance;
    }

    public WheelVisualView Initialize(
        WheelTierConfigSO currentTierConfig, IReadOnlyList<RewardOutcome> currentOutcomes,
        WheelTierConfigSO nextTierConfig, IReadOnlyList<RewardOutcome> nextOutcomes)
    {
        if (_previousInstance != null) { Destroy(_previousInstance.gameObject); }
        if (_currentInstance != null) { Destroy(_currentInstance.gameObject); }
        if (_nextInstance != null) { Destroy(_nextInstance.gameObject); }

        _previousInstance = null;
        _currentInstance = SpawnAt(_anchorCurrent, currentTierConfig, currentOutcomes);
        _nextInstance = SpawnAt(_anchorNext, nextTierConfig, nextOutcomes);

        if (_currentInstance.SpinButton != null)
        {
            _currentInstance.SpinButton.interactable = true;
        }

        return _currentInstance;
    }

    public WheelVisualView ResetToZoneOne(
        WheelTierConfigSO currentTierConfig, IReadOnlyList<RewardOutcome> currentOutcomes,
        WheelTierConfigSO nextTierConfig, IReadOnlyList<RewardOutcome> nextOutcomes)
    {
        return Initialize(currentTierConfig, currentOutcomes, nextTierConfig, nextOutcomes);
    }

    public void AdvanceOnCollect(
        WheelTierConfigSO newNextTierConfig, IReadOnlyList<RewardOutcome> newNextOutcomes,
        System.Action<WheelVisualView> onCurrentSettled)
    {
        WheelVisualView leaving = _previousInstance;
        WheelVisualView movingToPrevious = _currentInstance;
        WheelVisualView movingToCurrent = _nextInstance;
        WheelVisualView incoming = SpawnAt(_anchorEnter, newNextTierConfig, newNextOutcomes);

        if (movingToPrevious.SpinButton != null)
        {
            movingToPrevious.SpinButton.interactable = false;
        }

        if (leaving != null)
        {
            RectTransform leavingRt = leaving.GetComponent<RectTransform>();
            leavingRt.DOAnchorPos(_anchorExit.anchoredPosition, _slideDuration).SetEase(Ease.InOutSine);
            leaving.transform.DOScale(_anchorExit.localScale, _slideDuration).SetEase(Ease.InOutSine)
                .OnComplete(() => Destroy(leaving.gameObject));
        }

        Sequence seq = DOTween.Sequence();

        seq.Append(movingToPrevious.GetComponent<RectTransform>()
            .DOAnchorPos(_anchorPrevious.anchoredPosition, _slideDuration).SetEase(Ease.InOutSine));
        seq.Join(movingToPrevious.transform
            .DOScale(_anchorPrevious.localScale, _slideDuration).SetEase(Ease.InOutSine));

        seq.Join(movingToCurrent.GetComponent<RectTransform>()
            .DOAnchorPos(_anchorCurrent.anchoredPosition, _slideDuration).SetEase(Ease.InOutSine));
        seq.Join(movingToCurrent.transform
            .DOScale(_anchorCurrent.localScale, _slideDuration).SetEase(Ease.InOutSine));

        seq.Join(incoming.GetComponent<RectTransform>()
            .DOAnchorPos(_anchorNext.anchoredPosition, _slideDuration).SetEase(Ease.InOutSine));
        seq.Join(incoming.transform
            .DOScale(_anchorNext.localScale, _slideDuration).SetEase(Ease.InOutSine));

        seq.OnComplete(() =>
        {
            _previousInstance = movingToPrevious;
            _currentInstance = movingToCurrent;
            _nextInstance = incoming;

            if (_currentInstance.SpinButton != null)
            {
                _currentInstance.SpinButton.interactable = true;
            }

            onCurrentSettled?.Invoke(_currentInstance);
        });
    }
}