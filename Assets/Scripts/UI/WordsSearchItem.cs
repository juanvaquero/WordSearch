using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WordsSearchItem : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _wordText;
    [SerializeField] private Image _underlineImage;
    [SerializeField] private float _animationTime = 0.25f;
    #endregion

    #region Private Variables
    private bool _isFound = false;
    private Color _originalTextColor;
    private Color _originalUnderlineColor;
    #endregion

    #region Public Methods
    public void Initialize(string word)
    {
        _wordText.text = word;
        _underlineImage.fillAmount = 0; // Ensure that the strikethrough image is initially hidden.
        _originalTextColor = _wordText.color;
        _originalUnderlineColor = _underlineImage.color;
    }

    public void MarkAsFound()
    {
        if (!_isFound)
        {
            _isFound = true;
            StartCoroutine(StrikethroughAnimation());
            StartCoroutine(ColorChangeAnimation());
        }
    }
    #endregion

    #region Private Methods
    private IEnumerator StrikethroughAnimation()
    {
        float elapsed = 0f;

        while (elapsed < _animationTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _animationTime;
            _underlineImage.fillAmount = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        _underlineImage.fillAmount = 1; // Ensure the image is completely filled at the end.
    }

    private IEnumerator ColorChangeAnimation()
    {
        float elapsed = 0f;
        // Light gray
        Color endColor = new Color(0.851f, 0.835f, 0.831f);

        while (elapsed < _animationTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _animationTime;
            _wordText.color = Color.Lerp(_originalTextColor, endColor, t);
            _underlineImage.color = Color.Lerp(_originalUnderlineColor, endColor, t);
            yield return null;
        }
        // Ensure final color is the correct.
        _wordText.color = endColor;
        _underlineImage.color = endColor;
    }
    #endregion
}
