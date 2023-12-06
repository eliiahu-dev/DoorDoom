using OLS_HyperCasual;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PreLevelView : MonoBehaviour
{
    [field: SerializeField] public GameObject Stash { get; private set; }
    [field: SerializeField] public Button DamageButton { get; private set; }
    [field: SerializeField] public Button FireRateButton { get; private set; }
    [field: SerializeField] public Button HealthBarrierButton { get; private set; }
    [field: SerializeField] public Button FightButton { get; private set; }
    
    [field: SerializeField] public TextMeshProUGUI MoneyText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI DamageText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI FireRateText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI HealthBarrierText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI DamageTextAdd { get; private set; }
    [field: SerializeField] public TextMeshProUGUI FireRateTextAdd { get; private set; }
    [field: SerializeField] public TextMeshProUGUI HealthBarrierTextAdd { get; private set; }
    [field: SerializeField] public TextMeshProUGUI Level { get; private set; }
    
    [field: SerializeField] public GameObject[] DamageValue { get; private set; }
    [field: SerializeField] public GameObject[] FireRateValue { get; private set; }
    [field: SerializeField] public GameObject[] HealthBarrierValue { get; private set; }
    
    
    private void Start()
    {
        var entry = BaseEntryPoint.GetInstance();
        entry.SubscribeOnBaseControllersInit(() =>
        {
            entry.GetController<PreLevelController>().AddView(this);
        });

        Level.text = $"LEVEL {PlayerPrefs.GetInt("Level") + 1}";
    }

    public void UpdateMoney(int money)
    {
        MoneyText.text = $"{money}";
        PlayerPrefs.SetInt("Money", money);
    }
}
