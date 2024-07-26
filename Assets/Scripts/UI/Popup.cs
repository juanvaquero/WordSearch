using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    #region __Variables
    [SerializeField]
    protected string _popupID;
    protected LevelGenerator _levelGenerator;
    #endregion

    #region __Public Methods

    public string GetPopupID()
    {
        return _popupID;
    }

    public virtual void Configure(LevelGenerator levelGenerator)
    {
        _levelGenerator = levelGenerator;
    }

    public abstract void Show();
    public abstract void Hide();

    #endregion
}
