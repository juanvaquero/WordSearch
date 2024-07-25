using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WordsSearchItem : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public Image underlineImage; // Image for the strikethrough animation
    public float animationTime = 0.25f;


    private bool isFound = false;
    private Color originalTextColor;
    private Color originalUnderlineColor;

    public void Initialize(string word)
    {
        wordText.text = word;
        underlineImage.fillAmount = 0; // Make sure that the strikethrough image is initially hidden.
        originalTextColor = wordText.color;
        originalUnderlineColor = underlineImage.color;
    }

    public void MarkAsFound()
    {
        if (!isFound)
        {
            isFound = true;
            StartCoroutine(StrikethroughAnimation());
            StartCoroutine(ColorChangeAnimation());
        }
    }

    private IEnumerator StrikethroughAnimation()
    {
        float duration = animationTime;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            underlineImage.fillAmount = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        underlineImage.fillAmount = 1; // Make sure the image is completely filled at the end.
    }

    private IEnumerator ColorChangeAnimation()
    {
        float duration = animationTime;
        float elapsed = 0f;
        // Light gray
        Color endColor = new Color(0.851f, 0.835f, 0.831f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            wordText.color = Color.Lerp(originalTextColor, endColor, t);
            underlineImage.color = Color.Lerp(originalUnderlineColor, endColor, t);
            yield return null;
        }
        // Ensure final color is the correct.
        wordText.color = endColor;
        underlineImage.color = endColor;
    }
}
