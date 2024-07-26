using UnityEngine;
using System.Collections.Generic;
using System;

public class PopupController : MonoBehaviour
{
    #region Singleton
    public static PopupController Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private GameObject _backgroundPopups;
    [SerializeField] private List<Popup> _popups = new List<Popup>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        //Singleton implementation:
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _backgroundPopups.SetActive(false);
        for (int i = 0; i < _popups.Count; i++)
        {
            _popups[i].Configure(_levelGenerator);
            _popups[i].Hide();
        }
    }
    #endregion

    #region Public Methods
    // Method to show a popup
    public void ShowPopup(string popupID)
    {
        Popup popup = _popups.Find((p) => p.GetPopupID() == popupID);
        if (popup != null)
        {
            _backgroundPopups.SetActive(true);
            popup.Show();
        }
        else
        {
            Debug.LogError($"Popup with name {popupID} not found.");
        }
    }

    // Method to hide a popup
    public void HidePopup(string popupID)
    {
        Popup popup = _popups.Find((p) => p.GetPopupID() == popupID);
        if (popup != null)
        {
            popup.Hide();
            _backgroundPopups.SetActive(false);
        }
        else
        {
            Debug.LogError($"Popup with name {popupID} not found.");
        }
    }
    #endregion
}
