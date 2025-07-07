using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer), typeof(EdgeCollider2D))]
public class SplineEdgeCollider2D : MonoBehaviour
{
    [Tooltip("How many points to sample along the spline")]
    public int resolution = 50;

    void Start()
    {
        var spline = GetComponent<SplineContainer>().Spline;
        var edgeCollider = GetComponent<EdgeCollider2D>();
        var lineRenderer = GetComponent<LineRenderer>();

        Vector2[] points = new Vector2[resolution];
        lineRenderer.positionCount = resolution;
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            var position = spline.EvaluatePosition(t);
            points[i] = new Vector2(position.x, position.y);
            lineRenderer.SetPosition(i, transform.position + (Vector3)points[i]);
        }

        edgeCollider.points = points;

    }
}
