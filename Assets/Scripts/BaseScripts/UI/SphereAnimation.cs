using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAnimation : MonoBehaviour
{
    [SerializeField] VertexPoints m_vertexPointManager;
    [SerializeField] SphereTopicGenerator m_topicManager;
    [SerializeField] float speed = 1f;
    bool isAnimating = false;

    private void Start()
    {
        m_vertexPointManager = GetComponent<VertexPoints>();
        m_topicManager = GetComponent<SphereTopicGenerator>();
        isAnimating = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            EnlargeOuterShell();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShrinkOuterShell();
        }
    }

    public void EnlargeOuterShell()
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimateEnlargeShell());
        }
    }

    public void ShrinkOuterShell()
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimateShrinkShell());
        }
    }

    public IEnumerator AnimateEnlargeShell()
    {
        float t = 1;
        isAnimating = true;
        while (t < m_vertexPointManager.GetDistanceMultiplier)
        {
            t += Time.deltaTime * (speed * 10);
            yield return new WaitForSeconds(0);
            m_vertexPointManager.SetMultiplierDistance(t);
        }
        m_topicManager.ShowButtons(true);
        m_vertexPointManager.SetMultiplierDistance(m_vertexPointManager.GetDistanceMultiplier);
        isAnimating = false;
    }

    public IEnumerator AnimateShrinkShell()
    {
        float t = m_vertexPointManager.GetDistanceMultiplier;
        isAnimating = true;
        while (t > 1)
        {
            t -= Time.deltaTime * (speed * 10);
            yield return new WaitForSeconds(0);
            m_vertexPointManager.SetMultiplierDistance(t);
        }
        m_vertexPointManager.SetMultiplierDistance(1);
        m_topicManager.ShowButtons(false);
        isAnimating = false;
    }
}