using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Weapon : MonoBehaviour
{
    public Transform weaponHolder;
    public Color[] minColors;
    public Color[] maxColors;
    public Color[] reflectColors;
    public Color[] hitColors;
    public float[] colorLerpSpeeds;

    public LineRenderer[] sabreLines;

    private Color[] m_currentColors;

    private bool m_stateAction;
    private float m_stateActionTime = 0.5f;
    private float m_stateActionTimer;

    private void Awake()
    {
        m_currentColors = new Color[4];
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
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

    private void DefaultColorLerp()
    {
        for (int i = 0; i < sabreLines.Length; i++)
        {
            float t = Mathf.PingPong(Time.time * colorLerpSpeeds[i], 1f);
            m_currentColors[i] = Color.Lerp(minColors[i], maxColors[i], t);
            sabreLines[i].startColor = m_currentColors[i];
            sabreLines[i].endColor = m_currentColors[i];
        }
    }

    public void StateActionLerp(float t)
    {
        for (int i = 0; i < sabreLines.Length; i++)
        {
            Color targetColor = Color.Lerp(m_currentColors[i], maxColors[i], 1 - t);
            sabreLines[i].startColor = targetColor;
            sabreLines[i].endColor = targetColor;
        }
    }

    public void OnReflect()
    {
        m_stateAction = true;
        m_stateActionTimer = m_stateActionTime;

        for (int i = 0; i < sabreLines.Length; i++)
        {
            m_currentColors[i] = reflectColors[i];
            sabreLines[i].startColor = reflectColors[i];
            sabreLines[i].endColor = reflectColors[i];
        }
    }

    public void OnHit()
    {
        m_stateAction = true;
        m_stateActionTimer = m_stateActionTime;

        for (int i = 0; i < sabreLines.Length; i++)
        {
            m_currentColors[i] = hitColors[i];
            sabreLines[i].startColor = hitColors[i];
            sabreLines[i].endColor = hitColors[i];
        }
    }
}
