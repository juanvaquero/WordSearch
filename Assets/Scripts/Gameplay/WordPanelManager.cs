using System.Collections.Generic;
using UnityEngine;

public class WordPanelManager : MonoBehaviour
{
    #region Private Variables
    private WordsSearchItem _wordsSearchItemPrefab;
    private Transform _wordsSearchItemsParent;
    private Dictionary<string, WordsSearchItem> _wordSearchItems = new Dictionary<string, WordsSearchItem>();
    #endregion

    #region Public Methods
    public void Initialize(WordsSearchItem wordsSearchItemPrefab, Transform wordsSearchItemsParent)
    {
        _wordsSearchItemPrefab = wordsSearchItemPrefab;
        _wordsSearchItemsParent = wordsSearchItemsParent;
    }

    public void CreateWordPanels(List<string> wordsToFind)
    {
        foreach (Transform child in _wordsSearchItemsParent)
        {
            Destroy(child.gameObject);
        }
        _wordSearchItems.Clear();

        foreach (string word in wordsToFind)
        {
            CreateWordPanel(word);
        }
    }

    public void CreateWordPanel(string word)
    {
        WordsSearchItem wordSearchItem = Instantiate(_wordsSearchItemPrefab, _wordsSearchItemsParent);
        wordSearchItem.Initialize(word);
        if (!_wordSearchItems.ContainsKey(word))
        {
            _wordSearchItems.Add(word, wordSearchItem);
        }
    }

    public void MarkWordAsFound(string word)
    {
        if (_wordSearchItems.ContainsKey(word))
        {
            _wordSearchItems[word].MarkAsFound();
        }
    }
    #endregion
}
