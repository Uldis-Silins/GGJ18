using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_LeftArmTest : MonoBehaviour
{
    public Transform leftShoulder;
    public Vector3 leftShoulderRotation;

	// Use this for initialization
	void Start ()
    {
        leftShoulder.rotation = Quaternion.Euler(leftShoulderRotation);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }
}
