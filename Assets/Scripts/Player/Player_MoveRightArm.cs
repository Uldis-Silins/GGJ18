using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MoveRightArm : MonoBehaviour
{
    public Transform[] jointTransforms;
    public float xSpeed = 0.1f;
    public float ySpeed = 0.1f;

    private Vector3 m_defaultShoulderRotation;
    private Vector3 m_defaultArmRotation;

	// Use this for initialization
	void Start ()
    {
        m_defaultShoulderRotation = jointTransforms[0].localEulerAngles;
        m_defaultArmRotation = jointTransforms[1].localEulerAngles;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float xInput = (Input.mousePosition.x - Screen.width / 2) * xSpeed;
        float yInput = (Input.mousePosition.y - Screen.height / 2) * ySpeed;
        FollowMouse(new Vector3(xInput, yInput, 0));
	}

    private void FollowMouse(Vector3 input)
    {
        jointTransforms[0].localRotation = Quaternion.Euler(m_defaultShoulderRotation - input);
        //jointTransforms[1].localRotation = Quaternion.Euler(m_defaultArmRotation - (input * 0.3f));
    }
}
