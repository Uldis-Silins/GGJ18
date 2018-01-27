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

    private Color m_whiteAlpha = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    private int m_hintLayer;

    private void Awake()
    {
        m_incomingProjectiles = new List<Transform>();
    }

    private void Start()
    {
        m_hintLayer = LayerMask.GetMask("HitHint");

        //hintIcon.gameObject.SetActive(false);
        hintIcon.color = m_whiteAlpha;
        m_hintFadeTimer = hintFadeTime;
        m_hintEnabled = false;
    }

    private void LateUpdate()
    {
        if (m_hintEnabled)
        {
            m_hintFadeTimer -= Time.deltaTime;

            float t = 1f - (m_hintFadeTimer / hintFadeTime);

            hintIcon.color = Color.Lerp(Color.white, m_whiteAlpha, t);

            if (m_curProjectile != null)
            {
                hintIcon.transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one, Vector3.Distance(m_curProjectile.position, transform.position) / hintDistance);
            }

            if (m_hintFadeTimer < 0)
            {
                //hintIcon.gameObject.SetActive(false);
                m_hintEnabled = false;
            }
        }


        if (m_curProjectile != null && !m_hintEnabled)
        {
            RaycastHit hit;

            if (Physics.Raycast(m_curProjectile.position, m_curProjectile.forward, out hit, hintDistance, m_hintLayer))
            {
                Debug.DrawLine(m_curProjectile.position, hit.point);
                DrawHint(hit.point);
            }
        }
    }

    public void DrawHint(Vector3 pos)
    {
        m_hintFadeTimer = hintFadeTime;
        //hintIcon.gameObject.SetActive(true);
        hintIcon.color = Color.white;
        hintIcon.transform.position = Camera.main.WorldToScreenPoint(pos);
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
}