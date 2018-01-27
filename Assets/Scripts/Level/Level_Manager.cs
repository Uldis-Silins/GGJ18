using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Level_Manager : MonoBehaviour
{
    public enum GameState { Play, Win, GameOver }

    public GameObject playStateUI;
    public GameObject winStateUI;
    public GameObject gameOverStateUI;

    public Level_EnemySpawner[] enemySpawners;

    public Text scoreText;

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
            Destroy(gameObject);
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
                break;
            case GameState.Win:
                playStateUI.SetActive(false);
                gameOverStateUI.SetActive(false);
                winStateUI.SetActive(true);
                break;
            case GameState.GameOver:
                playStateUI.SetActive(false);
                gameOverStateUI.SetActive(true);
                winStateUI.SetActive(false);
                break;
            default:
                break;
        }
    }
}
