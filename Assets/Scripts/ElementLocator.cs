using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class ElementLocator : MonoBehaviour
{
    public Vector3 GetWorldPosition(Vector2Int coordinates) {
        return GetComponent<Grid>().CellToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
    }

    public Vector2Int GetCoordinates(GridElement e) {
        return FindTile(e.transform.position);
    }

    public ISet<GridElement> GetTile(Vector2Int coordinates) {
        return GetElements().Where(e => GetCoordinates(e) == coordinates).ToHashSet();
    }

    private ISet<GridElement> GetElements() {
        return transform.Cast<Transform>().Select(child => child.GetComponent<GridElement>()).Where(e => e != null).ToHashSet();
    }

    private Vector2Int FindTile(Vector3 worldLocation) {
        Vector3Int loc =  GetComponent<Grid>().WorldToCell(worldLocation);
        return new Vector2Int(loc.x, loc.y);
    }
}