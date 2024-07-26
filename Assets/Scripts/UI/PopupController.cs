using UnityEngine;
using System.Collections.Generic;
using System;

public class PopupController : MonoBehaviour
{
    public static PopupController Instance { get; private set; }

    public LevelGenerator levelGenerator;
    public GameObject backgroundPopups;
    public List<Popup> popups = new List<Popup>();

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

        backgroundPopups.SetActive(false);
        for (int i = 0; i < popups.Count; i++)
        {
            popups[i].Configure(levelGenerator);
            popups[i].Hide();
        }
    }

    // Method to show a popup
    public void ShowPopup(string popupID)
    {
        Popup popup = popups.Find((p) => p.GetPopupID() == popupID);
        if (popup != null)
        {
            backgroundPopups.SetActive(true);
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
        Popup popup = popups.Find((p) => p.GetPopupID() == popupID);
        if (popup != null)
        {
            popup.Hide();
            backgroundPopups.SetActive(false);
        }
        else
        {
            Debug.LogError($"Popup with name {popupID} not found.");
        }
    }
}
