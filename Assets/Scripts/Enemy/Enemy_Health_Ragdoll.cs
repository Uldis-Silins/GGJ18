using UnityEngine;
using System.Collections;

public class Enemy_Health_Ragdoll : MonoBehaviour
{
    public Transform modelRoot;
    public Transform ragdollRoot;

    // Use this for initialization
    void Start()
    {

    }

    public void SpawnRagdoll(Vector3 position, Quaternion rotation, Vector3 moveVelocity)
    {
        transform.position = position;
        transform.rotation = rotation;

        gameObject.SetActive(true);

        CopyTransformsRecursive(modelRoot, ragdollRoot, moveVelocity);
    }

    public void CopyTransformsRecursive(Transform src, Transform dst, Vector3 velocity)
    {
        Rigidbody body = dst.GetComponent<Rigidbody>();

        if (body != null)
        {
            body.isKinematic = true;

        }

        dst.position = src.position;
        dst.rotation = src.rotation;

        if (body != null)
        {
            body.isKinematic = false;
            //body.velocity = velocity;
            body.useGravity = true;

        }

        foreach (Transform child in dst)
        {
            Transform curSrc = src.Find(child.name);

            if (curSrc)
            {
                CopyTransformsRecursive(curSrc, child, velocity);
            }
        }

    }
}
