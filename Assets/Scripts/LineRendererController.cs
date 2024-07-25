using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private LevelGenerator levelGenerator;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.numCapVertices = lineRenderer.numCornerVertices = 15;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f) };
    }

    public void SetLevelGenerator(LevelGenerator generator)
    {
        levelGenerator = generator;
    }

    public void UpdateLineRenderer()
    {
        if (levelGenerator != null && levelGenerator.GetSelectedLetters().Count > 0)
        {
            var selectedLetters = levelGenerator.GetSelectedLetters();
            lineRenderer.positionCount = selectedLetters.Count;

            for (int i = 0; i < selectedLetters.Count; i++)
            {
                lineRenderer.SetPosition(i, selectedLetters[i].transform.position - Vector3.forward);
            }

            // If it's only one letter selected, create a second position, to create a point in the first letter.
            if (selectedLetters.Count == 1)
            {
                lineRenderer.positionCount = selectedLetters.Count + 1;
                lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
