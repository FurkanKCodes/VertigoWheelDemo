using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSlotLayout : MonoBehaviour
{
    [SerializeField] private float _radius;
    public float Radius => _radius;

    [SerializeField] private List<WheelSlotView> _slots;
    public IReadOnlyList<WheelSlotView> Slots => _slots;

    private void OnValidate()
    {
        WheelSlotView[] found = GetComponentsInChildren<WheelSlotView>();
        _slots = new List<WheelSlotView>(found);

        ApplySlotPositions();
    }

    private void ApplySlotPositions()
    {
        for(int i = 0; i < _slots.Count; i++)
        {
            float angle = Mathf.Deg2Rad * (90f - i * (360f / _slots.Count));
            float x = _radius * Mathf.Cos(angle);
            float y = _radius * Mathf.Sin(angle);

            RectTransform rt = _slots[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, y);
        }
    }
}
