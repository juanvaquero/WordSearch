using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineRendererManager : MonoBehaviour
{
    #region Private Variables
    private Transform _lineRendererParent;
    private WordSearchConfig _config;
    private LineRendererController _currentLineRendererController;
    private List<LineRendererController> _activeLineRenderers = new List<LineRendererController>();
    private int _colorIndex = 0;
    #endregion

    #region Public Methods
    public void Initialize(Transform lineRendererParent, WordSearchConfig config)
    {
        _lineRendererParent = lineRendererParent;
        _config = config;
    }

    public void UpdateLineRenderer(List<LetterItem> selectedLetters)
    {
        if (_currentLineRendererController != null)
        {
            Vector3[] positions = selectedLetters.Select(letter => letter.transform.position).ToArray();
            _currentLineRendererController.SetPositions(positions);
        }
    }

    public void CreateNewLineRenderer()
    {
        GameObject lineRendererObj = new GameObject("LineRendererController");
        lineRendererObj.transform.SetParent(_lineRendererParent);
        _currentLineRendererController = lineRendererObj.AddComponent<LineRendererController>();
        _currentLineRendererController.SetColor(GetNextColor());
        _activeLineRenderers.Add(_currentLineRendererController);
    }

    public void ClearLineRenderers()
    {
        foreach (var lineRenderer in _activeLineRenderers)
        {
            Destroy(lineRenderer.gameObject);
        }
        _activeLineRenderers.Clear();
    }
    #endregion

    #region Private Methods
    private Color GetNextColor()
    {
        if (_config != null && _config.Colors.Count > 0)
        {
            Color color = _config.Colors[_colorIndex];
            _colorIndex = (_colorIndex + 1) % _config.Colors.Count;
            return color;
        }
        return Color.white; // Default color if no config or colors available
    }
    #endregion
}
