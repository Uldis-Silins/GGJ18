using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MoveRightArm : MonoBehaviour
{
    public HackMotion motion;
    public Transform[] jointTransforms;
    public float xSpeed = 0.1f;
    public float ySpeed = 0.1f;

    private Vector3 m_defaultShoulderRotation;
    private Vector3 m_defaultArmRotation;

    private Player_Weapon_Audio m_audio;

    private Vector3 m_prevInput;

    private void Awake()
    {
        m_audio = GetComponent<Player_Weapon_Audio>();
    }

    void Start ()
    {
        m_defaultShoulderRotation = jointTransforms[0].localEulerAngles;
        m_defaultArmRotation = jointTransforms[1].localEulerAngles;

        m_prevInput = Vector2.zero;

    }
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 1; i < 5; i++)
        {
            jointTransforms[i - 1].localPosition = motion.m_positions[((HackMotion.SensorID)i)];
            jointTransforms[i - 1].localRotation = motion.m_rotations[((HackMotion.SensorID)i)];
        }

        if (Vector3.Distance(motion.m_positions[((HackMotion.SensorID)1)], m_prevInput) > 0.01f)
        {
            m_audio.SetState(Player_Weapon_Audio.ClipStates.Move);
        }

        m_prevInput = motion.m_positions[((HackMotion.SensorID)1)];

        //if (Level_Manager.Instance.CurrentGameState == Level_Manager.GameState.Play)
        //{
        //    float xInput = (Input.mousePosition.x - Screen.width / 2) * xSpeed;
        //    float yInput = (Input.mousePosition.y - Screen.height / 2) * ySpeed;
        //    FollowMouse(new Vector3(xInput, yInput, 0));

        //    if(Mathf.Abs(xInput - m_prevInput.x) + Mathf.Abs(m_prevInput.y - yInput) > 1f)
        //    {
        //        m_audio.SetState(Player_Weapon_Audio.ClipStates.Move);
        //    }

        //    m_prevInput.x = xInput;
        //    m_prevInput.y = yInput;
        //}
    }

    private void FollowMouse(Vector3 input)
    {
        //jointTransforms[0].localRotation = Quaternion.Euler(m_defaultShoulderRotation - input);
        //jointTransforms[1].localRotation = Quaternion.Euler(m_defaultArmRotation - (input * 0.3f));
    }
}
