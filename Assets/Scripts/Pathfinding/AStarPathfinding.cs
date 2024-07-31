using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding
{
    public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int target, StageManager stageManager)
    {
        List<Vector3Int> openSet = new List<Vector3Int> { start };
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>
        {
            [start] = 0
        };
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>
        {
            [start] = Heuristic(start, target)
        };

        //Debug.Log($"Starting pathfinding from {start} to {target}");

        while (openSet.Count > 0)
        {
            Vector3Int current = GetLowestFScore(openSet, fScore);

            //Debug.Log($"Current tile: {current}");

            if (current == target)
            {
                //Debug.Log("Target reached. Reconstructing path.");
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector3Int neighbor in GetNeighbors(current, stageManager))
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeGScore = gScore[current] + 1; // Cost from start to neighbor through current

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                // This path is the best until now. Record it!
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, target);
            }
        }

        //Debug.Log("No path found.");
        return null; // No path found
    }

    private static float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static Vector3Int GetLowestFScore(List<Vector3Int> openSet, Dictionary<Vector3Int, float> fScore)
    {
        float lowestScore = float.MaxValue;
        Vector3Int lowestTile = openSet[0];

        foreach (Vector3Int tile in openSet)
        {
            if (fScore.TryGetValue(tile, out float score) && score < lowestScore)
            {
                lowestScore = score;
                lowestTile = tile;
            }
        }

        return lowestTile;
    }

    private static List<Vector3Int> GetNeighbors(Vector3Int tile, StageManager stageManager)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            tile + Vector3Int.up,
            tile + Vector3Int.down,
            tile + Vector3Int.left,
            tile + Vector3Int.right
        };

        neighbors.RemoveAll(n => !stageManager.CheckValidTile(n));
        return neighbors;
    }

    private static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> path = new List<Vector3Int> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return path;
    }
}
