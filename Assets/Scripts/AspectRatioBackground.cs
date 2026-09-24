using UnityEngine;
using UnityEngine.UI;


public class AspectRatioBackground : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundOption
    {
        [Tooltip("Bu secenege ait arkaplan sprite'i")]
        public Sprite sprite;

        [Tooltip("Genislik/Yukseklik orani. Ornek: 16:9 icin 16f/9f = 1.778, 4:3 icin 4f/3f = 1.333, 20:9 icin 20f/9f = 2.222")]
        public float aspectRatio;
    }

    [Tooltip("Arkaplanin uygulanacagi Image component'i")]
    public Image targetImage;

    [Tooltip("Kullanilabilecek tum arkaplan secenekleri")]
    public BackgroundOption[] backgrounds;

    void Start()
    {
        ApplyBestBackground();
    }

    public void ApplyBestBackground()
    {
        if (targetImage == null || backgrounds == null || backgrounds.Length == 0)
        {
            Debug.LogWarning("AspectRatioBackground: targetImage veya backgrounds atanmamis.");
            return;
        }

        float screenAspect = (float)Screen.width / Screen.height;

        BackgroundOption best = null;
        float bestDiff = float.MaxValue;

        foreach (var option in backgrounds)
        {
            if (option.sprite == null) continue;

            float diff = Mathf.Abs(option.aspectRatio - screenAspect);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                best = option;
            }
        }

        if (best != null)
        {
            targetImage.sprite = best.sprite;
        }
    }
}