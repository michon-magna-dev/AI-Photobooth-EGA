using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum TopicVertexSelection
{
    BY_INDEX,
    ONLY_MIDDLE,
    ALL
}

[RequireComponent(typeof(SphereButtonManager))]
public class SphereTopicGenerator : MonoBehaviour
{
    [SerializeField] SphereButtonManager buttonManager;
    [Description("Creates the Topic Buttons on the Outer Vertices")]
    [SerializeField] VertexPoints _vectorPoint;
    [Space]
    [Tooltip("Chooses Selected Vertices where the button will spawn")]
    [SerializeField] TopicVertexSelection m_vertexSelection = TopicVertexSelection.ALL;
    [SerializeField] int[] m_selectedVertices;// should have choice to randomize or use these instead

    [Description("This is the topic names shown on the buttons.")]
    [SerializeField] GameObject[] m_topicButtonObjs;
    [SerializeField] string[] m_topics;
    [Space]
    public bool m_useImageButton = false;
    [SerializeField] Sprite[] m_imageTopicSprites;
    [SerializeField] Sprite[] m_imageUnselectedTopicSprites;
    [SerializeField] GameObject m_floatingButtonPrefab;

    void Start()
    {
        buttonManager = GetComponent<SphereButtonManager>();
        //TODO: m_topics GET IN CONFIG LATER
        //Invoke("SpawnFloatingButtons", 0.3f);
        Invoke("SpawnFloatingButtonsFromSelectedIndexes", 0.3f);
    }

    private void SpawnFloatingButtons()
    {
        m_topicButtonObjs = new GameObject[m_topics.Length];

        //By Default This will get all surface point Indexes. 
        var pointList = GetOuterSurfacePositions();
        var selectedTopicIndexes = GetRandomVertexIndexes();

        var parentObj = new GameObject("Topic Buttons");
        parentObj.transform.parent = this.transform;

        for (int vectorIndex = 0; vectorIndex < pointList.Length; vectorIndex++)
        {
            if (selectedTopicIndexes.Contains(vectorIndex))//if it is selected as part of the random
            {
                var topicName = m_topics[selectedTopicIndexes.IndexOf(vectorIndex)];
                SpawnFloatingButton(topicName, vectorIndex, parentObj.transform);
            }
        }
    }
    private void SpawnFloatingButtonsFromSelectedIndexes()
    {
        m_topicButtonObjs = new GameObject[m_topics.Length];

        //By Default This will get all surface point Indexes. 
        var parentObj = new GameObject("Topic Buttons");
        parentObj.transform.parent = this.transform;

        for (int vectorIndex = 0; vectorIndex < m_selectedVertices.Length; vectorIndex++)
        {
            //var topicName = m_topics[m_selectedVertices[0]];
            SpawnFloatingButton($"Button{vectorIndex}", m_selectedVertices[vectorIndex], parentObj.transform);
        }
    }

    private Vector3[] GetOuterSurfacePositions(bool p_selectOnlyMiddleRangedAngle)
    {
        if (p_selectOnlyMiddleRangedAngle)
            return _vectorPoint.GetAngledOutwardSurfacePoints();
        else
            return _vectorPoint.GetOutwardSurfacePoints();
    }

    private Vector3[] GetOuterSurfacePositions()
    {
        switch (m_vertexSelection)
        {
            case TopicVertexSelection.BY_INDEX:
                return _vectorPoint.GetOutwardSurfacePoints();
            case TopicVertexSelection.ONLY_MIDDLE:
                return _vectorPoint.GetAngledOutwardSurfacePoints();
            case TopicVertexSelection.ALL:
                return _vectorPoint.GetOutwardSurfacePoints();
            default:
                return _vectorPoint.GetOutwardSurfacePoints();
        }
    }

    public List<int> GetRandomVertexIndexes()
    {
        List<int> indexList = new List<int>();
        var pointListCount = _vectorPoint.GetOutwardSurfacePoints().Length;
        while (indexList.Count < m_topics.Length)
        {
            int genIndex = Random.Range(0, pointListCount);
            if (!indexList.Contains(genIndex))
            {
                indexList.Add(genIndex);
            }
        }
        return indexList;
    }

    private void SpawnFloatingButton(string p_topicName, int p_selectedIndex, Transform p_parent = null)
    {
        var spawnedObj = Instantiate(m_floatingButtonPrefab, p_parent);
        int objIndex = spawnedObj.transform.GetSiblingIndex();
        spawnedObj.transform.position = _vectorPoint.GetOuterSurfacePointPosition(p_selectedIndex);

        FloatingTopic objTopicComponent = spawnedObj.GetComponent<FloatingTopic>();
        objTopicComponent.SetVertexToFollow(_vectorPoint.GetVertexObject(p_selectedIndex));

        if (m_useImageButton)
        {
            Destroy(spawnedObj.GetComponentInChildren<TextMeshProUGUI>().gameObject);
            RenameSpawnedObj(spawnedObj);
        }
        else
        {
            spawnedObj.name = p_topicName;
            spawnedObj.GetComponentInChildren<TextMeshProUGUI>().text = p_topicName;
        }

        spawnedObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            buttonManager.ButtonSelected(objIndex);
            objTopicComponent.SetSelected();
        });

        m_topicButtonObjs[objIndex] = spawnedObj;
    }

    #region Getters
    public string GetTopicName(int p_index)
    {
        for (int index = 0; index < m_topics.Length; index++)
        {
            if (p_index == index)
            {
                return m_topics[index];
            }
        }
        Debug.LogError($"Could not find element");
        return null;
    }
    #endregion

    #region  UI

    private void RenameSpawnedObj(GameObject p_spawnedObj)
    {
        var spriteUsed = m_imageTopicSprites[p_spawnedObj.transform.GetSiblingIndex()];
        string spriteName = spriteUsed.name;
        try
        {
            spriteName = spriteName.Split('_')[0];
        }
        catch (System.Exception)
        {
            Debug.LogWarning($"Could not Rename Sprite {spriteName}");
        }
        p_spawnedObj.GetComponentInChildren<Image>().sprite = spriteUsed;
        p_spawnedObj.GetComponentInChildren<FloatingTopic>().SetUnselectedSprite(m_imageUnselectedTopicSprites[p_spawnedObj.transform.GetSiblingIndex()]);
        p_spawnedObj.gameObject.name = spriteName;
    }

    public void ShowButtons(bool p_showButton)
    {
        foreach (var item in m_topicButtonObjs)
        {
            item.SetActive(p_showButton);
        }
    }
    #endregion


}