using System.Linq;
using TMPro;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private TMP_Text _levelName;
    private TMP_Text _bestRecord;
    private int _currentLevelIndex = 0;
    private int? _mapCount = null;

    // Start is called before the first frame update
    void Start()
    {
        //_levels = GetAllLevels();
        GameManager.Instance.LoadMap(LoadMapByIndex());
    }

    public void SetReference(TMP_Text levelName, TMP_Text bestRecord)
    {
        _bestRecord = bestRecord;
        _levelName = levelName;
    }

    public void SetCurrentLevelData()
    {
        GameManager.Instance.LevelData = (_levelName.text, _bestRecord.text, _currentLevelIndex);
    }

    private MapOS[] GetAllLevels()
    {
        return Resources.LoadAll<MapOS>("Maps");
    }

    public MapOS LoadMapByIndex(int? index = null)
    {
        return Resources.Load<MapOS>($"Maps/{(index is null ? _currentLevelIndex : index)}");
    }

    public void NextMap()
    {
        MapOS mapOS = LoadMapByIndex(_currentLevelIndex + 1);
        if (mapOS is null)
        {
            if (_mapCount is null)
            {
                _mapCount = ++_currentLevelIndex;
            }
            else if (_currentLevelIndex < _mapCount)
            {
                _currentLevelIndex++;
            }
            _levelName.text = $"custom level";
            SelectPanelManager.Instance.ShowCustomPanel();
            GetNewCustomMap();
        }
        else
        {
            _currentLevelIndex++;
            _levelName.text = $"level {_currentLevelIndex + 1}";
            GameManager.Instance.LoadMap(mapOS);
        }
    }

    public void PreviousMap()
    {
        SelectPanelManager.Instance.HideCustomPanel();
        if (_currentLevelIndex > 0)
        {
            _currentLevelIndex--;
            _levelName.text = $"level {_currentLevelIndex + 1}";
            GameManager.Instance.LoadMap(LoadMapByIndex());
        }
    }

    private string GenerateRandomSeed(int length = 10)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        System.Random random = new System.Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public void LoadCustomMap(string arg0)
    {
        string seed = string.IsNullOrEmpty(arg0) ? GenerateRandomSeed() : arg0;
        SelectPanelManager.Instance.ChangeCustomSeed(seed);
        GameManager.Instance.DestroyMap(System.Math.Abs(seed.GetHashCode()));
    }

    public void GetNewCustomMap()
    {
        string seed = GenerateRandomSeed();
        SelectPanelManager.Instance.ChangeCustomSeed(seed);
        GameManager.Instance.DestroyMap(System.Math.Abs(seed.GetHashCode()));
    }

    public bool IsCustomMap()
    {
        return _currentLevelIndex == _mapCount;
    }
}
