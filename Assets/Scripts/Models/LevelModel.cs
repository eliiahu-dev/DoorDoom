using System;
using OLS_HyperCasual;
using UnityEngine.UI;
using UnityEngine;

public class LevelModel : BaseModel<LevelView>
{
    public Bounds BoundsSpawn { get; private set; }
    public GameObject OpenButton { get; private set; }
    public GameObject OpenButtonText { get; private set; }
    public Slider TimeSlider { get; private set; }
    public Animator Door { get; private set; }
    
    public LevelModel(LevelView view, Action click)
    {
        View = view;
        Door = view.DoorAnim;
        BoundsSpawn = view.SpawnZone.bounds;
        OpenButton = view.OpenButton.gameObject;
        OpenButtonText = view.OpenButtonText.gameObject;
        TimeSlider = view.TimeScaleSlider;
        
        view.OpenButton.onClick.AddListener(click.Invoke);
        view.NextButton.onClick.AddListener(NextLevel);
        view.RestartButton.onClick.AddListener(RestartLevel);
    }

    public void HideButton()
    {
        View.OpenButton.gameObject.SetActive(false);
    }

    public void ShowPopUpWin(int money)
    {
        View.PopUpWin.gameObject.SetActive(true);
        View.WinText.text = $"+{money}";
    }

    public void ShowPopUpLose(int money)
    {
        View.PopUpLose.gameObject.SetActive(true);
        View.LoseText.text = $"+{money}";
    }

    private void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    private void NextLevel()
    {
        var level = PlayerPrefs.GetInt("Level");

        if (View.maxLevel - 1 >= level + 1)
        {
            PlayerPrefs.SetInt("Level", level + 1);
        }
        
        Application.LoadLevel(Application.loadedLevel);
    }
}