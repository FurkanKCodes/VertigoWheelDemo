using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Operations for selecting the reward type and object of reward type to put in the chark
public static class WeightedRandomPicker 
{
    public static T Pick<T>(IReadOnlyList<T> items) where T : IWeighted
    {
        float sum = 0f;
        foreach (var item in items)
        {
            sum += item.Weight;
        }

        float r = Random.Range(0f, sum);

        float cumulative = 0f;
        foreach (var item in items)
        {
            cumulative += item.Weight;
            if (r < cumulative)
            {
                return item;
            }
        }

        return items[items.Count - 1];
        
    }
}
