using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    #region Private Variables
    private LineRenderer _lineRenderer;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        InitializeLineRenderer();
    }
    #endregion

    #region Public Methods
    public void SetColor(Color color)
    {
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = color };
    }

    public void SetPositions(Vector3[] positions)
    {
        if (positions.Length > 0)
        {
            _lineRenderer.positionCount = positions.Length;
            for (int i = 0; i < positions.Length; i++)
            {
                _lineRenderer.SetPosition(i, positions[i] - Vector3.forward);
            }

            // If it's only one letter selected, create a second position to create a point in the first letter.
            if (positions.Length == 1)
            {
                _lineRenderer.positionCount = positions.Length + 1;
                _lineRenderer.SetPosition(1, _lineRenderer.GetPosition(0));
            }
        }
        else
        {
            _lineRenderer.positionCount = 0;
        }
    }
    #endregion

    #region Private Methods
    private void InitializeLineRenderer()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _lineRenderer.startWidth = 0.5f;
        _lineRenderer.endWidth = 0.5f;
        _lineRenderer.numCapVertices = _lineRenderer.numCornerVertices = 15;
    }
    #endregion
}
