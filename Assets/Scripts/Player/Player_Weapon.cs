using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Weapon_Audio))]
public class Player_Weapon : MonoBehaviour
{
    public Transform weaponHolder;
    public Color[] minColors;
    public Color[] maxColors;
    public Color[] reflectColors;
    public Color[] hitColors;
    public float[] colorLerpSpeeds;

    public LineRenderer[] saberLines;

    private Color[] m_currentColors;
    private Color[] m_targetColors;

    private bool m_stateAction;
    private float m_stateActionTime = 0.5f;
    private float m_stateActionTimer;
    private bool m_sabreEnabled;

    private Player_Weapon_Audio m_audio;

    private void Awake()
    {
        m_currentColors = new Color[saberLines.Length];
        m_targetColors = new Color[saberLines.Length];

        m_audio = GetComponent<Player_Weapon_Audio>();
    }

    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < saberLines.Length; i++)
        {
            saberLines[i].startColor = Color.clear;
            saberLines[i].endColor = Color.clear;
        }
    }

    private void OnEnable()
    {
        Level_Manager.Instance.onChangeState += HandleGameStateChange;
    }

    private void OnDisable()
    {
        Level_Manager.Instance.onChangeState -= HandleGameStateChange;
    }

    // Update is called once per frame
    void Update ()
    {
        if (m_sabreEnabled)
        {
            if (m_stateAction)
            {
                StateActionLerp(m_stateActionTimer / m_stateActionTime);
                m_stateActionTimer -= Time.deltaTime;

                if (m_stateActionTimer <= 0)
                {
                    m_stateAction = false;
                }
            }
            else
            {
                DefaultColorLerp();
            }
        }
	}

    private void DefaultColorLerp()
    {
        for (int i = 0; i < saberLines.Length; i++)
        {
            float t = Mathf.PingPong(Time.time * colorLerpSpeeds[i], 1f);
            m_currentColors[i] = Color.Lerp(minColors[i], maxColors[i], t);
            saberLines[i].startColor = m_currentColors[i];
            saberLines[i].endColor = m_currentColors[i];
        }
    }

    public void StateActionLerp(float t)
    {
        for (int i = 0; i < saberLines.Length; i++)
        {
            m_targetColors[i] = Color.Lerp(m_currentColors[i], maxColors[i], 1 - t);
            saberLines[i].startColor = m_targetColors[i];
            saberLines[i].endColor = m_targetColors[i];
        }
    }

    public void OnReflect()
    {
        m_stateAction = true;
        m_stateActionTimer = m_stateActionTime;

        for (int i = 0; i < saberLines.Length; i++)
        {
            m_currentColors[i] = reflectColors[i];
            saberLines[i].startColor = reflectColors[i];
            saberLines[i].endColor = reflectColors[i];
        }

        m_audio.SetState(Player_Weapon_Audio.ClipStates.Hit, true);
    }

    public void OnHit()
    {
        m_stateAction = true;
        m_stateActionTimer = m_stateActionTime;

        for (int i = 0; i < saberLines.Length; i++)
        {
            m_currentColors[i] = hitColors[i];
            saberLines[i].startColor = hitColors[i];
            saberLines[i].endColor = hitColors[i];
        }
    }

    void HandleGameStateChange(Level_Manager.GameState state)
    {
        if(state == Level_Manager.GameState.GameOver || state == Level_Manager.GameState.Win)
        {
            m_sabreEnabled = false;
            m_audio.SetState(Player_Weapon_Audio.ClipStates.Off, true);

            for (int i = 0; i < saberLines.Length; i++)
            {
                saberLines[i].startColor = Color.clear;
                saberLines[i].endColor = Color.clear;
            }
        }

        if(state == Level_Manager.GameState.Play)
        {
            for (int i = 0; i < saberLines.Length; i++)
            {
                saberLines[i].startColor = Color.clear;
                saberLines[i].endColor = Color.clear;
            }

            StartCoroutine(EnableSabre());
        }
    }

    IEnumerator EnableSabre()
    {
        for (int c = 1; c <= 3; c++)
        {
            for (int i = 0; i < saberLines.Length; i++)
            {
                saberLines[i].startColor = maxColors[i];
                saberLines[i].endColor = maxColors[i];
            }

            m_audio.SetState(Player_Weapon_Audio.ClipStates.On, true);
            yield return new WaitForSeconds(0.1f * c);

            //m_audio.SetState(Player_Weapon_Audio.ClipStates.None);

            for (int i = 0; i < saberLines.Length; i++)
            {
                saberLines[i].startColor = Color.clear;
                saberLines[i].endColor = Color.clear;
            }
            yield return new WaitForSeconds(0.5f - (0.1f * c));
        }

        m_audio.SetState(Player_Weapon_Audio.ClipStates.On, true);

        m_sabreEnabled = true;
    }
}
