using UnityEngine;
using System.Collections;
using UnityEngine.PostProcessing;

public class Level_EnemyFireFX : MonoBehaviour
{
    public PostProcessingProfile mainProfile;
    public float bloomIntensity = 2f;

    public float resetTime = 0.5f;

    private BloomModel.Settings m_defaultBloomSettings;

    private float m_resetTimer;

    // Use this for initialization
    void Start()
    {
        m_defaultBloomSettings = mainProfile.bloom.settings;
    }

    private void Update()
    {
        if(m_resetTimer < 0)
        {
            mainProfile.bloom.settings = m_defaultBloomSettings;
        }

        m_resetTimer -= Time.deltaTime;
    }

    public void PlayFX()
    {
        BloomModel.Settings bloomSettings = mainProfile.bloom.settings;
        bloomSettings.bloom.intensity = bloomIntensity;
        mainProfile.bloom.settings = bloomSettings;
        m_resetTimer = resetTime;
    }
}
