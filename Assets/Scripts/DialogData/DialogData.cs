using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "dialogData", menuName = "Dialog")]
public class DialogData : ScriptableObject
{
    public Line[] lines; 
}