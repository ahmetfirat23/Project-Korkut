using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog;
    
    DialogManager dm;
    TextToSpeech tts;

    public List<string> Names = new List<string>();

    public void Start()
    {
        dm = FindObjectOfType<DialogManager>();
        tts = FindObjectOfType<TextToSpeech>();
    }

    public void TriggerDialog()
    {
        if (!dm.started && tts.maleVoices.Count!=0) {

            foreach (DialogBoxData dbd in dialog.dialogBoxDatas)
            {
                if (!Names.Contains(dbd.name))
                {
                    Names.Add(dbd.name);
                    Debug.Log(dbd.name);
                    tts.SelectVoice(dbd, dbd.gender);
                }
            }
            foreach(Line line in dialog.dialogData.lines)
            {
                line.audio = new byte[] { };
            }
            foreach (GameObject portraitGO in dialog.portraitGameObjects)
                portraitGO.SetActive(false);
            dm.StartDialog(dialog);
        }
    }

    public void NextDialog(DialogData dd, DialogBoxData[] dbds) 
    {
        if (!dm.started)
        {
            dialog.dialogData = dd;
            dialog.dialogBoxDatas.AddRange(dbds);
            foreach (GameObject portraitGO in dialog.portraitGameObjects)
                portraitGO.SetActive(false);
            dm.StartDialog(dialog);
        }
    }
}
