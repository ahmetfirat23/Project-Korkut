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
    GPTStoryGenerator gsg;
    Connector connector;
    GameManager gm;

    bool triggered = false;
    

    private void Awake()
    {
        dm = FindObjectOfType<DialogManager>();
        gsg = FindObjectOfType<GPTStoryGenerator>();
        connector = FindObjectOfType<Connector>();
        gm = FindObjectOfType<GameManager>();
    }

    public async void TriggerDialog()
    {
        if (!dm.started && !triggered) {
            triggered = true;
            connector.CreatePlayerDBD();
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
            bool flagged = await gsg.ModerationRequest(userInput);
            string gptResponse;
            if (!flagged || !gm.moderate)
                gptResponse = await gsg.SendCompletionRequest(userInput);
            else
                gptResponse = "[Narrator]: Inappropriate input! Please be more considerate and try again.";

            Dialog dialog = await connector.CreateDialog(gptResponse);
            dm.StartDialog(dialog);
            triggered = false;
        }
    }
}
