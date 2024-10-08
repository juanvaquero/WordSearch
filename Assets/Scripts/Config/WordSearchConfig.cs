using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WordSearchConfig", menuName = "ScriptableObjects/WordSearchConfig", order = 1)]
public class WordSearchConfig : ScriptableObject
{
    public int GridSize = 10;
    /// <summary>
    /// Offset to left and right border of the screen of words search panel.
    /// </summary>
    [Tooltip("Offset to left and right border of the screen of words search panel.")]
    public int WidthOffsetScreen = 80;
    public int NumberOfWordsToFind = 5;
    public bool AllowVerticalPlacement = true;
    public bool AllowDiagonalPlacement = true;
    public bool AllowReverseWords = true;
    public List<Color> Colors = new List<Color>();
}
