using UnityEngine;

public class WheelProgressionResolverTest : MonoBehaviour
{
    [SerializeField] private WheelProgressionConfigSO _progression;

    private void Start()
    {
        int[] testZones = { 1, 4, 5, 6, 9, 10, 11, 14, 15, 19, 20, 25, 29, 30, 31, 35, 40, 45, 50, 55, 60, 65, 90 };

        foreach (int zone in testZones)
        {
            WheelTierConfigSO config = WheelProgressionResolver.GetConfigForZone(zone, _progression);
            Debug.Log("Zone " + zone + " -> " + config.name);
        }
    }
}