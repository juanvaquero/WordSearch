using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    public int gridSize = 10;
    public Letter letterPrefab;
    public Transform gridParent;
    public TextMeshProUGUI selectedWordText;
    public TextMeshProUGUI topicTitleText;
    public TextAsset wordThemesFile;
    public int numberOfWordsToFind = 5;
    public bool allowVerticalPlacement = true;
    public bool allowDiagonalPlacement = true;
    public bool allowReverseWords = true;
    public Transform lineRendererParent; // Parent of all line renderers
    public WordSearchConfig config;

    public WordsSearchItem wordsSearchItem;
    public Transform wordsSearchItemsParent;
    private Dictionary<string, WordsSearchItem> wordSearchItems = new Dictionary<string, WordsSearchItem>();

    private List<Theme> wordThemes;
    private Letter[,] grid;
    private string selectedTheme;
    private List<string> currentWordList;
    private List<string> wordsToFind;
    private List<Letter> selectedLetters = new List<Letter>();
    private bool isSelecting = false;
    private Vector2Int startGridPosition;
    private Vector2Int selectionDirection;
    private LineRendererController currentLineRendererController;
    private List<LineRendererController> activeLineRenderers = new List<LineRendererController>();
    private int colorIndex = 0;

    void Start()
    {
        LoadWordThemes();
        if (wordThemes != null && wordThemes.Count > 0)
        {
            GenerateGrid();
            SelectRandomTheme();
            SelectRandomWords();
            PlaceWords();
            FillGrid();
            CreateNewLineRenderer();
            DisplayTopic();
            CreateWordPanels();
        }
        else
        {
            Debug.LogError("No word themes available. Check the wordThemesFile.");
        }
    }

    void LoadWordThemes()
    {
        var json = wordThemesFile.text;
        WordThemesData wordThemesData = JsonUtility.FromJson<WordThemesData>(json);

        if (wordThemesData != null && wordThemesData.themes != null)
        {
            wordThemes = wordThemesData.themes;
        }
        else
        {
            Debug.LogError("Failed to load word themes from JSON.");
        }
    }

    void SelectRandomTheme()
    {
        var themes = new List<string>();
        foreach (var theme in wordThemes)
        {
            themes.Add(theme.name);
        }

        if (themes.Count > 0)
        {
            selectedTheme = themes[Random.Range(0, themes.Count)];
            foreach (var theme in wordThemes)
            {
                if (theme.name == selectedTheme)
                {
                    currentWordList = theme.words.FindAll(word => word.Length <= gridSize && word.Length <= 10).Select(word => word.ToLower()).ToList();
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("No themes available to select.");
        }
    }

    void SelectRandomWords()
    {
        wordsToFind = new List<string>();
        List<int> usedIndices = new List<int>();

        // Sort the word list by length in descending order
        currentWordList = currentWordList.OrderByDescending(word => word.Length).ToList();

        for (int i = 0; i < numberOfWordsToFind; i++)
        {
            if (currentWordList.Count == 0)
            {
                Debug.LogError("Current word list is empty or no words match the criteria.");
                break;
            }

            int index;
            do
            {
                index = Random.Range(0, currentWordList.Count);
            } while (usedIndices.Contains(index) && usedIndices.Count < currentWordList.Count);

            if (!usedIndices.Contains(index))
            {
                usedIndices.Add(index);
                string word = currentWordList[index].ToLower();
                if (!wordsToFind.Contains(word))
                {
                    wordsToFind.Add(word);
                }
            }
        }
    }

    void GenerateGrid()
    {
        gridParent.GetComponent<GridLayoutGroup>().constraintCount = gridSize;

        grid = new Letter[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Letter letter = Instantiate(letterPrefab, gridParent);
                letter.SetLevelGenerator(this);
                letter.Initialize(' ', new Vector2Int(i, j));
                grid[i, j] = letter;
            }
        }
    }

    void PlaceWords()
    {
        for (int i = 0; i < wordsToFind.Count; i++)
        {
            if (!PlaceWordInGrid(wordsToFind[i], allowOverlap: true))
            {
                // If the word couldn't be placed, remove it from wordsToFind and try another word from the list
                string word = wordsToFind[i];
                wordsToFind.Remove(word);
                i--; // Adjust the index to try again
                currentWordList.Remove(word); // Remove the word from the possible words list
                TryPlaceAlternativeWord();
            }
        }
    }

    string ReverseString(string s)
    {
        char[] charArray = s.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }

    bool PlaceWordInGrid(string word, bool allowOverlap = false)
    {
        int attempts = 200;
        while (attempts > 0)
        {
            if (allowReverseWords && Random.value > 0.5f)
                word = ReverseString(word);

            int row = Random.Range(0, gridSize);
            int col = Random.Range(0, gridSize);
            int direction = Random.Range(0, allowDiagonalPlacement ? 4 : (allowVerticalPlacement ? 2 : 1)); // 0 = horizontal, 1 = vertical, 2 = diagonal (down-right), 3 = diagonal (down-left)

            if (allowOverlap ? CanPlaceWordWithOverlap(word, row, col, direction) : CanPlaceWord(word, row, col, direction))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    switch (direction)
                    {
                        case 0: // horizontal
                            grid[row, col + i].Initialize(word[i], new Vector2Int(row, col + i));
                            break;
                        case 1: // vertical
                            grid[row + i, col].Initialize(word[i], new Vector2Int(row + i, col));
                            break;
                        case 2: // diagonal (down-right)
                            grid[row + i, col + i].Initialize(word[i], new Vector2Int(row + i, col + i));
                            break;
                        case 3: // diagonal (down-left)
                            grid[row + i, col - i].Initialize(word[i], new Vector2Int(row + i, col - i));
                            break;
                    }
                }
                return true;
            }
            attempts--;
        }
        return false;
    }

    bool CanPlaceWord(string word, int row, int col, int direction)
    {
        switch (direction)
        {
            case 0: // horizontal
                if (col + word.Length > gridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (grid[row, col + i].letter != ' ' && grid[row, col + i].letter != word[i])
                        return false;
                break;

            case 1: // vertical
                if (row + word.Length > gridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (grid[row + i, col].letter != ' ' && grid[row + i, col].letter != word[i])
                        return false;
                break;

            case 2: // diagonal (down-right)
                if (row + word.Length > gridSize || col + word.Length > gridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (grid[row + i, col + i].letter != ' ' && grid[row + i, col + i].letter != word[i])
                        return false;
                break;

            case 3: // diagonal (down-left)
                if (row + word.Length > gridSize || col - word.Length < 0) return false;
                for (int i = 0; i < word.Length; i++)
                    if (grid[row + i, col - i].letter != ' ' && grid[row + i, col - i].letter != word[i])
                        return false;
                break;
        }
        return true;
    }

    bool CanPlaceWordWithOverlap(string word, int row, int col, int direction)
    {
        switch (direction)
        {
            case 0: // horizontal
                if (col + word.Length > gridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (grid[row, col + i].letter != ' ' && grid[row, col + i].letter != word[i])
                        return false;
                break;

            case 1: // vertical
                if (row + word.Length > gridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (grid[row + i, col].letter != ' ' && grid[row + i, col].letter != word[i])
                        return false;
                break;

            case 2: // diagonal (down-right)
                if (row + word.Length > gridSize || col + word.Length > gridSize) return false;
                for (int i = 0; i < word.Length; i++)
                    if (grid[row + i, col + i].letter != ' ' && grid[row + i, col + i].letter != word[i])
                        return false;
                break;

            case 3: // diagonal (down-left)
                if (row + word.Length > gridSize || col - word.Length < 0) return false;
                for (int i = 0; i < word.Length; i++)
                    if (grid[row + i, col - i].letter != ' ' && grid[row + i, col - i].letter != word[i])
                        return false;
                break;
        }
        return true;
    }

    string TryPlaceAlternativeWord()
    {
        for (int i = 0; i < currentWordList.Count; i++)
        {
            if (!PlaceWordInGrid(currentWordList[i], allowOverlap: true))
            {
                string wordToRemove = currentWordList[i];
                currentWordList.Remove(wordToRemove); // Remove the word from the possible words list
                TryPlaceAlternativeWord();
            }
        }
        return null;
    }

    void FillGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j].letter == ' ')
                {
                    char randomChar = (char)('a' + Random.Range(0, 26));
                    grid[i, j].Initialize(randomChar, new Vector2Int(i, j));
                }
            }
        }
    }

    void DisplayTopic()
    {
        topicTitleText.text = selectedTheme;
    }

    public void StartSelectingLetter(Letter letter)
    {
        isSelecting = true;
        selectedLetters.Clear();
        selectedLetters.Add(letter);
        startGridPosition = letter.gridPosition;
        selectionDirection = Vector2Int.zero;
        UpdateSelectedWordDisplay();
        UpdateLineRenderer();
    }


    public void SelectLetter(Letter letter)
    {
        if (isSelecting)
        {
            Vector2Int newDirection = Vector2Int.RoundToInt(letter.gridPosition - startGridPosition);
            newDirection.x = Mathf.Clamp(newDirection.x, -1, 1);
            newDirection.y = Mathf.Clamp(newDirection.y, -1, 1);

            if (newDirection != selectionDirection && selectionDirection != Vector2Int.zero)
            {
                ClearSelectionFromDirectionChange();
                selectionDirection = Vector2Int.zero;
            }

            selectionDirection = newDirection;
            Vector2Int expectedPosition = startGridPosition + selectionDirection * selectedLetters.Count;

            if (Vector2Int.RoundToInt(letter.gridPosition) == expectedPosition && !selectedLetters.Contains(letter))
            {
                selectedLetters.Add(letter);
                UpdateSelectedWordDisplay();
                UpdateLineRenderer();
            }
        }
    }


    void ClearSelectionFromDirectionChange()
    {
        selectedLetters.Clear();
        selectedLetters.Add(grid[startGridPosition.x, startGridPosition.y]);
        UpdateSelectedWordDisplay();
        UpdateLineRenderer();
    }

    public void FinishSelectingLetter()
    {
        isSelecting = false;
        string formedWord = new string(selectedLetters.Select(l => l.letter).ToArray());
        formedWord = formedWord.ToLower();
        if (wordsToFind.Contains(formedWord))
        {
            Debug.Log("Found word: " + formedWord);
            wordsToFind.Remove(formedWord);
            wordSearchItems[formedWord].MarkAsFound();
            currentLineRendererController = null;
            CreateNewLineRenderer();
            CheckIfLevelCompleted();
        }
        else
        {
            // Clear the selection letters if the word is not found.
            selectedLetters.Clear();
            UpdateLineRenderer();
        }
        UpdateSelectedWordDisplay();
    }

    void UpdateSelectedWordDisplay()
    {
        selectedWordText.text = new string(selectedLetters.Select(l => l.letter).ToArray());
        //TODO Apply a animation
    }

    public List<Letter> GetSelectedLetters()
    {
        return selectedLetters;
    }

    private void CheckIfLevelCompleted()
    {
        // If it's not more words to find, the level is completed.
        if (wordsToFind.Count == 0)
        {
            PopupController.Instance.ShowPopup("Reset Level");
        }
    }

    #region Line Render methods

    void UpdateLineRenderer()
    {
        if (currentLineRendererController != null)
        {
            Vector3[] positions = selectedLetters.Select(letter => letter.transform.position).ToArray();
            currentLineRendererController.SetPositions(positions);
        }
    }

    void CreateNewLineRenderer()
    {
        GameObject lineRendererObj = new GameObject("LineRendererController");
        lineRendererObj.transform.SetParent(lineRendererParent);
        currentLineRendererController = lineRendererObj.AddComponent<LineRendererController>();
        currentLineRendererController.SetColor(GetNextColor());
        activeLineRenderers.Add(currentLineRendererController);
    }

    Color GetNextColor()
    {
        if (config != null && config.colors.Count > 0)
        {
            Color color = config.colors[colorIndex];
            colorIndex = (colorIndex + 1) % config.colors.Count;
            return color;
        }
        return Color.white; // Default color if no config or colors available
    }

    #endregion
    #region Word search panel configuration

    void CreateWordPanels()
    {
        // Clear existing panels
        foreach (Transform child in wordsSearchItemsParent)
        {
            Destroy(child.gameObject);
        }
        wordSearchItems.Clear();

        // Create new panels
        foreach (string word in wordsToFind)
        {
            CreateWordPanel(word);
        }
    }

    void CreateWordPanel(string word)
    {
        WordsSearchItem wordSearchItem = Instantiate(wordsSearchItem, wordsSearchItemsParent);
        wordSearchItem.Initialize(word);
        if (!wordSearchItems.ContainsKey(word))
        {
            wordSearchItems.Add(word, wordSearchItem);
        }
    }

    #endregion
    #region Reset Level

    public void ResetLevel()
    {
        // Destroy all children of gridParent
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // Destroy all active line renderers
        foreach (var lineRenderer in activeLineRenderers)
        {
            Destroy(lineRenderer.gameObject);
        }
        activeLineRenderers.Clear();

        // Clear the list of words to find and selected letters
        wordsToFind.Clear();
        selectedLetters.Clear();

        // Reset the level state
        colorIndex = 0;

        // Generate the grid again
        GenerateGrid();
        SelectRandomTheme();
        SelectRandomWords();
        UpdateSelectedWordDisplay();
        PlaceWords();
        FillGrid();
        CreateNewLineRenderer();
        DisplayTopic();
        CreateWordPanels();
    }


    #endregion

}
