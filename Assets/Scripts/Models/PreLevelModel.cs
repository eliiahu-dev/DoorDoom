using OLS_HyperCasual;
using UnityEngine;

public class PreLevelModel : BaseModel<PreLevelView>
{
    private StatsControllerSO soStats;
    private int money;
    private int indexDamage;
    private int indexFireRate;
    private int indexHealthBarrier;
    
    public PreLevelModel(PreLevelView view)
    {
        var resourcesController = BaseEntryPoint.Get<ResourcesController>();
        soStats = resourcesController.GetResource<StatsControllerSO>(GameResourceConstants.StatsControllerSO, false);
        
        View = view;

        money = PlayerPrefs.GetInt("Money");
        view.UpdateMoney(money);
        
        indexDamage = PlayerPrefs.GetInt("Damage");
        indexFireRate = PlayerPrefs.GetInt("FireRate");
        indexHealthBarrier = PlayerPrefs.GetInt("HealthBarrier");

        UpdateDamage(0);
        UpdateFireRate(0);
        UpdateHealthBarrier(0);

        view.DamageButton.onClick.AddListener(TryUpdateDamage);
        view.FireRateButton.onClick.AddListener(TryUpdateFireRate);
        view.HealthBarrierButton.onClick.AddListener(TryUpdateHealthBarrier);
        view.FightButton.onClick.AddListener(OnClickFight);
    }

    private void OnClickFight()
    {
        View.Stash.SetActive(true);
        View.gameObject.SetActive(false);
    }
    
    private void TryUpdateDamage()
    {
        var value = soStats.GetValue(StatsType.Damage);
        var price = value.upgradeValue[indexDamage].Price;

        if (money < price)
        {
            return;
        }

        UpdateDamage(1);
        money -= price;
        View.UpdateMoney(money);
    }
    
    private void TryUpdateFireRate()
    {
        var value = soStats.GetValue(StatsType.FireRate);
        var price = value.upgradeValue[indexFireRate].Price;

        if (money < price)
        {
            return;
        }
        
        UpdateFireRate(1);
        money -= price;
        View.UpdateMoney(money);
    }
    
    private void TryUpdateHealthBarrier()
    {
        var value = soStats.GetValue(StatsType.HealthBarrier);
        var price = value.upgradeValue[indexHealthBarrier].Price;

        if (money < price)
        {
            return;
        }
        
        UpdateHealthBarrier(1);
        money -= price;
        View.UpdateMoney(money);
    }
    
    
    private void UpdateDamage(int addValue)
    {
        indexDamage += addValue;
        
        var value = soStats.GetValue(StatsType.Damage);
        
        if (value.upgradeValue.Length > indexDamage)
        {
            var price = value.upgradeValue[indexDamage].Price;
            View.DamageText.text = $"{price}";
        }
        else
        {
            View.DamageButton.gameObject.SetActive(false);
        }
        
        PlayerPrefs.SetInt("Damage", indexDamage);

        for (var i = 0; i < indexDamage; i++)
        {
            View.DamageValue[i].SetActive(true);
        }
        
        if (indexDamage == 0)
        {
            View.DamageTextAdd.text = "+0%";
            return;
        }
        
        var valueDamage = value.upgradeValue[indexDamage - 1].Upgrate;
        View.DamageTextAdd.text = $"+{valueDamage}%";
    }
    
    private void UpdateFireRate(int addValue)
    {
        indexFireRate += addValue;
        
        var value = soStats.GetValue(StatsType.FireRate);
        
        if (value.upgradeValue.Length > indexFireRate)
        {
            var price = value.upgradeValue[indexFireRate].Price;
            View.FireRateText.text = $"{price}";
        }
        else
        {
            View.FireRateButton.gameObject.SetActive(false);
        }
        
        PlayerPrefs.SetInt("FireRate", indexFireRate);
        
        for (var i = 0; i < indexFireRate; i++)
        {
            View.FireRateValue[i].SetActive(true);
        }
        
        if (indexFireRate == 0)
        {
            View.FireRateTextAdd.text = "+0";
            return;
        }
        
        var valueFireRate = value.upgradeValue[indexFireRate - 1].Upgrate;
        View.FireRateTextAdd.text = $"+{valueFireRate}";
    }
    
    private void UpdateHealthBarrier(int addValue)
    {
        indexHealthBarrier += addValue;
        
        var value = soStats.GetValue(StatsType.HealthBarrier);

        if (value.upgradeValue.Length > indexHealthBarrier)
        {
            var price = value.upgradeValue[indexHealthBarrier].Price;
            View.HealthBarrierText.text = $"{price}";
        }
        else
        {
            View.HealthBarrierButton.gameObject.SetActive(false);
        }
        
        PlayerPrefs.SetInt("HealthBarrier", indexHealthBarrier);
        
        for (var i = 0; i < indexHealthBarrier; i++)
        {
            View.HealthBarrierValue[i].SetActive(true);
        }

        if (indexHealthBarrier == 0)
        {
            View.HealthBarrierTextAdd.text = "+0";
            return;
        }

        var valueHealthBarrier = value.upgradeValue[indexHealthBarrier - 1].Upgrate;
        View.HealthBarrierTextAdd.text = $"+{valueHealthBarrier}";
    }
}
