using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanelManager : Singleton<VictoryPanelManager>
{
    [Header("Action Buttons"), Space(5f)]
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _nextButton;
    [Header("Level Content"), Space(5f)]
    [SerializeField] private TMP_Text _levelName;
    [SerializeField] private TMP_Text _bestRecord;
    [SerializeField] private TMP_Text _currentRecord;

    void Start()
    {
        _retryButton.onClick.AddListener(RetryButton_Click);
        _nextButton.onClick.AddListener(NextButton_Click);
    }

    void OnEnable()
    {
        var tmpData = GameManager.Instance.LevelData;
        _levelName.text = tmpData.Item1;
        _bestRecord.text = tmpData.Item2;
    }

    void RetryButton_Click()
    {
        UIManager.Instance.OnStart();
        GameManager.Instance.LoadMap();
        GameManager.Instance.RespawnPlayer();
    }

    void NextButton_Click()
    {
        UIManager.Instance.OnReturnSelectMenu();
    }
}
