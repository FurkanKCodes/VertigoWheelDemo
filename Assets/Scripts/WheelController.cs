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

    private void OnValidate()
    {
        WheelSlotView[] found = GetComponentsInChildren<WheelSlotView>();
        _slots = new List<WheelSlotView>(found);

        _wheelBaseImage = transform.Find("ui_wheel_visual/ui_wheel_base_visual/ui_image_wheel_base").GetComponent<Image>();

        _indicatorImage = transform.Find("ui_indicator").GetComponent<Image>();

        _spinButton = transform.Find("ui_spin_button").GetComponent<Button>();

        _wheelVisualTransform = transform.Find("ui_wheel_visual");

        _zoneTitleText = transform.Find("ui_background/ui_text_zone_title").GetComponent<TMP_Text>();

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
        
        _wheelBaseImage.sprite = _currentTierConfig.WheelBaseSprite;
        _indicatorImage.sprite = _currentTierConfig.IndicatorSprite;

        _currentOutcomes = WheelSliceGenerator.GenerateSlices(_currentTierConfig);

        _zoneTitleText.text = zone + ": " + _currentTierConfig.Group + " Spin"; 

        for(int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetData(_currentOutcomes[i]);
        }
    }

    private void Awake()
    {
        _spinButton.onClick.AddListener(Spin);
        _resultCard.OnOkClicked += HandleOkClicked;
        _resultCard.OnCashOutClicked += HandleCashOutOrGiveUp;
        _resultCard.OnGiveUpClicked += HandleCashOutOrGiveUp;
    }

    private void HandleOkClicked()
    {
        _resultCard.Hide();
        GenerateAndDisplay(_currentZone + 1);
    }

    private void HandleCashOutOrGiveUp()
    {
        _resultCard.Hide();
        _rewardSummary.Clear();
        GenerateAndDisplay(1);
    }

    public void Spin()
    {
        int targetIndex = Random.Range(0, _slots.Count);

        float R = targetIndex * (360f / _slots.Count);

        float targetZ = 360f * _spinCount + R;

        _spinButton.interactable = false;

        _wheelVisualTransform.DORotate(new Vector3(0f, 0f, targetZ), _spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                _spinButton.interactable = true;
                RewardOutcome result = _currentOutcomes[targetIndex];
                bool isBomb = result.Category == RewardCategoryType.Bomb;
                if (!isBomb)
                {
                    _rewardSummary.AddReward(result);
                }
                bool canCashOut = _currentTierConfig.Group != WheelTierGroups.Bronze;
                _resultCard.Show(result, canCashOut);
            });
    }

    private void Start()
    {
        GenerateAndDisplay(_testZone);
    }
}
