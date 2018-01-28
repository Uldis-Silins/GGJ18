using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Level_Manager : MonoBehaviour
{
    public delegate void StateChangeHandler(GameState state);
    public event StateChangeHandler onChangeState;

    public enum GameState { None, Play, Win, GameOver, Menu, Count }

    public GameObject playStateUI;
    public GameObject winStateUI;
    public GameObject gameOverStateUI;
    public GameObject mainMenuUI;

    public GameObject[] hudObjects;

    public Level_EnemySpawner[] enemySpawners;

    public List<Level_EnemySpawner> activeSpawners;

    public Player_HitHint hitHint;  // Projectile counter

    public TextMesh scoreText;
    public Transform healthbar;

    [SerializeField]
    private GameState m_curGameState;

    private int m_score = 0;
    private int m_spawnedEnemies;

    private int m_totalEnemies = 0;

    private int m_addSpawnerWaveCount = 3;

    private static Level_Manager m_instance = null;

    private WaitForSeconds m_waitForMenuDelay;

    public static Level_Manager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<Level_Manager>();
            }

            return m_instance;
        }
    }

    public GameState CurrentGameState { get { return m_curGameState; } }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }

        activeSpawners = new List<Level_EnemySpawner>();
        m_waitForMenuDelay = new WaitForSeconds(5f);
    }

    private void Start()
    {
        ResetLevel();

        if(m_totalEnemies <= 0)
        {
            m_totalEnemies = enemySpawners.Length * m_addSpawnerWaveCount;
        }

        playStateUI.SetActive(false);
        gameOverStateUI.SetActive(false);
        winStateUI.SetActive(false);

        ChangeGameState(GameState.Menu);
    }

    IEnumerator StartGameDelayed()
    {
        yield return new WaitForSeconds(3f);
        ChangeGameState(GameState.Play);

        int spawnerID = activeSpawners.Count;
        activeSpawners.Add(enemySpawners[spawnerID]);
        activeSpawners[spawnerID].enabled = true;
        activeSpawners[spawnerID].SpawnEnemy(true);
    }

    public void ResetLevel()
    {
        activeSpawners.Clear();
        m_score = 0;
        m_spawnedEnemies = 0;
        scoreText.text = m_score.ToString();
    }

    public void AddScore(int scoreAmount)
    {
        m_score += scoreAmount;
        scoreText.text = m_score.ToString();

        if (m_spawnedEnemies >= m_totalEnemies)
        {
            StartCoroutine(WaitLasersForWin());
        }
        else
        {
            if (m_score % m_addSpawnerWaveCount == 0)
            {
                int spawnerID = activeSpawners.Count;
                activeSpawners.Add(enemySpawners[spawnerID]);
                activeSpawners[spawnerID].enabled = true;
                activeSpawners[spawnerID].SpawnEnemy(true);
            }
        }
    }

    IEnumerator WaitLasersForWin()
    {
        while (hitHint.IncomingProjectiles.Count > 0)
        {
            yield return null;
        }

        ChangeGameState(GameState.Win);

        yield return m_waitForMenuDelay;

        ChangeGameState(GameState.Menu);
    }

    public void SetHealth(float healthPercent)
    {
        Vector3 scaleAmount = new Vector3(Mathf.Lerp(0f, 0.5f, healthPercent), healthbar.localScale.y, healthbar.localScale.z);
        healthbar.localScale = scaleAmount;

        if(healthPercent <= 0)
        {
            ChangeGameState(GameState.GameOver);
        }
    }

    public void ChangeGameState(GameState curState)
    {
        m_curGameState = curState;

        switch (curState)
        {
            case GameState.Play:
                playStateUI.SetActive(true);
                gameOverStateUI.SetActive(false);
                winStateUI.SetActive(false);
                ToggleHUD(true);
                mainMenuUI.SetActive(false);
                break;
            case GameState.Win:
                playStateUI.SetActive(false);
                gameOverStateUI.SetActive(false);
                winStateUI.SetActive(true);
                ToggleHUD(false);
                mainMenuUI.SetActive(false);
                break;
            case GameState.GameOver:
                playStateUI.SetActive(false);
                gameOverStateUI.SetActive(true);
                winStateUI.SetActive(false);
                ToggleHUD(false);
                mainMenuUI.SetActive(false);
                break;
            case GameState.Menu:
                playStateUI.SetActive(false);
                gameOverStateUI.SetActive(false);
                winStateUI.SetActive(false);
                ToggleHUD(false);
                mainMenuUI.SetActive(true);
                break;
            default:
                break;
        }

        OnChangeState(m_curGameState);
    }

    public void OnEnemySpawned()
    {
        m_spawnedEnemies++;

        if(m_spawnedEnemies >= m_totalEnemies)
        {
            foreach (var spawner in activeSpawners)
            {
                spawner.enabled = false;
            }
        }
    }

    private void ToggleHUD(bool toggle)
    {
        for (int i = 0; i < hudObjects.Length; i++)
        {
            hudObjects[i].SetActive(toggle);
        }
    }

    void OnChangeState(GameState state)
    {
        if(onChangeState != null)
        {
            onChangeState(state);
        }
    }

    public void CloseMenu()
    {
        ResetLevel();
        mainMenuUI.SetActive(false);
        StartCoroutine(StartGameDelayed());
    }

    public void ExitLevel()
    {
        Application.Quit();
    }
}
