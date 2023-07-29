using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    DialogManager dm;
    TextToSpeech tts;
    GPTStoryGenerator gsg;
    Connector connector;

    bool triggered = false;
    

    public void Start()
    {
        dm = FindObjectOfType<DialogManager>();
        tts = FindObjectOfType<TextToSpeech>();
        gsg = FindObjectOfType<GPTStoryGenerator>();
        connector = FindObjectOfType<Connector>();
    }

    public async void TriggerDialog()
    {
        if (!dm.started && tts.maleVoices.Count!=0 && !triggered) {
            triggered = true;
            string gptResponse = await gsg.StartStory();
            Dialog dialog = await connector.CreateDialog(gptResponse);
            dm.StartDialog(dialog);
            triggered = false;
        }
    }

    public async void NextDialog(string userInput) 
    {
        if (!dm.started && !triggered) 
        {
            triggered = true;
            string gptResponse = await gsg.SendCompletionRequest(userInput);
            Dialog dialog = await connector.CreateDialog(gptResponse);
            dm.StartDialog(dialog);
            triggered = false;
        }
    }
}
