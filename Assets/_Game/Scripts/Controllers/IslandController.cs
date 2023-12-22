using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class IslandController : MonoBehaviour
{
    [SerializeField] private IslandType _islandType;
    [SerializeField] private List<IslandValidStats> _validStats;

    void Start()
    {
        if (_islandType == IslandType.Start)
            GameManager.Instance._startPosition = this.transform.position + Vector3.up;
        else if (_islandType == IslandType.Finish)
            GameManager.Instance._finishedPosition = this.transform.position + Vector3.up;
    }
    public IslandType GetIslandType()
    {
        return _islandType;
    }

    public IslandValidStats GetIslandValidStats(IslandOutputDirection islandOutputDirection)
    {
        if (_validStats.Count == 1) return _validStats[0];
        foreach (IslandValidStats islandValidStats in _validStats)
        {
            if (islandValidStats.ValidOutputDirection == islandOutputDirection)
                return islandValidStats;
        }
        System.Random random = new Random(System.DateTime.Now.Millisecond);
        return random.Next(2) == 1 ? _validStats[0] : _validStats[1];
    }
}
