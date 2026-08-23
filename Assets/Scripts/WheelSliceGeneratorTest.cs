using System.Collections.Generic;
using UnityEngine;

public class WheelSliceGeneratorTest : MonoBehaviour
{
    [SerializeField] private WheelTierConfigSO _testConfig;

    private void Start()
    {
        List<RewardOutcome> slices = WheelSliceGenerator.GenerateSlices(_testConfig);

        for (int i = 0; i < slices.Count; i++)
        {
            RewardOutcome outcome = slices[i];
            Debug.Log("Slot " + i + ": " + outcome.Category + " - " + outcome.ItemName + " x" + outcome.Amount);
        }
    }
}