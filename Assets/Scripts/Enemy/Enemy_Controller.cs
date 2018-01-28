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
    public Transform[] waypoints;

    public Camera mainCam;

    public AudioSource speachAudio;
    public AudioClip[] spawnClips;
    public AudioClip[] attackClips;
    public AudioClip[] killPlayerClips;

    public float rotationSpeed = 5f;

    private readonly int m_fireAnimHash = Animator.StringToHash("Firing");
    private readonly int m_turnAnimHash = Animator.StringToHash("Turn");
    private readonly int m_moveAnimHash = Animator.StringToHash("Move");

    private Enemy_Animator_RootMotion m_motion;
    private int m_curWaypointID;

    private float m_fireTimer = 3f;

    private void Awake()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        mainCam = Camera.main;
        m_motion = animator.GetComponent<Enemy_Animator_RootMotion>();
        m_motion.aimTarget = player;
    }

    // Use this for initialization
    void Start ()
    {
        m_currentState = new State(EnterState_Idle);
    }

    private void OnDisable()
    {
        
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(CalculateWeaponSpread(), transform.position);
    }

    // Update is called once per frame
    void Update ()
    {
        if(m_currentState != null)
        {
            m_currentState();
        }
	}

    public void OnSpawn()
    {
        m_curWaypointID = 0;
        //animator.SetFloat(m_moveAnimHash, 0);
        //animator.SetBool(m_fireAnimHash, false);
        m_motion.applyFireAnimCorrection = false;
        m_currentState = EnterState_Idle;
    }

    #region States

    private void EnterState_Idle()
    {
        //Debug.Log("Enter state Idle; t: " + Time.time);
        //m_motion.applyFireAnimCorrection = false;
        //animator.SetBool(m_fireAnimHash, false);
        //m_fireTimer = 5f;
        m_currentState = State_Idle;
    }

    private void State_Idle()
    {
        //Debug.Log("State Idle; t: " + Time.time);
        //m_fireTimer -= Time.deltaTime;

        //if (m_fireTimer < 0)
        //{
        //    ExitState_Idle(EnterState_Attack);
        //}

        if (Level_Manager.Instance.CurrentGameState == Level_Manager.GameState.Play)
        {
            ExitState_Idle(EnterState_Move);
        }
    }

    private void ExitState_Idle(State targetState)
    {
        speachAudio.clip = spawnClips[Random.Range(0, spawnClips.Length)];
        speachAudio.Play();

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

        //transform.rotation = m_motion.animRootRotation;

        speachAudio.clip = attackClips[Random.Range(0, attackClips.Length)];
        speachAudio.Play();

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
        if (Level_Manager.Instance.CurrentGameState == Level_Manager.GameState.GameOver)
        {
            ExitState_Attack(EnterState_Idle);
        }

        weapon.muzzle.LookAt(mainCam.transform.position + CalculateWeaponSpread(0.5f));
        weapon.Fire();

        if (m_fireTimer < 0)
        {
            ExitState_Attack(EnterState_Idle);
        }

        m_fireTimer -= Time.deltaTime;
    }

    private void ExitState_Attack(State targetState)
    {
        animator.SetBool(m_fireAnimHash, false);
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
        if (Level_Manager.Instance.CurrentGameState == Level_Manager.GameState.GameOver)
        {
            ExitState_Move(EnterState_Idle);
            speachAudio.clip = killPlayerClips[Random.Range(0, killPlayerClips.Length)];
            speachAudio.Play();
        }

        Vector3 targetDir = waypoints[m_curWaypointID].position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        //transform.position += transform.forward * 5f * Time.deltaTime;
        animator.SetFloat(m_moveAnimHash, 1);
        transform.position += m_motion.animMoveDelta;

        if (Vector3.Distance(waypoints[m_curWaypointID].position, transform.position) < 0.5f)
        {
            ExitState_Move(EnterState_Attack);   
        }
    }

    private void ExitState_Move(State targetState)
    {
        animator.SetFloat(m_moveAnimHash, 0);
        m_curWaypointID = Random.Range(0, waypoints.Length);
        m_currentState = targetState;
    }
    #endregion  // ~States

    public Vector3 CalculateWeaponSpread(float amount)
    {
        amount = Mathf.Clamp01(amount);
        Vector3 spread = mainCam.ViewportToWorldPoint(Vector3.one * 0.5f) * amount;

        return new Vector3(Random.Range(-spread.x, spread.x), Random.Range(-spread.y, spread.y), 0);
    }
}
