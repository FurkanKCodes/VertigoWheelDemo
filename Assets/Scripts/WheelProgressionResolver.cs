using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WheelProgressionResolver 
{
    public static WheelTierConfigSO GetConfigForZone(int zone, WheelProgressionConfigSO progression)
    {
        int tier;
        // 1. Gold?
        if(zone % 30 == 0)
        {
            tier = Mathf.Min((zone + 29) / 30, 2);
            return progression.GoldTiers[tier - 1];
        }

        // 2. Silver?
        else if(zone % 5 == 0)
        {
            tier = Mathf.Min((zone + 9) / 10, 5);
            return progression.SilverTiers[tier - 1];
        }

        // 3. Bronze
        else
        {
            tier = Mathf.Min((zone + 9) / 10, 4);
            return progression.BronzeTiers[tier - 1];
        }
    }
}
