using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public Transform MapLoadPosition;
    [SerializeField] private PlayerController _playerController;
    public Vector3 _finishedPosition;
    public Vector3 _startPosition;
    public (string, string, int) LevelData { get; set; }
    private Coroutine _currentLoadMapCo;

    public void LoadMap(MapOS mapData = null)
    {
        if (LevelManager.Instance.IsCustomMap())
        {
            MapOS mapOS = ScriptableObject.CreateInstance<MapOS>();
            mapOS.MapData = MapGenerator.Instance.GetLastRandomMapData();
            DestroyMap(mapOS);
        }
        else
        {
            if (mapData is null)
            {
                mapData = LevelManager.Instance.LoadMapByIndex(LevelData.Item3);
            }
            if (MapLoadPosition.childCount > 0)
            {
                DestroyMap(mapData);
            }
            else
            {
                StartCoroutine(LoadMapAsync(mapData));
            }
        }
    }

    IEnumerator LoadMapAsync(MapOS mapData)
    {
        foreach (IslandData islandData in mapData.MapData)
        {
            IslandController tmp = Instantiate(islandData.IslandPrefab,
                        islandData.Position,
                        Quaternion.Euler(islandData.Rotation),
                        MapLoadPosition);
            tmp.transform.SetParent(MapLoadPosition);
            yield return null;
        }
        SelectPanelManager.Instance.CanClick = true;
    }

    void ReloadMap()
    {

    }

    public void DestroyMap(MapOS mapData = null)
    {
        if (_currentLoadMapCo is not null) StopCoroutine(_currentLoadMapCo);
        StartCoroutine(DestroyMapAsync(mapData));
    }

    public void DestroyMap(int seed)
    {
        if (_currentLoadMapCo is not null) StopCoroutine(_currentLoadMapCo);
        StartCoroutine(DestroyMapAsync(seed));
    }
    IEnumerator DestroyMapAsync(MapOS mapData)
    {
        for (int i = MapLoadPosition.childCount - 1; i >= 0; i--)
        {
            Destroy(MapLoadPosition.GetChild(i).gameObject);
            yield return null;
        }
        if (mapData is not null)
            _currentLoadMapCo = StartCoroutine(LoadMapAsync(mapData));
    }

    IEnumerator DestroyMapAsync(int seed)
    {
        for (int i = MapLoadPosition.childCount - 1; i >= 0; i--)
        {
            Destroy(MapLoadPosition.GetChild(i).gameObject);
            yield return null;
        }
        _currentLoadMapCo = StartCoroutine(MapGenerator.Instance.GenerateMapAsync(seed));
        //MapGenerator.Instance.GenerateMap(seed);
    }

    public void SpawnPlayer()
    {
        //_currentPlayer = Instantiate(_playerController, _startPosition, Quaternion.identity);
        RegisterPlayerForCamera(Instantiate(_playerController, _startPosition, Quaternion.identity));
    }

    public void RespawnPlayer(float time = 1f)
    {
        Invoke(nameof(SpawnPlayer), time);
    }

    public void DestroyPlayer(GameObject player, float time)
    {
        StartCoroutine(DestroyPlayerAsync(player, time));
    }

    IEnumerator DestroyPlayerAsync(GameObject player, float time)
    {
        yield return new WaitForSeconds(time);
        RegisterPlayerForCamera();
        yield return null;
        Destroy(player);
        UIManager.Instance.OnVictory();
    }

    public void RegisterPlayerForCamera(PlayerController _playerController = null)
    {
        CameraController.Instance.SetTarget(_playerController);
    }
}
