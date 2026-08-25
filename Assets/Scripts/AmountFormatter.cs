using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AmountFormatter 
{
    public static string Format(int amount)
    {
        if(amount < 1000)
        {
            return amount.ToString();
        }
        else
        {
            float temp = amount / 1000f;
            return temp.ToString("0.#")+"k";
        }
    }
}
