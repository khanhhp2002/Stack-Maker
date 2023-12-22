using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    [SerializeField] private IslandOutputDirection _currentOutputDirection;
    [SerializeField, Range(10, 20)] private int _mapLength;
    private List<IslandData> _randomMapData = new List<IslandData>();
    private List<IslandController> _availableIsland = new List<IslandController>();
    private Vector3 _currentSpawnPosition;
    private Vector3 _currentSpawnRotation;
    private IslandController _finishIsland;
    private IslandData _startIsland;

    void Start()
    {
        Init();
        LoadAllAvaiableIsland();
    }

    public void Init()
    {
        _randomMapData.Clear();
        _randomMapData.Add(_startIsland);
        _currentSpawnPosition = new Vector3(0, 0, 4);
        _currentSpawnRotation = Vector3.zero;
        _currentOutputDirection = IslandOutputDirection.Front;
    }

    void LoadAllAvaiableIsland()
    {
        IslandController[] allIsland = Resources.LoadAll<IslandController>("Islands");
        foreach (IslandController island in allIsland)
        {
            if (island.GetIslandType() == IslandType.Start)
            {
                _startIsland = new IslandData(island, Vector3.zero, Vector3.zero);
            }
            else if (island.GetIslandType() != IslandType.Finish)
                _availableIsland.Add(island);
            else
                _finishIsland = island;
        }
    }

    void SetIsland(IslandController island, IslandValidStats islandValidStats)
    {
        // align spawn point
        if (_currentOutputDirection == IslandOutputDirection.Front)
        {
            _currentSpawnPosition = new Vector3(_currentSpawnPosition.x - islandValidStats.OffsetIn.x, _currentSpawnPosition.y, _currentSpawnPosition.z);
        }
        else if (_currentOutputDirection == IslandOutputDirection.Left)
        {
            _currentSpawnPosition = new Vector3(_currentSpawnPosition.x, _currentSpawnPosition.y, _currentSpawnPosition.z - islandValidStats.OffsetIn.x);
        }
        else if (_currentOutputDirection == IslandOutputDirection.Right)
        {
            _currentSpawnPosition = new Vector3(_currentSpawnPosition.x, _currentSpawnPosition.y, _currentSpawnPosition.z + islandValidStats.OffsetIn.x);
        }

        Debug.Log("Align point: " + _currentSpawnPosition);

        // create island
        IslandData tmp = new IslandData(island, _currentSpawnPosition, _currentSpawnRotation + islandValidStats.ValidRotation);
        _randomMapData.Add(tmp);

        // next spawn point
        GetCurrentDirection(islandValidStats.ValidOutputDirection);
        Debug.Log("New Direction: " + _currentOutputDirection.ToString());
        if (_currentOutputDirection == IslandOutputDirection.Front)
        {
            _currentSpawnRotation = Vector3.zero;
            _currentSpawnPosition += new Vector3(islandValidStats.OffsetOut.x, 0, 5);
        }
        else if (_currentOutputDirection == IslandOutputDirection.Left)
        {
            _currentSpawnRotation = new Vector3(0, -90, 0);
            _currentSpawnPosition += new Vector3(-5, 0, islandValidStats.OffsetOut.x);
        }
        else if (_currentOutputDirection == IslandOutputDirection.Right)
        {
            _currentSpawnRotation = new Vector3(0, 90, 0);
            _currentSpawnPosition += new Vector3(5, 0, -islandValidStats.OffsetOut.x);
        }

        Debug.Log("Next point: " + _currentSpawnPosition);
    }

    void GetCurrentDirection(IslandOutputDirection outputDirection)
    {
        if (outputDirection == IslandOutputDirection.Front) return;
        switch (_currentOutputDirection)
        {
            case IslandOutputDirection.Front:
                if (outputDirection == IslandOutputDirection.Left)
                    _currentOutputDirection = IslandOutputDirection.Left;
                else if (outputDirection == IslandOutputDirection.Right)
                    _currentOutputDirection = IslandOutputDirection.Right;
                break;
            case IslandOutputDirection.Left:
                if (outputDirection == IslandOutputDirection.Front)
                    _currentOutputDirection = IslandOutputDirection.Left;
                else if (outputDirection == IslandOutputDirection.Right)
                    _currentOutputDirection = IslandOutputDirection.Front;
                break;
            case IslandOutputDirection.Right:
                if (outputDirection == IslandOutputDirection.Front)
                    _currentOutputDirection = IslandOutputDirection.Right;
                else if (outputDirection == IslandOutputDirection.Left)
                    _currentOutputDirection = IslandOutputDirection.Front;
                break;
        }
    }

    public void GenerateMap(int seed)
    {
        StartCoroutine(GenerateMapAsync(seed));
    }

    public IEnumerator GenerateMapAsync(int seed)
    {
        // Reset map
        Init();

        // Generate map
        System.Random random = new System.Random(seed);
        for (int i = 0; i < _mapLength; i++)
        {
            // Get random island
            IslandController tmp = _availableIsland[random.Next(0, _availableIsland.Count - 1)];

            Debug.Log(tmp.gameObject.name);

            // Choose next direction
            IslandOutputDirection needDirection;
            if (_currentOutputDirection == IslandOutputDirection.Left)
                needDirection = IslandOutputDirection.Right;
            else if (_currentOutputDirection == IslandOutputDirection.Right)
                needDirection = IslandOutputDirection.Left;
            else
                needDirection = random.Next(2) == 1 ? IslandOutputDirection.Left : IslandOutputDirection.Right;

            SetIsland(tmp, tmp.GetIslandValidStats(needDirection));
            yield return null;
        }

        // Add finish island
        _randomMapData.Add(new IslandData(_finishIsland, _currentSpawnPosition, _currentSpawnRotation + _finishIsland.GetIslandValidStats(IslandOutputDirection.Front).ValidRotation));
        yield return null;

        // Load map
        StartCoroutine(LoadCustomMapAsync());
    }

    IEnumerator LoadCustomMapAsync()
    {
        foreach (IslandData islandData in _randomMapData)
        {
            IslandController tmp = Instantiate(islandData.IslandPrefab,
                        islandData.Position,
                        Quaternion.Euler(islandData.Rotation),
                        GameManager.Instance.MapLoadPosition);
            tmp.transform.SetParent(GameManager.Instance.MapLoadPosition);
            yield return null;
        }
        SelectPanelManager.Instance.CanClick = true;
    }

    public List<IslandData> GetLastRandomMapData()
    {
        return _randomMapData;
    }
}
