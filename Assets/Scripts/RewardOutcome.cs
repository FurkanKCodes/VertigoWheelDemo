using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardCategoryType
{
    Bomb,
    Cash,
    Gold,
    WeaponPoints,
    TierIChest,
    TierIIChest,
    TierIIIChest,
    Consumables,
    CosmeticsTierI,
    CosmeticsTierII,
    TierRenders
}


// Class for items with category type, name, amount, icon 
[System.Serializable]
public class RewardOutcome 
{
    [SerializeField] private RewardCategoryType _category;
    public RewardCategoryType Category => _category;

    [SerializeField] private string _itemName;
    public string ItemName => _itemName;

    [SerializeField] private int _amount;
    public int Amount => _amount;

    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
}
