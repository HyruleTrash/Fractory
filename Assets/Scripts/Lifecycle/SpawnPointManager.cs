using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour {
    public List<SpawnPoint> spawnPointsReached = new List<SpawnPoint>();
    public SpawnPoint currentSpawnPoint;

    public void SetSpawnPoint(SpawnPoint spawnPoint) {
        if (spawnPointsReached.Contains(spawnPoint)) {
            return;
        }
        currentSpawnPoint = spawnPoint;
        spawnPointsReached.Add(spawnPoint);
    }
}