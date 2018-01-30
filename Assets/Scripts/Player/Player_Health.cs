using UnityEngine;
using System.Collections;
using UnityEngine.PostProcessing;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;

    public Player_Weapon wapon;

    private int m_curHealth;

    private Transform m_mainCamTransform;
    public PostProcessingProfile mainPostProfile;
    private Vector3 m_defaultCamEuler;
    private float m_shakeScale = 25f;

    private bool m_inShake;
    private VignetteModel.Settings m_shakeVignette;
    private VignetteModel.Settings m_defaultVignetteSettings;
    private float m_randVignetteIntensity;

    private float m_shakeTime = 0.7f;
    private float m_shakeTimer;

    private void Awake()
    {
        m_mainCamTransform = Camera.main.transform;
        m_defaultCamEuler = m_mainCamTransform.eulerAngles;

        m_shakeVignette.intensity = 0.3f;
        m_shakeVignette.smoothness = 1f;
        m_shakeVignette.roundness = 1f;
        m_shakeVignette.center = new Vector2(0.5f, 0f);
        m_shakeVignette.rounded = true;

        m_defaultVignetteSettings = mainPostProfile.vignette.settings;
    }

    void OnEnable()
    {
        m_curHealth = maxHealth;

        if (Level_Manager.Instance != null)
        {
            Level_Manager.Instance.onChangeState += HandleGameStart;
        }
    }

    private void OnDisable()
    {
        if (Level_Manager.Instance != null)
        {
            Level_Manager.Instance.onChangeState -= HandleGameStart;
        }
    }

    private void OnDrawGizmos()
    {
        Color tempColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = tempColor;
    }

    private void Update()
    {
        if (m_inShake)
        {
            if (m_shakeTimer > 0f)
            {
                float t = m_shakeTimer / m_shakeTime;
                Shake();
                m_shakeVignette.intensity = Mathf.Lerp(0f, m_randVignetteIntensity, t);
                mainPostProfile.vignette.settings = m_shakeVignette;
            }
            else
            {
                mainPostProfile.vignette.settings = m_defaultVignetteSettings;
                m_inShake = false;
            }

            m_shakeTimer -= Time.deltaTime;
        }
    }

    public void SetDamage(int damageAmount)
    {
        m_curHealth -= damageAmount;
        wapon.OnHit();
        Level_Manager.Instance.SetHealth((float)m_curHealth / maxHealth);

        if(m_curHealth > 0)
        {
            m_mainCamTransform.rotation = Quaternion.Euler(m_defaultCamEuler + Vector3.Lerp(-Vector3.one * m_shakeScale, Vector3.one * m_shakeScale, Random.value));
            m_randVignetteIntensity = Random.Range(0.15f, 0.3f);
            m_shakeVignette.intensity = m_randVignetteIntensity;
            mainPostProfile.vignette.settings = m_shakeVignette;
            m_shakeTimer = m_shakeTime;
            m_inShake = true;
        }
    }

    void HandleGameStart(Level_Manager.GameState state)
    {
        if (state == Level_Manager.GameState.Play)
        {
            ResetLevel();
        }
    }

    void ResetLevel()
    {
        mainPostProfile.vignette.settings = m_defaultVignetteSettings;
        m_curHealth = maxHealth;
        Level_Manager.Instance.SetHealth((float)m_curHealth / maxHealth);
    }

    public void Shake()
    {
        m_mainCamTransform.rotation = Quaternion.Slerp(m_mainCamTransform.rotation, Quaternion.Euler(m_defaultCamEuler), 15f * Time.deltaTime);
    }
}
