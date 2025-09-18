using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelAnimation : MonoBehaviour
{
    public GameObject[] panels;
    public Image panelImage;

    private int currentPanelIndex = 0;

    void Start()
    {
        // Hide all panels except the first one
        for (int i = 1; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
    }

    public void TransitionToPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= panels.Length)
            return;

        // Get the current panel and next panel
        GameObject currentPanel = panels[currentPanelIndex];
        GameObject nextPanel = panels[panelIndex];

        // Hide the current panel
        currentPanel.SetActive(false);

        // Set the next panel to active
        nextPanel.SetActive(true);

        // Fade out the current panel (if using a CanvasGroup)
        CanvasGroup currentCanvasGroup = currentPanel.GetComponent<CanvasGroup>();
        currentCanvasGroup.DOFade(0f, 0.5f).OnComplete(() => currentPanel.SetActive(false));

        // Fade in the next panel (if using a CanvasGroup)
        CanvasGroup nextCanvasGroup = nextPanel.GetComponent<CanvasGroup>();
        nextCanvasGroup.alpha = 0f;
        nextCanvasGroup.DOFade(1f, 0.5f);

        currentPanelIndex = panelIndex;
    }
}
