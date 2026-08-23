using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for wheels with their slot counts and slots
[CreateAssetMenu(fileName = "Wheels", menuName = "Data/Wheels")]
public class WheelTierConfigSO : ScriptableObject
{
    [SerializeField] private Sprite _wheelBaseSprite;
    public Sprite WheelBaseSprite => _wheelBaseSprite;

    [SerializeField] private Sprite _indicatorSprite;
    public Sprite IndicatorSprite => _indicatorSprite;

    [SerializeField] private bool _hasBombSlot;
    public bool HasBombSlot => _hasBombSlot;

    [SerializeField] private int _totalSlotCount;
    public int TotalSlotCount => _totalSlotCount;

    [SerializeField] private List<RewardCategoryDefinition> _categories;
    public IReadOnlyList<RewardCategoryDefinition> Categories => _categories;

    [SerializeField] private RewardOutcome _bombOutcome;
    public RewardOutcome BombOutcome => _bombOutcome;
}
