using UnityEngine;
using System.Collections;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;

    public Player_Weapon wapon;

    private int m_curHealth;

    // Use this for initialization
    void OnEnable()
    {
        m_curHealth = maxHealth;

        Level_Manager.Instance.onChangeState += HandleGameStart;
    }

    private void OnDisable()
    {
        Level_Manager.Instance.onChangeState -= HandleGameStart;
    }

    private void OnDrawGizmos()
    {
        Color tempColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = tempColor;
    }

    public void SetDamage(int damageAmount)
    {
        m_curHealth -= damageAmount;
        wapon.OnHit();
        Level_Manager.Instance.SetHealth((float)m_curHealth / maxHealth);
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
        m_curHealth = maxHealth;
        Level_Manager.Instance.SetHealth((float)m_curHealth / maxHealth);
    }
}
