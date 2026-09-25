using UnityEngine;

// Goes on the arena prefab and marks where enemies can come from.
// The spawn points sit on the arena floor, so enemies always appear on the AR plane.
public class ArenaLayout : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;

    // random spawn point that isn't right on top of the player,
    // falls back to the furthest one if the player is standing in the middle of everything
    public Transform PickSpawnPoint(Vector3 playerPosition, float minDistance)
    {
        Transform furthest = spawnPoints[0];
        float furthestDistance = 0f;

        int start = Random.Range(0, spawnPoints.Length);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform point = spawnPoints[(start + i) % spawnPoints.Length];
            float distance = FlatDistance(point.position, playerPosition);

            if (distance >= minDistance) return point;

            if (distance > furthestDistance)
            {
                furthest = point;
                furthestDistance = distance;
            }
        }

        return furthest;
    }

    static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }
}
