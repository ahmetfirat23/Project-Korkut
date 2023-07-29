using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog;
    
    DialogManager dm;
    TextToSpeech tts;
    DallEImageGenerator dalle;

    public List<string> Names = new List<string>();

    public async void Start()
    {
        dm = FindObjectOfType<DialogManager>();
        tts = FindObjectOfType<TextToSpeech>();
        dalle = FindObjectOfType<DallEImageGenerator>();
        while (tts.maleVoices.Count==0)
        {
            Console.WriteLine("Excel is busy");
            await Task.Delay(25);
        }

        foreach (DialogBoxData dbd in dialog.dialogBoxDatas)
        {
            if (!Names.Contains(dbd.name))
            {
                Names.Add(dbd.name);
                Debug.Log(dbd.name);
                tts.GenerateSynthesizer(dbd, dbd.gender);
                dalle.GenerateImage(dbd, @"[Narrator]: You wake up in a deserted planet.
[Firat]: Wake up! Who are you?
[Narrator]: The men standing right besides you are huge orks!
[Ahmet]: Stranger! You are under arrest!");
            }
        }
        foreach (Line line in dialog.dialogData.lines)
        {
            tts.GenerateSentenceAudio(line, dialog.GetDialogBoxDataWithName(line.name).synthesizer);
        }
    }

    public void TriggerDialog()
    {
        if (!dm.started && tts.maleVoices.Count!=0) {
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
