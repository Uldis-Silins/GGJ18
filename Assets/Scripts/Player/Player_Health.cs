using UnityEngine;
using System.Collections;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;
    public UnityEngine.UI.Slider healthbar;

    private int m_curHealth;

    // Use this for initialization
    void OnEnable()
    {
        m_curHealth = maxHealth;
        healthbar.value = (float)m_curHealth / maxHealth;
    }

    public void SetDamage(int damageAmount)
    {
        m_curHealth -= damageAmount;
        healthbar.value = (float)m_curHealth / maxHealth;
    }
}
