using System.Collections.Generic;

[System.Serializable]
public class WordThemesData
{
    public List<Theme> themes;
}

[System.Serializable]
public class Theme
{
    public string name;
    public List<string> words;
}