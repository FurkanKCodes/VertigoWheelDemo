using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class RewardSummaryView : MonoBehaviour
{
    [SerializeField] private RewardRowView _rewardRowPrefab;
    public RewardRowView RewardRowPrefab => _rewardRowPrefab;

    [SerializeField] private Transform _content;
    public Transform Content => _content;

    [SerializeField] private Button _closeButton;
    public Button CloseButton => _closeButton;

    [SerializeField] private TMP_Text _headerText;
    public TMP_Text HeaderText => _headerText;

    public event System.Action OnCloseClicked;

    private List<RewardOutcome> _accumulatedRewards = new List<RewardOutcome>();

    private Dictionary<string, RewardRowView> _rowsByItemName = new Dictionary<string, RewardRowView>();

    [SerializeField] private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup => _canvasGroup;

    [SerializeField] private float _openDuration = 0.3f;
    [SerializeField] private float _closeDuration = 0.2f;

    private void OnValidate()
    {
        _content = transform.Find("ui_scrollview_reward_list/Viewport/Content");
        _closeButton = transform.Find("ui_close_button").GetComponent<Button>();
        _headerText = transform.Find("ui_scrollview_reward_list/ui_header_text_value").GetComponent<TMP_Text>();

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Awake()
    {
        _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
    }

    public void Show(string headerText)
    {
        _headerText.text = headerText;
        gameObject.SetActive(true);

        _canvasGroup.DOKill();
        transform.DOKill();

        _canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        _canvasGroup.DOFade(1f, _openDuration);
        transform.DOScale(1f, _openDuration).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        _canvasGroup.DOKill();
        transform.DOKill();

        _canvasGroup.DOFade(0f, _closeDuration)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void AddReward(RewardOutcome outcome)
    {
        _accumulatedRewards.Add(outcome);

        bool isAdd = _rowsByItemName.TryGetValue(outcome.ItemName, out RewardRowView existingRow);

        if(isAdd)
        {
            existingRow.AddAmount(outcome.Amount);
        }

        else
        {
            RewardRowView row = Instantiate(_rewardRowPrefab, _content);
            row.Setup(outcome);
            _rowsByItemName.Add(outcome.ItemName, row);
        }
    }

    public void Clear()
    {
        foreach (RewardRowView row in _rowsByItemName.Values)
        {
            Destroy(row.gameObject);
        }
        _rowsByItemName.Clear();
        _accumulatedRewards.Clear();
    }
}
