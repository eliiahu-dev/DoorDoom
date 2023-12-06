using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelControllerSO", menuName = "SO/LevelControllerSO", order = 51)]
public class LevelControllerSO : ScriptableObject
{
    [SerializeField] private SettingData[] settings;
    private AmountValue[] amountValues;

    public AmountValue[] GetValue(int levelIndex)
    {
        return settings[levelIndex].value;
    }

    public int GetLevels()
    {
        return settings.Length;
    }

    [Serializable]
    public struct SettingData
    {
        public AmountValue[] value;
    }
    
    [Serializable]
    public struct AmountValue
    {
        public EnemyType enemy;
        public int count;
    }
}
