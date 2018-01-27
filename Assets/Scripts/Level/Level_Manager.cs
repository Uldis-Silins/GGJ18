using UnityEngine;
using System.Collections;

public class Level_Manager : MonoBehaviour
{
    public UnityEngine.UI.Text scoreText;

    private int m_score = 0;

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

    public void AddScore(int scoreAmount)
    {
        m_score += scoreAmount;
        scoreText.text = m_score.ToString();
    }
}
