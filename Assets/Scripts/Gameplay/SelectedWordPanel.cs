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

    private Tween _fadeAnimation;
    private Sequence _shakeAnimation;
    private Sequence _scaleAnimation;

    #endregion
    #region __Public methods

    public void SetSelectedWord(string selection)
    {
        if (string.IsNullOrEmpty(selection))
            return;

        _selectedWordText.text = selection;
    }

    public void DoFadeAnimation(bool fadeIn)
    {
        if ((_shakeAnimation != null && _shakeAnimation.IsPlaying()) || (_scaleAnimation != null && _scaleAnimation.IsPlaying()))
            return;


        if ((fadeIn && _canvasGroup.alpha == 0f) || (!fadeIn && _canvasGroup.alpha > 0f))
        {
            _fadeAnimation?.Complete();
            _fadeAnimation = FadeAnimation(fadeIn);
        }
    }




    public void DoShakeAnimation()
    {
        _shakeAnimation = DOTween.Sequence();
        _shakeAnimation.Append(transform.DOShakePosition(_shakeAnimationTime, new Vector3(10f, 0, 0), snapping: true, fadeOut: true));
        _shakeAnimation.Append(FadeAnimation(false).SetDelay(_delayFadeAnimationTime));
        _shakeAnimation.Play();
    }

    public void DoScaleAnimation()
    {
        _scaleAnimation = DOTween.Sequence();
        _scaleAnimation.Append(transform.DOPunchScale(Vector3.one * 1.15f, _scaleAnimationTime));
        _scaleAnimation.Append(FadeAnimation(false).SetDelay(_delayFadeAnimationTime));
        _scaleAnimation.Play();
    }

    #endregion
    #region __Life cycle methods

    private void Awake()
    {
        _canvasGroup.alpha = 0;
        _shakeAnimation = DOTween.Sequence();
        _scaleAnimation = DOTween.Sequence();
    }

    #endregion
    #region __Private methods

    private Tween FadeAnimation(bool fadeIn = true)
    {
        _canvasGroup.alpha = 1f;
        float endValue = 0f;
        if (fadeIn)
        {
            _canvasGroup.alpha = 0f;
            endValue = 1f;
        }
        return _canvasGroup.DOFade(endValue, _fadeAnimationTime).OnComplete(() => _fadeAnimation = null);
    }

    #endregion
}