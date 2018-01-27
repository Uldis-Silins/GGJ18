using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_EnemySpawner : MonoBehaviour
{
    public Enemy_Controller enemyPrefab;
    public Transform[] waypoints;

    public float respawnDelay = 2f;

    private Enemy_Controller m_mySpawnedEnemy;

	// Use this for initialization
	void Start ()
    {
        m_mySpawnedEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        m_mySpawnedEnemy.GetComponent<Enemy_Health>().mySpawner = this;
        m_mySpawnedEnemy.waypoints = this.waypoints;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnDrawGizmos()
    {
        Color oldGizmosColor = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(waypoints[0].position, 0.5f);
        Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);

        if (waypoints.Length > 1)
        {
            for (int i = 1; i < waypoints.Length; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i - 1].position);
                Gizmos.DrawSphere(waypoints[i].position, 0.5f);
            }
        }

        Gizmos.color = oldGizmosColor;
    }

    private void SpawnEnemy()
    {
        m_mySpawnedEnemy.transform.SetPositionAndRotation(transform.position, transform.rotation);
        m_mySpawnedEnemy.gameObject.SetActive(true);
    }

    public void RquestRespawn()
    {
        Invoke("SpawnEnemy", respawnDelay);
    }
}
