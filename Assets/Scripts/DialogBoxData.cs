using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ColorEnum { Red, Blue};
public enum OrientationEnum { Left, Middle, Right };

[System.Serializable]
public class DialogBoxData
{
    public string name;
    public ColorEnum color;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public GameObject dialogBox;
    public Sprite portraitSprite;
    public OrientationEnum portraitOrientation;
}
