﻿using UnityEngine;
using System.Collections;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;

    private int m_curHealth;

    // Use this for initialization
    void OnEnable()
    {
        m_curHealth = maxHealth;
        Level_Manager.Instance.SetHealth((float)m_curHealth / maxHealth);
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
        Level_Manager.Instance.SetHealth((float)m_curHealth / maxHealth);
    }
}
