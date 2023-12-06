using OLS_HyperCasual;
using UnityEngine;

public class LevelController : BaseMonoController<LevelView, LevelModel>
{
    private ChangePlaceController changePlaceController;
    private EnemyController enemyController;
    private int waveIndex;

    public LevelController()
    {
        changePlaceController = BaseEntryPoint.Get<ChangePlaceController>();
        enemyController = BaseEntryPoint.Get<EnemyController>();
    }
    
    public override LevelModel AddView(LevelView view)
    {
        var model = new LevelModel(view, OnClickStart);
        
        enemyController.MaxBoundsPos = model.BoundsSpawn.max;
        enemyController.MinBoundsPos = model.BoundsSpawn.min;

        modelsList.Add(model);
        return model;
    }

    public LevelModel GetModel()
    {
        return modelsList[0];
    }
    
    public void OnClickStart()
    {
        waveIndex++;
        enemyController.StartGame = true;
        changePlaceController.SpawnTeammate();
        enemyController.StartWaveEnemies();
        
        foreach (var model in modelsList)
        {
            model.View.ChangeWaveText(waveIndex);
        }
    }

    public void HideOpenButton()
    {
        foreach (var model in modelsList)
        {
            model.HideButton();
        }
    }
    
    public void ShowWinPopUp()
    {
        foreach (var model in modelsList)
        {
            Time.timeScale = 1f;
            var teammateController = BaseEntryPoint.Get<TeammateController>();
            var money = teammateController.Money;
            model.ShowPopUpWin(money);
        }
    }
    
    public void ShowLosePopUp()
    {
        foreach (var model in modelsList)
        {
            Time.timeScale = 1f;
            var teammateController = BaseEntryPoint.Get<TeammateController>();
            var money = teammateController.Money;
            model.ShowPopUpLose(money);
        }
    }

    public void OnOpenDoors(bool state)
    {
        GetModel().Door.SetBool("isOpen", state);
        GetModel().Door.SetBool("isClose", !state);
    }
}