using OLS_HyperCasual;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private GameObject _console;

    public void SpawnTeammate()
    {
        var changePlaceController = BaseEntryPoint.Get<ChangePlaceController>();
        changePlaceController.SpawnTeammate();
    }
    
    public void SpawnEnemy()
    {
        var enemyController = BaseEntryPoint.Get<EnemyController>();
        enemyController.StartWaveEnemies();
    }
    
    public void Open()
    {
        var levelController = BaseEntryPoint.Get<LevelController>();
        levelController.OnClickStart();
    }
    
    private void ShowConsole()
    {
        _console.SetActive(!_console.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ShowConsole();
        }
    }
}
