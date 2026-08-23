using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Class for adding weight info to items
[System.Serializable]
public class WeightedEntry<T> : IWeighted
{
    [SerializeField] private T _value;
    public T Value => _value;

    [SerializeField] private float _weight;
    public float Weight => _weight;
}
