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
            Vector2Int newDirection = GetDirection(letter.GridPosition, _startGridPosition);

            ClearSelectionFromDirectionChange();
            _selectionDirection = newDirection;

            // If the new direction is equal to (0,0) then is only selected the first letter.
            if (newDirection == Vector2Int.zero)
            {
                _selectedLetters.Add(letter);
                return;
            }

            Vector2Int currentPosition = _startGridPosition;
            LetterItem currentLetter = _levelGenerator.GetLetterAtGridPosition(currentPosition);

            while (currentLetter != null && currentPosition != letter.GridPosition)
            {
                if (currentLetter != null)
                {
                    _selectedLetters.Add(currentLetter);
                }

                currentPosition += _selectionDirection;
                currentLetter = _levelGenerator.GetLetterAtGridPosition(currentPosition);
            }

            Vector2Int lastLetterDirection = GetDirection(letter.GridPosition, _selectedLetters.Last().GridPosition);
            if (lastLetterDirection == _selectionDirection)
                _selectedLetters.Add(letter);
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
    }

    private Vector2Int GetDirection(Vector2Int pos1, Vector2Int pos2)
    {
        //If is the same position return the zero direction.
        if (pos1 == pos2)
            return Vector2Int.zero;

        Vector2Int direction = Vector2Int.RoundToInt(pos1 - pos2);

        int maximumExponent = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        direction.x = Mathf.CeilToInt(direction.x / maximumExponent);
        direction.y = Mathf.CeilToInt(direction.y / maximumExponent);

        return direction;
    }

    private bool IsDirectionValid(Vector2Int newDirection)
    {
        // Ensure the direction is strictly horizontal, vertical, or diagonal
        bool isHorizontal = newDirection.x != 0 && newDirection.y == 0;
        bool isVertical = newDirection.x == 0 && newDirection.y != 0;
        bool isDiagonal = Mathf.Abs(newDirection.x) == 1 && Mathf.Abs(newDirection.y) == 1;

        return isHorizontal || isVertical || isDiagonal;
    }

    #endregion
}
