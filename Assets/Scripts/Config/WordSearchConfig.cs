using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WordSearchConfig", menuName = "ScriptableObjects/WordSearchConfig", order = 1)]
public class WordSearchConfig : ScriptableObject
{
    public List<Color> colors;
}
