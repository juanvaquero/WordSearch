using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public char letter;
    public Vector2 gridPosition;
    public TextMeshProUGUI letterText;
    private LevelGenerator _levelGenerator;

    public void SetLevelGenerator(LevelGenerator levelGenerator)
    {
        _levelGenerator = levelGenerator;
    }
    public void Initialize(char c, Vector2 position)
    {
        letter = c;
        gridPosition = position;
        letterText.text = letter.ToString();
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(letter + " |down " + gridPosition.ToString());
        _levelGenerator.StartSelectingLetter(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log(letter + " |enter click down " + gridPosition.ToString());
            _levelGenerator.SelectLetter(this);
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(letter + " |up " + gridPosition.ToString());
        _levelGenerator.FinishSelectingLetter();
    }

}
