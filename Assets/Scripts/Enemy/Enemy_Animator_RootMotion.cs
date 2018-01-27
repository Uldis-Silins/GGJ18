using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy_Animator_RootMotion : MonoBehaviour
{
    public Vector3 animMoveDelta;
    public Quaternion animRootRotation;
    public Transform aimTarget;

    public Vector3 fireAnimOffset = new Vector3(0f, 50f, 0f);
    public bool applyFireAnimCorrection;

    private Animator m_animator;
    private Transform m_chest;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_chest = m_animator.GetBoneTransform(HumanBodyBones.Chest);
    }

    private void LateUpdate()
    {
        if (applyFireAnimCorrection)
        {
            m_chest.LookAt(aimTarget);
            m_chest.rotation *= Quaternion.Euler(fireAnimOffset);
        }
    }

    private void OnAnimatorMove()
    {
        animMoveDelta = m_animator.deltaPosition;
        animRootRotation = m_animator.rootRotation;
    }
}
