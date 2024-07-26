using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupResetLevel : Popup
{
    public GameObject popup; // Reference to the popup GameObject
    public TextMeshProUGUI descriptionText; // Reference to description text
    public Button yesButton; // Reference to the "Yes" button
    public Button noButton; // Reference to the "No" button


    void Start()
    {
        // Attach listeners to the buttons
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);
    }

    public override void Show()
    {
        popup.SetActive(true);
    }

    public override void Hide()
    {
        popup.SetActive(false);
    }

    // Method called when "Yes" button is clicked
    void OnYesButtonClicked()
    {
        // Reset the level with a new theme and words
        _levelGenerator.ResetLevel();

        // Hide the popup
        PopupController.Instance.HidePopup(_popupID);
    }

    // Method called when "No" button is clicked
    void OnNoButtonClicked()
    {
        // Exit the application
        Application.Quit();
    }


}
