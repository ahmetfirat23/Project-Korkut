using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Line
{
    public string name;
    [TextArea(2, 10)]
    public string line;
    public bool final;
}