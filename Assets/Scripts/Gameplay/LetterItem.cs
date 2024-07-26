using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LetterItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    #region Public Variables
    public char Letter { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    #endregion

    #region Serialized Variables
    [SerializeField] private TextMeshProUGUI _letterText;
    #endregion

    #region Private Variables
    private LevelGenerator _levelGenerator;
    #endregion

    #region Public Methods
    public void SetLevelGenerator(LevelGenerator levelGenerator)
    {
        _levelGenerator = levelGenerator;
    }

    public void Initialize(char c, Vector2Int position)
    {
        Letter = c;
        GridPosition = position;
        _letterText.text = Letter.ToString();
    }
    #endregion

    #region Pointer Event Handlers
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(Letter + " |down " + GridPosition);
        _levelGenerator.StartSelectingLetter(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log(Letter + " |enter click down " + GridPosition);
            _levelGenerator.SelectLetter(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(Letter + " |up " + GridPosition);
        _levelGenerator.FinishSelectingLetter();
    }
    #endregion
}
