using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WheelController : MonoBehaviour
{
    [SerializeField] private WheelProgressionConfigSO _progressionConfig;
    public WheelProgressionConfigSO ProgressionConfig => _progressionConfig;

    [SerializeField] private List<WheelSlotView> _slots;
    public IReadOnlyList<WheelSlotView> Slots => _slots;

    [SerializeField] private Image _wheelBaseImage;
    public Image WheelBaseImage => _wheelBaseImage;

    [SerializeField] private Image _indicatorImage;
    public Image IndicatorImage => _indicatorImage;

    [SerializeField] private int _testZone;
    public int TestZone => _testZone;

    [SerializeField] private Button _spinButton;
    public Button SpinButton => _spinButton;

    [SerializeField] private Transform _wheelVisualTransform;
    public Transform WheelVisualTransform => _wheelVisualTransform;

    [SerializeField] private float _spinDuration = 4f;
    public float SpinDuration => _spinDuration;

    [SerializeField] private int _spinCount = 5;
    public int SpinCount => _spinCount;

    [SerializeField] private TMP_Text _zoneTitleText;
    public TMP_Text ZoneTitleText => _zoneTitleText;

    private WheelTierConfigSO _currentTierConfig;

    private List<RewardOutcome> _currentOutcomes;

    private int _currentZone;

    [SerializeField] private ResultCardView _resultCard;
    public ResultCardView ResultCard => _resultCard;

    [SerializeField] private RewardSummaryView _rewardSummary;
    public RewardSummaryView RewardSummary => _rewardSummary;

    [SerializeField] private Button _myRewardsButton;
    public Button MyRewardsButton => _myRewardsButton;

    [SerializeField] private Button _leaveButton;
    public Button LeaveButton => _leaveButton;

    private bool _resetOnRewardSummaryClose;

    [SerializeField] private Image _premiumShine;
    public Image PremiumShine => _premiumShine;

    private void OnValidate()
    {
        WheelSlotView[] found = GetComponentsInChildren<WheelSlotView>();
        _slots = new List<WheelSlotView>(found);

        _wheelBaseImage = transform.Find("ui_wheel_visual/ui_wheel_base_visual/ui_image_wheel_base_value").GetComponent<Image>();

        _indicatorImage = transform.Find("ui_indicator").GetComponent<Image>();

        _spinButton = transform.Find("ui_spin_button").GetComponent<Button>();

        _myRewardsButton = transform.Find("ui_my_rewards_button").GetComponent<Button>();

        _leaveButton = transform.Find("ui_leave_button").GetComponent<Button>();

        _wheelVisualTransform = transform.Find("ui_wheel_visual");

        _zoneTitleText = transform.Find("ui_wheel_background/ui_text_zone_title_value").GetComponent<TMP_Text>();

        _premiumShine = transform.Find("ui_wheel_background/ui_vfx_premium_shine").GetComponent<Image>();

        Transform resultCardTransform = transform.parent.Find("ui_result_card");
        Debug.Log("Found Transform: " + resultCardTransform);
        _resultCard = resultCardTransform.GetComponent<ResultCardView>();
        Debug.Log("Found Component: " + _resultCard);

        Transform rewardSummaryTransform = transform.parent.Find("ui_panel_reward_summary");
        Debug.Log("Found Transform: " + rewardSummaryTransform);
        _rewardSummary = rewardSummaryTransform.GetComponent<RewardSummaryView>();
        Debug.Log("Found Component: " + _rewardSummary);
    }

    public void GenerateAndDisplay(int zone)
    {

        _wheelVisualTransform.rotation = Quaternion.identity;

        _currentZone = zone;
        
        _currentTierConfig = WheelProgressionResolver.GetConfigForZone(zone, _progressionConfig);

        _leaveButton.interactable = _currentTierConfig.Group != WheelTierGroups.Bronze;
        _spinButton.interactable = true;
        
        _wheelBaseImage.sprite = _currentTierConfig.WheelBaseSprite;
        _indicatorImage.sprite = _currentTierConfig.IndicatorSprite;

        _currentOutcomes = WheelSliceGenerator.GenerateSlices(_currentTierConfig);

        _zoneTitleText.text = zone + ": " + _currentTierConfig.Group + " Spin"; 

        for(int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetData(_currentOutcomes[i]);
        }

        bool isPremium = _currentTierConfig.Group != WheelTierGroups.Bronze;
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

    private void Awake()
    {
        _spinButton.onClick.AddListener(Spin);
        _resultCard.OnOkClicked += HandleOkClicked;
        _resultCard.OnGiveUpClicked += HandleGiveUpClicked;
        _myRewardsButton.onClick.AddListener(HandleMyRewardsClicked);
        _leaveButton.onClick.AddListener(HandleLeaveClicked);
        _rewardSummary.OnCloseClicked += HandleRewardSummaryCloseClicked;
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
            GenerateAndDisplay(1);
        }
    }

    private void HandleOkClicked()
    {
        _resultCard.Hide();
        GenerateAndDisplay(_currentZone + 1);
    }

    private void HandleGiveUpClicked()
    {
        _resultCard.Hide();
        _resetOnRewardSummaryClose = true;
        _rewardSummary.Show("Lost Rewards");
    }

    public void Spin()
    {
        int targetIndex = Random.Range(0, _slots.Count);

        float R = targetIndex * (360f / _slots.Count);

        float targetZ = 360f * _spinCount + R;

        _spinButton.interactable = false;

        _leaveButton.interactable = false;

        _wheelVisualTransform.DORotate(new Vector3(0f, 0f, targetZ), _spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                RewardOutcome result = _currentOutcomes[targetIndex];
                bool isBomb = result.Category == RewardCategoryType.Bomb;
                if (!isBomb)
                {
                    _rewardSummary.AddReward(result);
                }

                _slots[targetIndex].PlayWinAnimation(() =>
                {
                    _resultCard.Show(result);
                });
            });
    }

    private void Start()
    {
        GenerateAndDisplay(_testZone);
    }
}
