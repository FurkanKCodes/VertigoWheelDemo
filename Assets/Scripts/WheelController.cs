using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WheelController : MonoBehaviour
{
    [SerializeField] private WheelProgressionConfigSO _progressionConfig;
    public WheelProgressionConfigSO ProgressionConfig => _progressionConfig;

    [SerializeField] private WheelCarouselManager _carouselManager;
    public WheelCarouselManager CarouselManager => _carouselManager;

    [SerializeField] private int _testZone = 1;
    public int TestZone => _testZone;

    [SerializeField] private float _spinDuration = 4f;
    public float SpinDuration => _spinDuration;

    [SerializeField] private int _spinCount = 5;
    public int SpinCount => _spinCount;

    [SerializeField] private TMP_Text _zoneTitleText;
    public TMP_Text ZoneTitleText => _zoneTitleText;

    [SerializeField] private ResultCardView _resultCard;
    public ResultCardView ResultCard => _resultCard;

    [SerializeField] private RewardSummaryView _rewardSummary;
    public RewardSummaryView RewardSummary => _rewardSummary;

    [SerializeField] private Button _myRewardsButton;
    public Button MyRewardsButton => _myRewardsButton;

    [SerializeField] private Button _leaveButton;
    public Button LeaveButton => _leaveButton;

    private WheelVisualView _activeVisual;
    private WheelTierConfigSO _currentTierConfig;
    private List<RewardOutcome> _currentOutcomes;
    private int _currentZone;

    private WheelTierConfigSO _nextTierConfig;
    private List<RewardOutcome> _nextOutcomes;

    private bool _resetOnRewardSummaryClose;

    private void OnValidate()
    {
        Transform myRewardsT = transform.Find("ui_my_rewards_button");
        if (myRewardsT != null) _myRewardsButton = myRewardsT.GetComponent<Button>();

        Transform leaveT = transform.Find("ui_leave_button");
        if (leaveT != null) _leaveButton = leaveT.GetComponent<Button>();

        Transform titleT = transform.Find("ui_text_zone_title_value");
        if (titleT != null) _zoneTitleText = titleT.GetComponent<TMP_Text>();

        if (transform.parent != null)
        {
            Transform resultCardTransform = transform.parent.Find("ui_result_card");
            if (resultCardTransform != null)
            {
                _resultCard = resultCardTransform.GetComponent<ResultCardView>();
            }

            Transform rewardSummaryTransform = transform.parent.Find("ui_panel_reward_summary");
            if (rewardSummaryTransform != null)
            {
                _rewardSummary = rewardSummaryTransform.GetComponent<RewardSummaryView>();
            }
        }
    }

    private (WheelTierConfigSO tier, List<RewardOutcome> outcomes) BuildZoneData(int zone)
    {
        WheelTierConfigSO tier = WheelProgressionResolver.GetConfigForZone(zone, _progressionConfig);
        List<RewardOutcome> outcomes = WheelSliceGenerator.GenerateSlices(tier);
        return (tier, outcomes);
    }

    private void BindActive(int zone, WheelVisualView visual, WheelTierConfigSO tier, List<RewardOutcome> outcomes)
    {
        _currentZone = zone;
        _activeVisual = visual;
        _currentTierConfig = tier;
        _currentOutcomes = outcomes;

        _zoneTitleText.text = zone + ": " + tier.Group + " Spin";
        _leaveButton.interactable = tier.Group != WheelTierGroups.Bronze;

    }

    private void Awake()
    {
        _carouselManager.SetSpinHandler(Spin);

        _resultCard.OnOkClicked += HandleOkClicked;
        _resultCard.OnGiveUpClicked += HandleGiveUpClicked;
        _myRewardsButton.onClick.AddListener(HandleMyRewardsClicked);
        _leaveButton.onClick.AddListener(HandleLeaveClicked);
        _rewardSummary.OnCloseClicked += HandleRewardSummaryCloseClicked;
    }

    private void Start()
    {
        var current = BuildZoneData(_testZone);
        var next = BuildZoneData(_testZone + 1);

        WheelVisualView visual = _carouselManager.Initialize(current.tier, current.outcomes, next.tier, next.outcomes);

        BindActive(_testZone, visual, current.tier, current.outcomes);
        _nextTierConfig = next.tier;
        _nextOutcomes = next.outcomes;
    }

    private void HandleMyRewardsClicked()
    {
        _resetOnRewardSummaryClose = false;
        _rewardSummary.Show("My Rewards");
    }

    private void HandleLeaveClicked()
    {
        _resetOnRewardSummaryClose = true;
        _rewardSummary.Show("My Rewards");
    }

    private void HandleRewardSummaryCloseClicked()
    {
        _rewardSummary.Hide();

        if (_resetOnRewardSummaryClose)
        {
            _rewardSummary.Clear();
            ResetToZoneOne();
        }
    }

    private void ResetToZoneOne()
    {
        var current = BuildZoneData(1);
        var next = BuildZoneData(2);

        WheelVisualView visual = _carouselManager.ResetToZoneOne(current.tier, current.outcomes, next.tier, next.outcomes);

        BindActive(1, visual, current.tier, current.outcomes);
        _nextTierConfig = next.tier;
        _nextOutcomes = next.outcomes;
    }

    private void HandleOkClicked()
    {
        _resultCard.Hide();

        WheelTierConfigSO becomingCurrentTier = _nextTierConfig;
        List<RewardOutcome> becomingCurrentOutcomes = _nextOutcomes;
        int becomingCurrentZone = _currentZone + 1;

        var newNext = BuildZoneData(_currentZone + 2);

        _carouselManager.AdvanceOnCollect(newNext.tier, newNext.outcomes, (settledVisual) =>
        {
            BindActive(becomingCurrentZone, settledVisual, becomingCurrentTier, becomingCurrentOutcomes);
            _nextTierConfig = newNext.tier;
            _nextOutcomes = newNext.outcomes;
        });
    }

    private void HandleGiveUpClicked()
    {
        _resultCard.Hide();
        _resetOnRewardSummaryClose = true;
        _rewardSummary.Show("Lost Rewards");
    }

    public void Spin()
    {
        IReadOnlyList<WheelSlotView> slots = _activeVisual.Slots;
        int targetIndex = Random.Range(0, slots.Count);

        float R = targetIndex * (360f / slots.Count);
        float targetZ = 360f * _spinCount + R;

        _activeVisual.SpinButton.interactable = false;
        _leaveButton.interactable = false;

        _activeVisual.WheelVisualTransform.DORotate(new Vector3(0f, 0f, targetZ), _spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                RewardOutcome result = _currentOutcomes[targetIndex];
                bool isBomb = result.Category == RewardCategoryType.Bomb;
                if (!isBomb)
                {
                    _rewardSummary.AddReward(result);
                }

                _activeVisual.Slots[targetIndex].PlayWinAnimation(() =>
                {
                    _resultCard.Show(result);
                });
            });
    }
}