using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class LevelGenerator : MonoBehaviour
{
    public int gridSize = 10;
    public Button letterButtonPrefab;
    public Transform gridParent;
    public TextMeshProUGUI selectedWordText;
    public TextMeshProUGUI topicTitleText;
    public TextMeshProUGUI wordsToFindText;
    public TextAsset wordThemesFile;
    public int numberOfWordsToFind = 5;
    private List<Theme> wordThemes;
    private char[,] grid;
    private string selectedTheme;
    private List<string> currentWordList;
    private List<string> wordsToFind;

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
            DisplayTopicAndWords();
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
                    currentWordList = theme.words.FindAll(word => word.Length <= gridSize && word.Length <= 10);
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
                wordsToFind.Add(currentWordList[index]);
            }
        }
    }

    void GenerateGrid()
    {
        grid = new char[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Button button = Instantiate<Button>(letterButtonPrefab, gridParent);
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                button.GetComponent<Button>().onClick.AddListener(() => OnLetterButtonClick(button));
                grid[i, j] = ' ';
            }
        }
    }

    void PlaceWords()
    {
        foreach (string word in wordsToFind)
        {
            PlaceWordInGrid(word.ToUpper());
        }
    }

    void PlaceWordInGrid(string word)
    {
        int attempts = 100;
        while (attempts > 0)
        {
            int row = Random.Range(0, gridSize);
            int col = Random.Range(0, gridSize);
            int direction = Random.Range(0, 2); // 0 = horizontal, 1 = vertical

            if (CanPlaceWord(word, row, col, direction))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (direction == 0)
                    {
                        grid[row, col + i] = word[i];
                    }
                    else
                    {
                        grid[row + i, col] = word[i];
                    }
                }
                break;
            }
            attempts--;
        }
    }

    bool CanPlaceWord(string word, int row, int col, int direction)
    {
        if (direction == 0 && col + word.Length > gridSize) return false;
        if (direction == 1 && row + word.Length > gridSize) return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (direction == 0)
            {
                if (grid[row, col + i] != ' ' && grid[row, col + i] != word[i])
                {
                    return false;
                }
            }
            else
            {
                if (grid[row + i, col] != ' ' && grid[row + i, col] != word[i])
                {
                    return false;
                }
            }
        }
        return true;
    }

    void FillGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j] == ' ')
                {
                    grid[i, j] = (char)('A' + Random.Range(0, 26));
                }
                gridParent.GetChild(i * gridSize + j).GetComponentInChildren<TextMeshProUGUI>().text = grid[i, j].ToString();
            }
        }
    }

    void DisplayTopicAndWords()
    {
        topicTitleText.text = selectedTheme;
        wordsToFindText.text = "<b>Find the words:</b> " + string.Join(", ", wordsToFind);
    }

    public void OnLetterButtonClick(Button button)
    {
        selectedWordText.text += button.GetComponentInChildren<TextMeshProUGUI>().text;
    }
}
