using UnityEngine.SceneManagement;
using OLS_HyperCasual;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LevelView : MonoBehaviour
{
    [field: SerializeField] public Collider SpawnZone { get; private set; }
    [field: SerializeField] public TextMeshPro Text { get; private set; }
    [field: SerializeField] public Button OpenButton { get; private set; }
    [field: SerializeField] public Button NextButton { get; private set; }
    [field: SerializeField] public Button RestartButton { get; private set; }
    [field: SerializeField] public Slider TimeScaleSlider { get; private set; }
    [field: SerializeField] public GameObject PopUpWin { get; private set; }
    [field: SerializeField] public GameObject PopUpLose { get; private set; }
    [field: SerializeField] public TextMeshProUGUI WinText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI LoseText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI OpenButtonText { get; private set; }
    [field: SerializeField] public Animator DoorAnim { get; private set; }

    public int maxLevel { get; private set; }
    private int maxWave;
    
    private void Start()
    {
        var entry = BaseEntryPoint.GetInstance();
        entry.SubscribeOnBaseControllersInit(() =>
        {
            entry.GetController<LevelController>().AddView(this);
        });
        
        var resourcesController = BaseEntryPoint.Get<ResourcesController>();
        var soLevels = resourcesController.GetResource<LevelControllerSO>(GameResourceConstants.LevelControllerSO, false);
        var level = PlayerPrefs.GetInt("Level");

        maxLevel = soLevels.GetLevels();
        maxWave = soLevels.GetValue(level).Length;
        ChangeWaveText(0);
    }

    public void ChangeWaveText(int value)
    {
        Text.text = $"{value}/{maxWave}";
    }
}