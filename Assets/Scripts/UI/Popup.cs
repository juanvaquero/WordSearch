using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class Popup : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    protected string _popupID;
    [SerializeField]
    protected float _fadeDuration = 0.5f;
    #endregion

    #region Protected Variables 
    protected LevelGenerator _levelGenerator;
    #endregion

    #region Private Variables 
    private CanvasGroup _canvasGroup;
    #endregion

    #region Public Methods
    public string GetPopupID()
    {
        return _popupID;
    }

    public virtual void Configure(LevelGenerator levelGenerator)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _levelGenerator = levelGenerator;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        if (gameObject.activeInHierarchy)
            StartCoroutine(FadeIn());
    }

    public virtual void Hide()
    {
        _canvasGroup.alpha = 1f;
        if (gameObject.activeInHierarchy)
            StartCoroutine(FadeOut());
    }
    #endregion

    #region Private Methods
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
    #endregion
}