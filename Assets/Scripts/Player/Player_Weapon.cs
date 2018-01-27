using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Weapon : MonoBehaviour
{
    public Transform weaponHolder;
    public Color[] minColors;
    public Color[] maxColors;
    public Color[] reflectColors;
    public float[] colorLerpSpeeds;

    public LineRenderer[] sabreLines;

    private Color[] m_currentColors;

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
        for (int i = 0; i < sabreLines.Length; i++)
        {
            float t = Mathf.PingPong(Time.time * colorLerpSpeeds[i], 1f);
            m_currentColors[i] = Color.Lerp(minColors[i], maxColors[i], t);
            sabreLines[i].startColor = m_currentColors[i];
            sabreLines[i].endColor = m_currentColors[i];
        }
	}

    public void OnReflect()
    {
        for (int i = 0; i < sabreLines.Length; i++)
        {
            m_currentColors[i] = reflectColors[i];
            sabreLines[i].startColor = m_currentColors[i];
            sabreLines[i].endColor = m_currentColors[i];
        }
    }
}
