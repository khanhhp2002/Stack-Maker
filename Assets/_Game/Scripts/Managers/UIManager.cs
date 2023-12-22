using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("UI Component"), Space(5f)]
    [SerializeField] private SelectPanelManager _selectPanelManager;
    [SerializeField] private VictoryPanelManager _victoryPanelManager;

    public void OnStart()
    {
        _selectPanelManager.gameObject.SetActive(false);
        _victoryPanelManager.gameObject.SetActive(false);
    }

    public void OnVictory()
    {
        _victoryPanelManager.gameObject.SetActive(true);
    }

    public void OnReturnSelectMenu()
    {
        _victoryPanelManager.gameObject.SetActive(false);
        _selectPanelManager.gameObject.SetActive(true);
        LevelManager.Instance.NextMap();
    }
}
