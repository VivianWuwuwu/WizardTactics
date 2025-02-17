using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathDisplay : MonoBehaviour
{
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float arrowSize = 0.2f;
    [SerializeField] private List<Vector2Int> serializedPath;

    public Board board;
    public Color color = Color.white;

    private LineRenderer lineRenderer;
    private List<Vector2Int> path = new List<Vector2Int>();

    public List<Vector2Int> Path
    {
        get => path;
        set
        {
            path = value;
            UpdatePathDisplay();
        }
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private void UpdatePathDisplay()
    {
        if (board == null || path == null || path.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // Convert tile positions to world positions
        List<Vector3> worldPositions = new List<Vector3>();
        foreach (var tile in path)
        {
            worldPositions.Add(board.GetWorldPosition(tile));
        }

        // Add arrowhead at the end
        if (worldPositions.Count > 1)
        {
            AddArrowToPath(worldPositions);
        }

        // Update LineRenderer positions
        lineRenderer.positionCount = worldPositions.Count;
        lineRenderer.SetPositions(worldPositions.ToArray());
    }

    private void AddArrowToPath(List<Vector3> worldPositions)
    {
        Vector3 end = worldPositions[worldPositions.Count - 1];
        Vector3 prev = worldPositions[worldPositions.Count - 2];

        Vector3 direction = (end - prev).normalized;
        Vector3 right = Quaternion.Euler(0, 0, 30) * -direction * arrowSize;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -direction * arrowSize;

        worldPositions.Add(end + right);
        worldPositions.Add(end);  // Ensure the arrow joins at the tip
        worldPositions.Add(end + left);
    }

    private void OnValidate()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
        UpdatePathDisplay();
        Path = serializedPath;
    }
}
