using UnityEngine;
using System.Collections.Generic;

public class Player_HitHint : MonoBehaviour
{
    public UnityEngine.UI.Image hintIcon;
    public float hintFadeTime = 1f;
    public float hintDistance = 20f;

    private float m_hintFadeTimer;
    private bool m_hintEnabled;

    private List<Transform> m_incomingProjectiles;
    private Transform m_curProjectile;

    private Color m_fullWhite = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    private Color m_whiteAlpha = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    private int m_hintLayer;

    private Camera mainCam;
    private Quaternion m_defaultCamRotation;

    public List<Transform> IncomingProjectiles
    {
        get { return m_incomingProjectiles; }
    }

    private void Awake()
    {
        m_incomingProjectiles = new List<Transform>();
        mainCam = Camera.main;
    }

    private void Start()
    {
        m_hintLayer = LayerMask.GetMask("HitHint");

        //hintIcon.gameObject.SetActive(false);
        hintIcon.color = m_whiteAlpha;
        m_hintFadeTimer = hintFadeTime;
        m_hintEnabled = false;

        m_defaultCamRotation = mainCam.transform.rotation;

        Level_Manager.Instance.hitHint = this;
    }

    private void LateUpdate()
    {
        if (m_hintEnabled)
        {
            m_hintFadeTimer -= Time.deltaTime;

            if (m_curProjectile != null)
            {
                float t = Vector3.Distance(m_curProjectile.position, transform.position) / hintDistance;
                hintIcon.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 0.1f, t);
                hintIcon.color = Color.Lerp(m_fullWhite, m_whiteAlpha, t);
            }
            else
            {
                if (m_hintFadeTimer < 0)
                {
                    hintIcon.color = m_whiteAlpha;
                    m_hintEnabled = false;
                }
            }
        }


        if (m_curProjectile != null && !m_hintEnabled)
        {
            RaycastHit hit;

            if (Physics.Raycast(m_curProjectile.position, m_curProjectile.forward, out hit, hintDistance, m_hintLayer))
            {
                //Debug.DrawLine(m_curProjectile.position, hit.point);
                DrawHint();
            }
        }

        RotateCamera();
    }

    public void DrawHint()
    {
        m_hintFadeTimer = hintFadeTime;
        hintIcon.color = m_fullWhite;
        hintIcon.transform.position = Camera.main.WorldToScreenPoint(m_curProjectile.position);
        m_hintEnabled = true;
    }

    public void AddProjectile(Transform projectile)
    {
        m_incomingProjectiles.Add(projectile);
        CalcCurrentProjectile();
    }

    public void RemoveProjectile(Transform projectile)
    {
        m_incomingProjectiles.Remove(projectile);
        CalcCurrentProjectile();
    }

    private void CalcCurrentProjectile()
    {
        if(m_incomingProjectiles.Count <= 0)
        {
            m_curProjectile = null;
            return;
        }

        float minDist = float.MaxValue;

        for (int i = 0; i < m_incomingProjectiles.Count; i++)
        {
            float tempDist = Vector3.Distance(m_incomingProjectiles[i].position, transform.position);

            if (tempDist < minDist)
            {
                minDist = tempDist;
                m_curProjectile = m_incomingProjectiles[i];
            }
        }
    }

    private void RotateCamera()
    {
        Quaternion lookRot = mainCam.transform.rotation;

        if (m_curProjectile != null)
        {
            if (Vector3.Distance(m_curProjectile.position, mainCam.transform.position) > 7f)
            {
                lookRot = Quaternion.LookRotation(m_curProjectile.position - transform.position);
            }
        }
        else
        {
            lookRot = m_defaultCamRotation;
        }

        mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, lookRot, 0.5f * Time.deltaTime);
    }
}