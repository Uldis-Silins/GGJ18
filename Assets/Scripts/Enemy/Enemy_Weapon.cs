using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Weapon : MonoBehaviour
{
    public Transform muzzle;

    public AudioSource fireAudioSource;
    public AudioClip[] fireClips;

    [Tooltip("Rounds fired per minute")]
    public int rateOfFire = 60;

    public Enemy_Weapon_Projectile projectilePrefab;
    public Rigidbody dummyProjectile;

    private float m_fireDelay;
    private float m_fireTimer;

    private Enemy_Weapon_Projectile[] m_projectiles;
    private int m_curProjectileID;
    private const int SPAWNED_PROJECTILES = 10;

    private void Awake()
    {
        m_projectiles = new Enemy_Weapon_Projectile[SPAWNED_PROJECTILES];
    }

    // Use this for initialization
    void Start ()
    {
        m_fireDelay = (float)60 / rateOfFire;
        m_fireTimer = 0f;

        for (int i = 0; i < SPAWNED_PROJECTILES; i++)
        {
            m_projectiles[i] = Instantiate<Enemy_Weapon_Projectile>(projectilePrefab, transform.position, Quaternion.identity);
            m_projectiles[i].gameObject.name += i;
            m_projectiles[i].Disable();
        }

        m_curProjectileID = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        m_fireTimer -= Time.deltaTime;
	}

    public void Fire()
    {
        if (m_fireTimer <= 0)
        {
            m_projectiles[m_curProjectileID % m_projectiles.Length].Fire(muzzle.position, muzzle.rotation);
            m_curProjectileID++;
            m_fireTimer = m_fireDelay;

            fireAudioSource.clip = fireClips[Random.Range(0, fireClips.Length)];
            fireAudioSource.Play();
        }
    }
}
