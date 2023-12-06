using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StatsControllerSO", menuName = "SO/StatsControllerSO", order = 51)]
public class StatsControllerSO : ScriptableObject
{
    [SerializeField] private AmountValue[] amountValues;

    public AmountValue GetValue(StatsType type)
    {
        AmountValue amountValue = new();
        
        foreach (var value in amountValues)
        {
            if (value.statType == type)
            {
                amountValue =  value;
            }
        }

        return amountValue;
    }
    
    [Serializable]
    public struct SettingData
    {
        public AmountValue[] value;
    }
    
    [Serializable]
    public struct AmountValue
    {
        public StatsType statType;
        public UpgradeValue[] upgradeValue;
    }
    
    [Serializable]
    public struct UpgradeValue
    {
        public int Price;
        public float Upgrate;
    }
}
