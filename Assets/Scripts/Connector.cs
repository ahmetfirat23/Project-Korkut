using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Connector : MonoBehaviour
{
    public Sprite[] dialogBoxColors;
    public GameObject[] portraitGameObjects;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public GameObject dialogBox;
    public List<string> Names = new List<string>();
    List<DialogBoxData> dbds = new List<DialogBoxData>();

    GPTStoryGenerator gsg;
    TextToSpeech tts;
    DallEImageGenerator dalle;


    void Start()
    {
        gsg = FindObjectOfType<GPTStoryGenerator>();
        tts = FindObjectOfType<TextToSpeech>();
        dalle = FindObjectOfType<DallEImageGenerator>();

    }

    public async Task<Dialog> CreateDialog(string gptResponse) {

        List<Line> lines = SplitResponseToLines(gptResponse);
        
        List<Task> tasks = new List<Task>();
        DialogData dialogData = new DialogData();
        List<DialogBoxData> dbdList = new List<DialogBoxData>();

        foreach (Line line in lines)
        {
            if (!Names.Contains(line.name))
            {
                OrientationEnum portraitOrientation = line.name == "Narrator" ? OrientationEnum.Middle : (OrientationEnum)(Names.Count % 2);
                DialogBoxData dbd = new DialogBoxData()
                {
                    name = line.name,
                    gender = (GenderEnum)Random.Range(0, 3),
                    color = (ColorEnum)Random.Range(0, 2),
                    nameText = nameText,
                    dialogText = dialogText,
                    dialogBox = dialogBox,
                    portraitOrientation = portraitOrientation
                };
                Names.Add(line.name);
                dbds.Add(dbd);
                Debug.Log(line.name);
                tts.GenerateSynthesizer(dbd, dbd.gender);
                Task task = dalle.GenerateImage(dbd, gptResponse);
                tasks.Add(task);
                dbdList.Add(dbd);
            }
            else
            {
                int idx = Names.IndexOf(line.name);
                if (!dbdList.Contains(dbds[idx]))
                {
                    Debug.Log(line.name);
                    dbdList.Add(dbds[idx]);
                }
            }
        }
        dialogData.lines = lines.ToArray();

        Dialog dialog = new Dialog()
        {
            dialogData = dialogData,
            dialogBoxColors = dialogBoxColors,
            dialogBoxDatas = dbdList.ToArray(),
            portraitGameObjects = portraitGameObjects,
        };

        foreach (Line line in dialog.dialogData.lines)
        {
            line.voiceName = dialog.GetDialogBoxDataWithName(line.name).voiceName;
            tts.GenerateSentenceAudio(line, dialog.GetDialogBoxDataWithName(line.name).synthesizer);
        }
        foreach (GameObject portraitGO in dialog.portraitGameObjects)
            portraitGO.SetActive(false);
        await Task.WhenAll(tasks.ToArray());

        return dialog;
    }

    // Update is called once per frame
    public List<Line> SplitResponseToLines(string gptResponse)
    { 
        List<string> speechs = new List<string>();
        List<string> names = new List<string>();
        List<Line> lines = new List<Line>();
        //TODO Logic to split response into list of names and speechs
        names.Add("Narrator");
        speechs.Add(gptResponse);
        for (int i = 0; i < speechs.Count; i++)
        {
            Line line = new Line()
            {
                name = names[i],
                line = speechs[i]
            };
            lines.Add(line);
        }
        lines[^1].final = true;
        return lines;
    }
}
