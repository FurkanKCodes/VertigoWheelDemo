using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WheelSliceGenerator 
{
    public static List<RewardOutcome> GenerateSlices(WheelTierConfigSO config)
    {
        // 1. boş liste oluştur
        List<RewardOutcome> result = new List<RewardOutcome>();

        // 2. kalan slot sayısını config.TotalSlotCount ile başlat,
        //bomba varsa listeye ekle ve sayıyı 1 azalt
        int item_count = config.TotalSlotCount;
        if(config.HasBombSlot)
        {
            result.Add(config.BombOutcome);
            item_count = item_count - 1;
        }

        // 3. for döngüsüyle kalan slotları doldur
        for(int i = 0; i < item_count; i++)
        {
            RewardCategoryDefinition temp = WeightedRandomPicker.Pick(config.Categories);
            result.Add(WeightedRandomPicker.Pick(temp.Outcomes).Value);
        }


        // 4. listeyi return et
        return result;
    }
}
