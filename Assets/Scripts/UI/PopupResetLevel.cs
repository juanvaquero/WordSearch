using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupResetLevel : Popup
{
    #region Serialized Fields
    [SerializeField] private GameObject _popup; // Reference to the popup GameObject
    [SerializeField] private TextMeshProUGUI _descriptionText; // Reference to description text
    [SerializeField] private Button _yesButton; // Reference to the "Yes" button
    [SerializeField] private Button _noButton; // Reference to the "No" button
    #endregion

    #region Unity Methods
    private void Start()
    {
        // Attach listeners to the buttons
        _yesButton.onClick.AddListener(OnYesButtonClicked);
        _noButton.onClick.AddListener(OnNoButtonClicked);
    }
    #endregion

    #region Public Methods
    public override void Show()
    {
        _popup.SetActive(true);
    }

    public override void Hide()
    {
        _popup.SetActive(false);
    }
    #endregion

    #region Private Methods
    private void OnYesButtonClicked()
    {
        // Reset the level with a new theme and words
        _levelGenerator.ResetLevel();

        // Hide the popup
        PopupController.Instance.HidePopup(_popupID);
    }

    private void OnNoButtonClicked()
    {
        // Exit the application
        Application.Quit();
    }
    #endregion
}
