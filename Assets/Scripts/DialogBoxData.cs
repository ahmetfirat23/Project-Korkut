using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.CognitiveServices.Speech;

public enum ColorEnum { Red, Blue};
public enum OrientationEnum { Left, Middle, Right };
public enum GenderEnum { Male, Female, Unknown};

[System.Serializable]
public class DialogBoxData
{
    public string name;
    public string speaker;
    public GenderEnum gender;
    public ColorEnum color;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public GameObject dialogBox;
    public Sprite portraitSprite;
    public OrientationEnum portraitOrientation;
}
