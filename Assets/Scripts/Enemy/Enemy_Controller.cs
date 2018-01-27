using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    private delegate void State();

    private State m_currentState;

    public Enemy_Weapon weapon;
    public Animator animator;
    public Transform player;    // TODO: Remove?
    public Transform fireTarget;
    public Transform[] waypoints;

    public Camera mainCam;

    public float rotationSpeed = 5f;

    private readonly int m_fireAnimHash = Animator.StringToHash("Firing");
    private readonly int m_turnAnimHash = Animator.StringToHash("Turn");

    private Enemy_Animator_RootMotion m_motion;
    private int m_curWaypointID;

    private float m_fireTimer = 10f;

    private void Awake()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        mainCam = Camera.main;
        fireTarget = mainCam.transform;
        m_motion = animator.GetComponent<Enemy_Animator_RootMotion>();
        m_motion.aimTarget = player;
    }

    // Use this for initialization
    void Start ()
    {
        m_curWaypointID = Random.Range(0, waypoints.Length);
        m_currentState = new State(EnterState_Idle);
	}

    private void OnDisable()
    {
        m_motion.applyFireAnimCorrection = false;
        m_currentState = EnterState_Idle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(CalculateWeaponSpread(), transform.position);
    }

    // Update is called once per frame
    void Update ()
    {
        if(m_currentState != null)
        {
            m_currentState();
        }
	}

    private void EnterState_Idle()
    {
        //Debug.Log("Enter state Idle; t: " + Time.time);
        //m_motion.applyFireAnimCorrection = false;
        //animator.SetBool(m_fireAnimHash, false);
        //m_fireTimer = 5f;
        m_currentState = State_Idle;
    }

    #region States

    private void State_Idle()
    {
        //Debug.Log("State Idle; t: " + Time.time);
        //m_fireTimer -= Time.deltaTime;

        //if (m_fireTimer < 0)
        //{
        //    ExitState_Idle(EnterState_Attack);
        //}

        ExitState_Idle(EnterState_Move);
    }

    private void ExitState_Idle(State targetState)
    {
        m_currentState = targetState;
    }

    private void EnterState_Attack()
    {
        Vector3 targetDir = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        //Debug.Log(angle);
        //animator.SetFloat(m_turnAnimHash, angle);

        //transform.rotation = m_motion.animRootRotation;

        if (angle < 10f)
        {
            m_motion.applyFireAnimCorrection = true;
            animator.SetBool(m_fireAnimHash, true);
            m_fireTimer = 10f;
            m_currentState = State_Attack;
        }
    }

    private void State_Attack()
    {
        weapon.muzzle.LookAt(fireTarget.position + CalculateWeaponSpread());
        weapon.Fire();

        m_fireTimer -= Time.deltaTime;

        if (m_fireTimer < 0)
        {
            ExitState_Attack(EnterState_Idle);
        }
    }

    private void ExitState_Attack(State targetState)
    {
        m_currentState = targetState;
    }

    private void EnterState_Move()
    {
        Vector3 targetDir = waypoints[m_curWaypointID].position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        //Debug.Log(angle);
        //animator.SetFloat(m_turnAnimHash, angle);

        //transform.rotation = m_motion.animRootRotation;

        if (angle < 1f)
        {
            m_currentState = State_Move;
        }
    }

    private void State_Move()
    {
        Vector3 targetDir = waypoints[m_curWaypointID].position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * 5f * Time.deltaTime;

        if (Vector3.Distance(waypoints[m_curWaypointID].position, transform.position) < 0.5f)
        {
            ExitState_Move(EnterState_Attack);   
        }
    }

    private void ExitState_Move(State targetState)
    {
        m_curWaypointID = Random.Range(0, waypoints.Length);
        m_currentState = targetState;
    }
    #endregion  // ~States

    public Vector3 CalculateWeaponSpread()
    {
        // TODO: Calc clip planes

        Vector3 spread = Vector3.one * 0.1f;

        return new Vector3(Random.Range(-spread.x, spread.x), Random.Range(-spread.y, spread.y), 0);
    }
}
