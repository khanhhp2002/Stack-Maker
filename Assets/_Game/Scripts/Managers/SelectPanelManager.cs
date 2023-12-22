using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanelManager : Singleton<SelectPanelManager>
{
    [Header("Select Buttons"), Space(5f)]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Button _reloadButton;

    [Header("Level Content"), Space(5f)]
    [SerializeField] private TMP_Text _levelName;
    [SerializeField] private TMP_Text _bestRecord;

    [Header("Play Button"), Space(5f)]
    [SerializeField] private Button _playButton;

    [Header("Input Field"), Space(5f)]
    [SerializeField] private TMP_InputField _customSeed;

    [Header("Custom Panel"), Space(5f)]
    [SerializeField] private GameObject _customPanel;

    public bool CanClick = true;

    // Start is called before the first frame update
    void Start()
    {
        _leftButton.onClick.AddListener(LeftButton_Click);
        _rightButton.onClick.AddListener(RightButton_Click);
        _playButton.onClick.AddListener(PlayButton_Click);
        _reloadButton.onClick.AddListener(ReloadButton_Click);
        _customSeed.onSubmit.AddListener(CustomSeed_OnSubmit);
        LevelManager.Instance.SetReference(_levelName, _bestRecord);
    }


    void PlayButton_Click()
    {
        if (CanClick)
        {
            CanClick = false;
            LevelManager.Instance.SetCurrentLevelData();
            UIManager.Instance.OnStart();
            GameManager.Instance.SpawnPlayer();
        }
    }

    void RightButton_Click()
    {
        if (CanClick)
        {
            CanClick = false;
            LevelManager.Instance.NextMap();
        }
    }

    void LeftButton_Click()
    {
        if (CanClick)
        {
            CanClick = false;
            LevelManager.Instance.PreviousMap();
        }
    }

    void ReloadButton_Click()
    {
        if (CanClick)
        {
            CanClick = false;
            LevelManager.Instance.GetNewCustomMap();
        }
    }
    void CustomSeed_OnSubmit(string arg0)
    {
        LevelManager.Instance.LoadCustomMap(arg0);
    }

    public void ShowCustomPanel()
    {
        _customPanel.SetActive(true);
    }

    public void HideCustomPanel()
    {
        _customPanel.SetActive(false);
    }

    public void ChangeCustomSeed(string seed)
    {
        _customSeed.text = $"Seed: {seed}";
    }
}
