using System;
using System.Collections.Generic;

[Serializable]
public struct WordList
{
    public bool allowDictionary;
    public List<string> words;
    public List<string> bonus;
}