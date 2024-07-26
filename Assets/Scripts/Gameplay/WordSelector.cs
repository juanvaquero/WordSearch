using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordSelector : MonoBehaviour
{
    #region Private Variables
    private List<LetterItem> _selectedLetters = new List<LetterItem>();
    private Vector2Int _startGridPosition;
    private Vector2Int _selectionDirection;
    private bool _isSelecting = false;
    private LevelGenerator _levelGenerator;
    #endregion

    #region Public Properties
    public List<LetterItem> SelectedLetters => _selectedLetters;
    #endregion

    #region Public Methods
    public void Initialize(LevelGenerator levelGenerator)
    {
        _levelGenerator = levelGenerator;
    }

    public void StartSelectingLetter(LetterItem letter)
    {
        _isSelecting = true;
        _selectedLetters.Clear();
        _selectedLetters.Add(letter);
        _startGridPosition = letter.GridPosition;
        _selectionDirection = Vector2Int.zero;
    }

    public void SelectLetter(LetterItem letter)
    {
        if (_isSelecting)
        {
            Vector2Int newDirection = Vector2Int.RoundToInt(letter.GridPosition - _startGridPosition);
            newDirection.x = Mathf.Clamp(newDirection.x, -1, 1);
            newDirection.y = Mathf.Clamp(newDirection.y, -1, 1);

            if (newDirection != _selectionDirection && _selectionDirection != Vector2Int.zero)
            {
                ClearSelectionFromDirectionChange();
                _selectionDirection = Vector2Int.zero;
            }

            _selectionDirection = newDirection;
            Vector2Int expectedPosition = _startGridPosition + _selectionDirection * _selectedLetters.Count;

            if (Vector2Int.RoundToInt(letter.GridPosition) == expectedPosition && !_selectedLetters.Contains(letter))
            {
                _selectedLetters.Add(letter);
            }
        }
    }

    public void FinishSelectingLetter()
    {
        _isSelecting = false;
    }

    public void ClearSelection()
    {
        _selectedLetters.Clear();
    }
    #endregion

    #region Private Methods
    private void ClearSelectionFromDirectionChange()
    {
        _selectedLetters.Clear();
        _selectedLetters.Add(_levelGenerator.Grid[_startGridPosition.x, _startGridPosition.y]);
    }
    #endregion
}
