using System;
using UnityEngine;
[Serializable]
public class IslandData
{
    public IslandController IslandPrefab;
    public Vector3 Position;
    public Vector3 Rotation;
    public IslandData(IslandController islandPrefab, Vector3 position, Vector3 rotation)
    {
        IslandPrefab = islandPrefab;
        Position = position;
        Rotation = rotation;
    }
}
