using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

[Serializable]
public class Dialog
{
    public DialogData dialogData;
    public Sprite[] dialogBoxColors;
    public GameObject[] portraitGameObjects;
    public DialogBoxData[] dialogBoxDatas;

 
    public DialogBoxData GetDialogBoxDataWithName(string name)
    {
        foreach(DialogBoxData data in dialogBoxDatas)
        {
            if(data.name.ToLower() == name.ToLower())
            {
                return data;
            }
        }

        throw new Exception($"Invalid character name {name}");
    }

    public GameObject GetPortraitGOWithOrientation(OrientationEnum orientation) 
    {
        GameObject portrait;
        try
        {
            portrait = portraitGameObjects[(int)orientation];
        }
        catch
        {
            throw new Exception("Incorrect orientation");
        }
        return portrait;
    }

    public void SetVisuals(string name, ColorEnum color, OrientationEnum orientation)
    {
        DialogBoxData dialogBoxData = dialogBoxDatas[0];
        foreach (DialogBoxData dbd in dialogBoxDatas)
        {
            if (dbd.name == name)
            {
                dialogBoxData = dbd;
            }
        }
        GameObject portraitGO = GetPortraitGOWithOrientation(orientation);

        portraitGO.GetComponent<Image>().sprite = dialogBoxData.portraitSprite;

        switch (color)
        {
            case ColorEnum.Blue:
                dialogBoxData.dialogBox.GetComponent<Image>().sprite = dialogBoxColors[0];
                return;
            case ColorEnum.Red:
                dialogBoxData.dialogBox.GetComponent<Image>().sprite = dialogBoxColors[1];
                return;
        }
    }
}



