using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageSelectionPromptHandler : MonoBehaviour
{
    public enum PromptType
    {
        Male,
        Female
    }

    [SerializeField] PromptType currentPromptType = PromptType.Male;
    [SerializeField] TextMeshProUGUI selectedTitleUI;
    [SerializeField] Transform selectionTransform;
    [SerializeField] Transform[] selectPositions = new Transform[3];
    [SerializeField] int selectedIndex = 0;
    [Space]
    [SerializeField] Sprite[] maleSprites;
    [SerializeField] Sprite[] femaleSprites;
    public int SelectedIndex => selectedIndex;
    private void OnEnable()
    {

        try
        {
            currentPromptType = GameManager.Instance.GetSelectedGender;
            SetPromptSelection(currentPromptType);
            //Invoke("SelectDefault", 1f);
        }
        catch (System.Exception)
        {
            SetPromptSelection(currentPromptType);
            //only happens on start for Game Manager 
        }
        selectedTitleUI.text = currentPromptType.ToString();
        SelectDefault();
        selectionTransform.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            MoveSelection(0);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            MoveSelection(1);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            MoveSelection(2);
        }
    }

    public void MoveSelection(int p_index)
    {
        if (p_index < 0 || p_index >= selectPositions.Length) return;
        selectionTransform.gameObject.SetActive(true);
        Debug.Log($"Selected Position: {selectedIndex}");
        selectionTransform.position = selectPositions[p_index].position;
        selectedIndex = p_index;
    }

    public void SetPromptSelection(PromptType p_promptGender)
    {
        //var selectedFromGameManager = GameManager.Instance.GetSelectedGender;
        ApplySprites(p_promptGender);
        switch (p_promptGender)
        {
            case PromptType.Male:
                selectPositions[2].gameObject.SetActive(true);
                MoveSelection(1);
                break;
            case PromptType.Female:
                selectPositions[2].gameObject.SetActive(false);
                MoveSelection(0);
                break;
            default:
                break;
        }
    }

    [ContextMenu("Select Default")]
    public void SelectDefault()
    {
        switch (currentPromptType)
        {
            case PromptType.Male:
                MoveSelection(1);
                break;
            case PromptType.Female:
                MoveSelection(0);
                break;
            default:
                break;
        }
    }

    void ApplySprites(PromptType p_promptGender)
    {
        Sprite[] targetSprites = p_promptGender == PromptType.Male ? maleSprites : femaleSprites;
        for (int i = 0; i < selectPositions.Length; i++)
        {
            if (p_promptGender.Equals(PromptType.Female) && i > 1)
            {
                continue;
            }
            selectPositions[i].gameObject.GetComponent<Image>().sprite = targetSprites[i];
        }
    }
}
