using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Level_Manager : MonoBehaviour
{
    public enum GameState { Play, Win, GameOver }

    public GameObject playStateUI;
    public GameObject winStateUI;
    public GameObject gameOverStateUI;

    public GameObject[] hudObjects;

    public Level_EnemySpawner[] enemySpawners;

    public TextMesh scoreText;
    public Transform healthbar;

    [SerializeField]
    private GameState m_curGameState;

    private int m_score = 0;
    private int m_curSpawnWave;

    private int m_totalWaves = 3;

    private static Level_Manager m_instance = null;

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

    public int SpawnWavesLeft
    {
        get { return m_totalWaves - m_curSpawnWave; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Debug.LogError("More than one Level_Manager instance");
        }
    }

    private void Start()
    {
        m_score = 0;
        m_curSpawnWave = 0;

        playStateUI.SetActive(false);
        gameOverStateUI.SetActive(false);
        winStateUI.SetActive(false);

        ChangeGameState(GameState.Play);
    }

    public void AddScore(int scoreAmount)
    {
        m_score += scoreAmount;
        scoreText.text = m_score.ToString();
    }

    public void SetHealth(float healthPercent)
    {
        Vector3 scaleAmount = new Vector3(healthPercent, healthbar.localScale.y, healthbar.localScale.z);
        healthbar.localScale = scaleAmount;

        if(healthPercent <= 0)
        {
            ChangeGameState(GameState.GameOver);
        }
    }

    public void AddSpawnWave()
    {
        m_curSpawnWave++;

        if (m_curSpawnWave > m_totalWaves)
        {
            ChangeGameState(GameState.Win);
        }
    }

    public void ChangeGameState(GameState curState)
    {
        switch (curState)
        {
            case GameState.Play:
                playStateUI.SetActive(true);
                gameOverStateUI.SetActive(false);
                winStateUI.SetActive(false);
                ToggleHUD(true);
                break;
            case GameState.Win:
                playStateUI.SetActive(false);
                gameOverStateUI.SetActive(false);
                winStateUI.SetActive(true);
                ToggleHUD(false);
                break;
            case GameState.GameOver:
                playStateUI.SetActive(false);
                gameOverStateUI.SetActive(true);
                winStateUI.SetActive(false);
                ToggleHUD(false);
                break;
            default:
                break;
        }
    }

    private void ToggleHUD(bool toggle)
    {
        for (int i = 0; i < hudObjects.Length; i++)
        {
            hudObjects[i].SetActive(toggle);
        }
    }
}
