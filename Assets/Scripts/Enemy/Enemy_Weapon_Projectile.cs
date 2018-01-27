using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Enemy_Weapon_Projectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 10f;
    public float reflectedSpeed = 20f;
    public float sabreLookAhead = 2f;
    public float lifetime = 5f;

    private Rigidbody m_rigidbody;
    private Vector3 m_prevPosition;

    private Player_Health m_playerHealth;
    private int m_playerLayer;
    private int m_sabreLayer;
    private int m_enemyLayer;

    private bool m_reflected;

    private void Awake()
    {
        m_rigidbody = transform.GetComponent<Rigidbody>();
        m_playerLayer = LayerMask.GetMask("Player");
        m_sabreLayer = LayerMask.GetMask("Sabre");
        m_enemyLayer = LayerMask.GetMask("Enemy");
        m_playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Health>();
    }

    void Start()
    {
        m_prevPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Linecast(transform.position, m_prevPosition, m_playerLayer))
        {
            m_playerHealth.SetDamage(damage);
            Disable();
        }

        RaycastHit enemyHit;

        if (m_reflected && Physics.Linecast(transform.position, m_prevPosition, out enemyHit, m_enemyLayer))
        {
            enemyHit.collider.GetComponent<Enemy_Health>().SetDamage(damage);
            Disable();
        }

        if (!m_reflected && Physics.Linecast(transform.position, transform.position + transform.forward * sabreLookAhead, m_sabreLayer))
        {
            Reflect();
        }
    }

    public void Fire(Vector3 position, Quaternion rotation)
    {
        this.transform.position = position;
        this.transform.rotation = rotation;
        gameObject.SetActive(true);
        m_rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        m_reflected = false;

        if (IsInvoking("Disable"))
        {
            CancelInvoke("Disable");
        }

        Invoke("Disable", lifetime);
    }

    public void Disable()
    {
        m_rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void Reflect()
    {
        m_reflected = true;
        m_rigidbody.velocity = -m_rigidbody.velocity.normalized * reflectedSpeed;
    }
}
