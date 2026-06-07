using UnityEngine;

public class TabController : MonoBehaviour
{
    [Header("Content Panels")]
    [SerializeField] private GameObject _soilContent;
    [SerializeField] private GameObject _plantsContent;
    [SerializeField] private GameObject _decorContent;
    [SerializeField] private GameObject _foodContent;
    [SerializeField] private GameObject _colonyContent;

    // ---------------------------------------------------------------

    private void Start()
    {
        // Start the game with only the soil tab open
        ShowSoilTab();
    }

    // HIDE EVERYTHING BUT THE SELECTED TAB
    public void ShowSoilTab()
    {
        HideAllTabs();
        _soilContent.SetActive(true);
    }

    public void ShowPlantsTab()
    {
        HideAllTabs();
        _plantsContent.SetActive(true);
    }

    public void ShowDecorTab()
    {
        HideAllTabs();
        _decorContent.SetActive(true);
    }

    public void ShowFoodTab()
    {
        HideAllTabs();
        _foodContent.SetActive(true);
    }

    public void ShowColonyTab()
    {
        HideAllTabs();
        _colonyContent.SetActive(true);
    }

    private void HideAllTabs()
    {
        _soilContent.SetActive(false);
        _plantsContent.SetActive(false);
        _decorContent.SetActive(false);
        _foodContent.SetActive(false);
        _colonyContent.SetActive(false);
    }
}
