using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WordSearch
{
    public class WordSelection : MonoBehaviour
    {
        #region __Inspector Variables

        [SerializeField]
        private TextMeshProUGUI _selectedWordText;

        #endregion
        #region __Variables

        private string _selectedWord = "";
        private List<Button> _selectedButtons = new List<Button>();

        #endregion

        #region __Public methods
        #endregion
        public void OnLetterButtonClick(Button button)
        {
            _selectedWord += button.GetComponentInChildren<TextMeshProUGUI>().text;
            _selectedButtons.Add(button);
            _selectedWordText.text = _selectedWord;
        }

        public void ValidateWord()
        {
            // Lógica para validar si la palabra seleccionada es correcta
        }

        public void ResetSelection()
        {
            _selectedWord = "";
            _selectedButtons.Clear();
            _selectedWordText.text = _selectedWord;
        }
        #region __Private methods
        #endregion
    }
}
