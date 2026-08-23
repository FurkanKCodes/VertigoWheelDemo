using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// class for category types that will be in slots for chark
[System.Serializable]
public class RewardCategoryDefinition : IWeighted
{
    [SerializeField] private RewardCategoryType _categoryType;
    public RewardCategoryType CategoryType => _categoryType;

    [SerializeField] private float _weight;
    public float Weight => _weight;

    [SerializeField] private List<WeightedEntry<RewardOutcome>> _outcomes;
    public IReadOnlyList<WeightedEntry<RewardOutcome>> Outcomes => _outcomes;
}
