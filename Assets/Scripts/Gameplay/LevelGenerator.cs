using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{

    #region Public Variables

    public LetterItem[,] Grid
    {
        get { return _grid; }
    }

    #endregion

    #region Serialized Fields
    [SerializeField] private LetterItem _letterPrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private TextMeshProUGUI _selectedWordText;
    [SerializeField] private TextMeshProUGUI _topicTitleText;
    [SerializeField] private TextAsset _wordThemesFile;
    [SerializeField] private Transform _lineRendererParent;
    [SerializeField] private WordSearchConfig _config;
    [SerializeField] private WordsSearchItem _wordsSearchItemPrefab;
    [SerializeField] private Transform _wordsSearchItemsParent;
    #endregion

    #region Private Variables
    private List<Theme> _wordThemes;
    private LetterItem[,] _grid;
    private string _selectedTheme;
    private List<string> _currentWordList;
    private List<string> _wordsToFind;

    private WordSelector _wordSelector;
    private LineRendererManager _lineRendererManager;
    private WordPanelManager _wordPanelManager;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _wordSelector = gameObject.AddComponent<WordSelector>();
        _lineRendererManager = gameObject.AddComponent<LineRendererManager>();
        _wordPanelManager = gameObject.AddComponent<WordPanelManager>();
    }

    private void Start()
    {
        _wordSelector.Initialize(this);
        _lineRendererManager.Initialize(_lineRendererParent, _config);
        _wordPanelManager.Initialize(_wordsSearchItemPrefab, _wordsSearchItemsParent);
        LoadWordThemes();
        if (_wordThemes != null && _wordThemes.Count > 0)
        {
            InitializeLevel();
        }
        else
        {
            Debug.LogError("No word themes available. Check the wordThemesFile.");
        }
    }
    #endregion

    #region Private Methods
    private void LoadWordThemes()
    {
        var json = _wordThemesFile.text;
        WordThemesData wordThemesData = JsonUtility.FromJson<WordThemesData>(json);

        if (wordThemesData != null && wordThemesData.themes != null)
        {
            _wordThemes = wordThemesData.themes;
        }
        else
        {
            Debug.LogError("Failed to load word themes from JSON.");
        }
    }

    private void InitializeLevel()
    {
        GenerateGrid();
        SelectRandomTheme();
        SelectRandomWords();
        UpdateSelectedWordDisplay();
        PlaceWords();
        FillGrid();
        DisplayTopic();
        _lineRendererManager.CreateNewLineRenderer();
        _wordPanelManager.CreateWordPanels(_wordsToFind);
    }

    private void SelectRandomTheme()
    {
        var themes = _wordThemes.Select(theme => theme.name).ToList();

        if (themes.Count > 0)
        {
            _selectedTheme = themes[Random.Range(0, themes.Count)];
            _currentWordList = _wordThemes.First(theme => theme.name == _selectedTheme).words
                .Where(word => word.Length <= _config.GridSize && word.Length <= 10)
                .Select(word => word.ToLower())
                .ToList();
        }
        else
        {
            Debug.LogError("No themes available to select.");
        }
    }

    private void SelectRandomWords()
    {
        _wordsToFind = new List<string>();
        _currentWordList = _currentWordList.OrderByDescending(word => word.Length).ToList();

        while (_wordsToFind.Count < _config.NumberOfWordsToFind && _currentWordList.Count > 0)
        {
            string word = _currentWordList[Random.Range(0, _currentWordList.Count)];
            if (!_wordsToFind.Contains(word))
            {
                _wordsToFind.Add(word);
            }
            _currentWordList.Remove(word);
        }
    }

    private void GenerateGrid()
    {
        GridLayoutGroup grid = _gridParent.GetComponent<GridLayoutGroup>();
        grid.constraintCount = _config.GridSize;

        int cellSize = Mathf.RoundToInt((Screen.width - _config.WidthOffsetScreen) / _config.GridSize);
        grid.cellSize = Vector2Int.one * cellSize;

        _grid = new LetterItem[_config.GridSize, _config.GridSize];
        for (int i = 0; i < _config.GridSize; i++)
        {
            for (int j = 0; j < _config.GridSize; j++)
            {
                LetterItem letter = Instantiate(_letterPrefab, _gridParent);
                letter.SetLevelGenerator(this);
                letter.Initialize('A', new Vector2Int(i, j));
                _grid[i, j] = letter;
            }
        }
    }

    private void PlaceWords()
    {
        foreach (var word in _wordsToFind.ToList())
        {
            if (!PlaceWordInGrid(word, allowOverlap: true))
            {
                _wordsToFind.Remove(word);
                TryPlaceAlternativeWord();
            }
        }
    }

    private bool PlaceWordInGrid(string word, bool allowOverlap = false)
    {
        int attempts = 200;
        while (attempts > 0)
        {
            if (_config.AllowReverseWords && Random.value > 0.5f)
                word = ReverseString(word);

            int row = Random.Range(0, _config.GridSize);
            int col = Random.Range(0, _config.GridSize);
            int direction = Random.Range(0, _config.AllowDiagonalPlacement ? 4 : (_config.AllowVerticalPlacement ? 2 : 1)); // 0 = horizontal, 1 = vertical, 2 = diagonal (down-right), 3 = diagonal (down-left)

            if (CanPlaceWord(word, row, col, direction))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    switch (direction)
                    {
                        case 0: // horizontal
                            _grid[row, col + i].Initialize(word[i], new Vector2Int(row, col + i));
                            break;
                        case 1: // vertical
                            _grid[row + i, col].Initialize(word[i], new Vector2Int(row + i, col));
                            break;
                        case 2: // diagonal (down-right)
                            _grid[row + i, col + i].Initialize(word[i], new Vector2Int(row + i, col + i));
                            break;
                        case 3: // diagonal (down-left)
                            _grid[row + i, col - i].Initialize(word[i], new Vector2Int(row + i, col - i));
                            break;
                    }
                }
                return true;
            }
            attempts--;
        }
        return false;
    }

    private bool CanPlaceWord(string word, int row, int col, int direction)
    {
        switch (direction)
        {
            case 0: // horizontal
                if (col + word.Length > _config.GridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (_grid[row, col + i].Letter != ' ' && _grid[row, col + i].Letter != word[i])
                        return false;
                break;

            case 1: // vertical
                if (row + word.Length > _config.GridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (_grid[row + i, col].Letter != ' ' && _grid[row + i, col].Letter != word[i])
                        return false;
                break;

            case 2: // diagonal (down-right)
                if (row + word.Length > _config.GridSize || col + word.Length > _config.GridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (_grid[row + i, col + i].Letter != ' ' && _grid[row + i, col + i].Letter != word[i])
                        return false;
                break;

            case 3: // diagonal (down-left)
                if (row + word.Length > _config.GridSize || col - word.Length < 0) return false;
                for (int i = 0; i < word.Length; i++)
                    if (_grid[row + i, col - i].Letter != ' ' && _grid[row + i, col - i].Letter != word[i])
                        return false;
                break;
        }
        return true;
    }


    private void TryPlaceAlternativeWord()
    {
        foreach (var word in _currentWordList.ToList())
        {
            if (!PlaceWordInGrid(word, true))
            {
                _currentWordList.Remove(word); // Remove the word from the possible words list
            }
        }
    }

    private void FillGrid()
    {
        for (int i = 0; i < _config.GridSize; i++)
        {
            for (int j = 0; j < _config.GridSize; j++)
            {
                if (_grid[i, j].Letter == ' ')
                {
                    char randomChar = (char)('a' + Random.Range(0, 26));
                    _grid[i, j].Initialize(randomChar, new Vector2Int(i, j));
                }
            }
        }
    }

    private void DisplayTopic()
    {
        _topicTitleText.text = _selectedTheme;
    }

    private void UpdateSelectedWordDisplay()
    {
        _selectedWordText.text = new string(_wordSelector.SelectedLetters.Select(l => l.Letter).ToArray());

        // Show/Hide the selected word depending on the is any letter selected or not.
        bool activeSelectedWord = _selectedWordText.text != string.Empty;
        _selectedWordText.transform.parent.gameObject.SetActive(activeSelectedWord);
    }

    private void CheckIfLevelCompleted()
    {
        if (_wordsToFind.Count == 0)
        {
            PopupController.Instance.ShowPopup("Reset Level");
        }
    }

    /// <summary>
    /// Reverses the given string.
    /// </summary>
    /// <param name="word">The word to be reversed.</param>
    /// <returns>The reversed string.</returns>
    private string ReverseString(string word)
    {
        char[] charArray = word.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }

    #endregion

    #region Public Methods
    public void StartSelectingLetter(LetterItem letter)
    {
        _wordSelector.StartSelectingLetter(letter);
        UpdateSelectedWordDisplay();
        _lineRendererManager.UpdateLineRenderer(_wordSelector.SelectedLetters);
    }

    public void SelectLetter(LetterItem letter)
    {
        _wordSelector.SelectLetter(letter);
        UpdateSelectedWordDisplay();
        _lineRendererManager.UpdateLineRenderer(_wordSelector.SelectedLetters);
    }

    public void FinishSelectingLetter()
    {
        _wordSelector.FinishSelectingLetter();
        string formedWord = new string(_wordSelector.SelectedLetters.Select(l => l.Letter).ToArray());
        formedWord = formedWord.ToLower();
        if (_wordsToFind.Contains(formedWord))
        {
            Debug.Log("Found word: " + formedWord);
            _wordsToFind.Remove(formedWord);
            _wordPanelManager.MarkWordAsFound(formedWord);
            _lineRendererManager.CreateNewLineRenderer();
            CheckIfLevelCompleted();
        }
        else
        {
            _wordSelector.ClearSelection();
            _lineRendererManager.UpdateLineRenderer(_wordSelector.SelectedLetters);
        }
        UpdateSelectedWordDisplay();
    }

    public void ResetLevel()
    {
        foreach (Transform child in _gridParent)
        {
            Destroy(child.gameObject);
        }

        _lineRendererManager.ClearLineRenderers();

        _wordsToFind.Clear();
        _wordSelector.ClearSelection();

        InitializeLevel();
    }
    #endregion
}
