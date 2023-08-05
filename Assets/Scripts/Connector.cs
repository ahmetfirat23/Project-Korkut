using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Connector : MonoBehaviour
{
    public Sprite[] femaleNarrators;
    public Sprite[] maleNarrators;
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

    public void CreatePlayer() {

        DialogBoxData dbd = new DialogBoxData() {
            name = PlayerInfo.GetName(),
            gender = PlayerInfo.GetGender(),
            color = (ColorEnum)Random.Range(0, 2),
            dialogText = dialogText,
            dialogBox = dialogBox,
            portraitOrientation = OrientationEnum.Middle,
        };
        tts.GenerateSynthesizer(dbd, dbd.gender);
        Names.Add(PlayerInfo.GetName());
        dbds.Add(dbd);
        Task task = dalle.GeneratePlayerImage(dbd);
        task.ContinueWith(task => { });
    }

    public async Task<Dialog> CreateDialog(string gptResponse) {
 
        List<Line> lines = SplitResponseToLines(gptResponse);
        
        List<Task> tasks = new List<Task>();
        DialogData dialogData = new DialogData();
        List<DialogBoxData> dbdList = new List<DialogBoxData>();

        Task background = dalle.GenerateBackgroundImage(gptResponse);
        tasks.Append(background);

        dbdList.Add(dbds[0]);

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

                if (line.name == "Narrator")
                {
                    if (dbd.gender == GenderEnum.Male)
                        dbd.portraitSprite = maleNarrators[Random.Range(0, maleNarrators.Length)];
                    else
                        dbd.portraitSprite = femaleNarrators[Random.Range(0, femaleNarrators.Length)];
                }
                else
                {
                    Task task = dalle.GenerateNPCImage(dbd, gptResponse);
                    tasks.Add(task);
                }
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
        /**foreach (GameObject portraitGO in dialog.portraitGameObjects)
            portraitGO.SetActive(false);**/
        await Task.WhenAll(tasks.ToArray());

        return dialog;
    }

    // Update is called once per frame
    //[Narrator]:Speak
    //[a KISISI]:SPEAK STUFF.
    public List<Line> SplitResponseToLines(string gptResponse)
    {
        List<string> speechs = new List<string>();
        List<string> names = new List<string>();
        List<Line> lines = new List<Line>();

        MatchCollection speechMatches = Regex.Matches(gptResponse, @"\[[A-Za-z\s0-9]+\]:([^\[]*)");
        MatchCollection namesMatches = Regex.Matches(gptResponse, @"\[([A-Za-z0-9\s]+)\]:");
        foreach (Match match in speechMatches)
        {
            speechs.Add(match.Groups[1].Value);
            Debug.Log(match.Groups[1].Value);
        }
        foreach (Match match in namesMatches)
        {
            names.Add(match.Groups[1].Value);
            Debug.Log(match.Groups[1].Value);
        }
        if (speechs.Count == 0) {
            speechs.Add(gptResponse);
            names.Add("Narrator");
            Debug.Log("ChatGPT formatting error!");
        }

        for (int i = 0; i < speechs.Count; i++)
        {
            if (names[i] == "You" || names[i] == PlayerInfo.GetName())
                break;
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
