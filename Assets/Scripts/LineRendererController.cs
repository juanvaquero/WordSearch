using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.numCapVertices = lineRenderer.numCornerVertices = 15;
    }

    public void SetColor(Color color)
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = color };
    }

    public void SetPositions(Vector3[] positions)
    {
        if (positions.Length > 0)
        {
            lineRenderer.positionCount = positions.Length;
            for (int i = 0; i < positions.Length; i++)
            {
                lineRenderer.SetPosition(i, positions[i] - Vector3.forward);
            }

            // If it's only one letter selected, create a second position, to create a point in the first letter.
            if (positions.Length == 1)
            {
                lineRenderer.positionCount = positions.Length + 1;
                lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
