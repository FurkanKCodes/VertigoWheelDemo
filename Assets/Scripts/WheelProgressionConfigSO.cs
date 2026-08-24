using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wheel Tiers", menuName = "Data/Wheels Tiers")]
public class WheelProgressionConfigSO : ScriptableObject
{
    [SerializeField] private List<WheelTierConfigSO> _bronzeTiers;
    public IReadOnlyList<WheelTierConfigSO> BronzeTiers => _bronzeTiers;

    [SerializeField] private List<WheelTierConfigSO> _silverTiers;
    public IReadOnlyList<WheelTierConfigSO> SilverTiers => _silverTiers;

    [SerializeField] private List<WheelTierConfigSO> _goldTiers;
    public IReadOnlyList<WheelTierConfigSO> GoldTiers => _goldTiers;
}
