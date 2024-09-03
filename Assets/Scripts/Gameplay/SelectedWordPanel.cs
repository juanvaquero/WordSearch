using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class SelectedWordPanel : MonoBehaviour
{

    #region __Inspector Variables

    [SerializeField] private float _fadeAnimationTime = 0.5f;
    [SerializeField] private float _shakeAnimationTime = 0.5f;
    [SerializeField] private float _scaleAnimationTime = 0.5f;
    [SerializeField] private float _delayFadeAnimationTime = 0.25f;

    [SerializeField] private TextMeshProUGUI _selectedWordText;
    [SerializeField] private CanvasGroup _canvasGroup;

    #endregion
    #region __Variables
    #endregion
    #region __Public methods

    public void SetSelectedWord(string selection)
    {
        if (string.IsNullOrEmpty(selection))
            return;

        _selectedWordText.text = selection;

        if (selection != string.Empty && _canvasGroup.alpha != 1f)
            DoFadeAnimation(true);
        else if (selection.Length == 1)
            DoFadeAnimation(false);
    }

    public void DoShakeAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOShakePosition(_shakeAnimationTime, new Vector3(10f, 0, 0), snapping: true, fadeOut: true));
        sequence.Append(DoFadeAnimation(false).SetDelay(_delayFadeAnimationTime));
        sequence.Play();
    }

    public void DoScaleAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(Vector3.one * 1.15f, _scaleAnimationTime));
        sequence.Append(DoFadeAnimation(false).SetDelay(_delayFadeAnimationTime));
        sequence.Play();
    }

    #endregion
    #region __Life cycle methods

    private void Awake()
    {
        _canvasGroup.alpha = 0;
    }

    #endregion
    #region __Private methods

    private Tween DoFadeAnimation(bool fadeIn = true)
    {
        _canvasGroup.alpha = 1f;
        float endValue = 0f;
        if (fadeIn)
        {
            _canvasGroup.alpha = 0f;
            endValue = 1;
        }
        return _canvasGroup.DOFade(endValue, _fadeAnimationTime);
    }

    #endregion
}