using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
    public static bool Adjacent(Vector2Int a, Vector2Int b) {
        return new HashSet<Vector2Int>{Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down}
            .Select(v => v + a)
            .Contains(b);
    }

    public static bool IsPath(List<Vector2Int> path) {
        return path.Zip(path.Skip(1), (a, b) => (a, b)).All(pair => Adjacent(pair.a, pair.b));
    }


    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, Func<Vector2Int, bool> isWalkable) {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int?> cameFrom = new Dictionary<Vector2Int, Vector2Int?>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        while (queue.Count > 0) {
            Vector2Int current = queue.Dequeue();

            if (current == end) {
                List<Vector2Int> path = new List<Vector2Int>();
                while (current != start) {
                    path.Add(current);
                    current = cameFrom[current].Value;
                }
                path.Reverse();
                return path;
            }

            foreach (Vector2Int direction in directions) {
                Vector2Int neighbor = current + direction;
                if (isWalkable(neighbor) && !cameFrom.ContainsKey(neighbor)) {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }
        return null; // Return null if no path is found
    }
}
