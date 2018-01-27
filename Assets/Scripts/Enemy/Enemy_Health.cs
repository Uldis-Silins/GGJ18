using UnityEngine;
using System.Collections;

public class Enemy_Health : MonoBehaviour
{
    public int maxHealth = 1;
    public Enemy_Health_Ragdoll ragdoll;
    public Level_EnemySpawner mySpawner;

    private int m_curHealth;
    private bool m_dead;

    private void Start()
    {
        ragdoll.transform.SetParent(null);
    }

    void OnEnable()
    {
        m_dead = false;
        m_curHealth = maxHealth;
        
        ragdoll.gameObject.SetActive(false);
    }


    public void SetDamage(int damageAmount)
    {
        m_curHealth -= damageAmount;

        if(!m_dead && m_curHealth <= 0)
        {
            Level_Manager.Instance.AddScore(1);

            ragdoll.SpawnRagdoll(transform.position, transform.rotation, GetComponent<Rigidbody>().velocity);
            gameObject.SetActive(false);
            mySpawner.RquestRespawn();
            m_dead = true;
        }
    }
}
