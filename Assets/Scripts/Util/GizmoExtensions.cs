using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmoExtensions
{
    public static void DrawArrow(Vector2 from, Vector2 to, float arrowheadSize = 0.2f)
    {
        Gizmos.DrawLine(from, to);

        Vector2 direction = (to - from).normalized;

        // Rotate the direction by 45 degrees to get the arrowhead sides
        Vector2 right = RotateVector(direction, 45f * 3);
        Vector2 left = RotateVector(direction, 45f * -3);

        // Draw the arrowhead
        Gizmos.DrawLine(to, to + right * arrowheadSize); // Right side of arrowhead
        Gizmos.DrawLine(to, to + left * arrowheadSize);  // Left side of arrowhead
    }

    // Helper function to rotate a vector by a given angle
    private static Vector2 RotateVector(Vector2 v, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            cos * v.x - sin * v.y,
            sin * v.x + cos * v.y
        );
    }
}
