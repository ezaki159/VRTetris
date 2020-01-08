using System.Linq;
using UnityEngine;

public static class PieceOrientation
{
    private static readonly Vector3[] Directions;
    private static readonly float[] Angles = {0.0f, 90.0f, 180.0f, 270.0f, 360.0f};

    static PieceOrientation()
    {
        Directions = (from x in Angles from y in Angles from z in Angles select new Vector3(x, y, z)).ToArray();
    }

    public static Vector3 ClosestValidOrientation(Quaternion rotation) {
        var rotationEuler = new Vector3(rotation.eulerAngles.x % 360,
                                        rotation.eulerAngles.y % 360,
                                        rotation.eulerAngles.z % 360);
        var minDistance = float.PositiveInfinity;
        var finalDirection = Vector3.zero;
        
        foreach (var direction in Directions)
        {
            var distance = Vector3.Distance(direction, rotationEuler);
            if (distance < minDistance)
            {
                minDistance = distance;
                finalDirection = direction;
            }
        }
        return finalDirection;
    }
}
