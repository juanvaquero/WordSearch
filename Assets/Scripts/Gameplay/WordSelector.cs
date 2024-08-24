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
        if (!_isSelecting) return;

        Vector2Int newDirection = GetDirection(letter.GridPosition, _startGridPosition);

        // If the new direction is (0,0), only the first letter is selected
        if (newDirection == Vector2Int.zero)
        {
            ClearSelectionFromDirectionChange();
            _selectionDirection = Vector2Int.zero;
            _selectedLetters.Add(letter);
            return;
        }

        // Generate a list of possible selected letters based on the new direction
        List<LetterItem> possibleSelectedLetters = GetLettersInDirection(_startGridPosition, letter.GridPosition, newDirection);

        // If no valid selection is found, exit
        if (possibleSelectedLetters.Count == 0) return;

        // Update selection direction and selected letters
        ClearSelectionFromDirectionChange();
        _selectionDirection = newDirection;
        _selectedLetters = possibleSelectedLetters;

        // Add the current letter if it aligns with the current selection direction
        if (GetDirection(letter.GridPosition, _selectedLetters.Last().GridPosition) == _selectionDirection)
        {
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

    private List<LetterItem> GetLettersInDirection(Vector2Int start, Vector2Int end, Vector2Int direction)
    {
        List<LetterItem> letters = new List<LetterItem>();
        Vector2Int currentPosition = start;
        LetterItem currentLetter;

        while (currentPosition != end)
        {
            currentLetter = _levelGenerator.GetLetterAtGridPosition(currentPosition);
            if (currentLetter == null) return new List<LetterItem>();

            letters.Add(currentLetter);
            currentPosition += direction;
        }

        return letters;
    }

    #endregion
}
