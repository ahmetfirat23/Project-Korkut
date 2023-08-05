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
    public List<string> Names = new();
    List<DialogBoxData> generalDBDs = new();
    TextToSpeech tts;
    DallEImageGenerator dalle;
    GameManager gm;

    void Awake()
    {
        tts = FindObjectOfType<TextToSpeech>();
        dalle = FindObjectOfType<DallEImageGenerator>();
        gm = FindAnyObjectByType<GameManager>();
    }

    public void CreatePlayerDBD() {

        DialogBoxData dbd = new() {
            name = PlayerInfo.GetName(),
            gender = PlayerInfo.GetGender(),
            color = (ColorEnum)Random.Range(0, 2),
            dialogText = dialogText,
            dialogBox = dialogBox,
            portraitOrientation = OrientationEnum.Middle,
        };
        if (gm.generateVoice)
            tts.GenerateSynthesizer(dbd, dbd.gender);
        Names.Add(PlayerInfo.GetName());
        generalDBDs.Add(dbd);
        Task task = dalle.GeneratePlayerImage(dbd);
        task.ContinueWith(task => { });
    }

    public async Task<Dialog> CreateDialog(string gptResponse) {

        List<Line> lines = SplitResponseToLines(gptResponse);

        List<Task> tasks = new();
        DialogData dialogData = new();
        List<DialogBoxData> dialogDBDs = new();
        if(gm.generateBackground)
        {
            Task background = dalle.GenerateBackgroundImage(gptResponse); 
            tasks.Append(background); 
        }

        dialogDBDs.Add(generalDBDs[0]); //Add player dbd to list

        foreach (Line line in lines)
        {
            if (!Names.Contains(line.name))
            {
                OrientationEnum portraitOrientation = 
                    line.name == "Narrator" ? OrientationEnum.Middle : (OrientationEnum)(Names.Count % 2);

                DialogBoxData dbd = new()
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
                generalDBDs.Add(dbd);
                Debug.Log($"DBD created for: {line.name}");

                if (gm.generateVoice)
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
                dialogDBDs.Add(dbd);
            }
            else
            {
                int idx = Names.IndexOf(line.name);
                if (!dialogDBDs.Contains(generalDBDs[idx]))
                {
                    dialogDBDs.Add(generalDBDs[idx]);
                }
            }
        }
        dialogData.lines = lines.ToArray();


        Dialog dialog = new()
        {
            dialogData = dialogData,
            dialogBoxColors = dialogBoxColors,
            dialogBoxDatas = dialogDBDs.ToArray(),
            portraitGameObjects = portraitGameObjects,
        };

        if (gm.generateVoice)
        {
            foreach (Line line in dialog.dialogData.lines)
            {
                line.voiceName = dialog.GetDialogBoxDataWithName(line.name).voiceName;
                tts.GenerateSentenceAudio(line, dialog.GetDialogBoxDataWithName(line.name).synthesizer);
            }
        }

        await Task.WhenAll(tasks.ToArray());

        return dialog;
    }


    public List<Line> SplitResponseToLines(string gptResponse)
    {
        List<string> speechs = new();
        List<string> names = new();
        List<Line> lines = new();

        MatchCollection speechMatches = Regex.Matches(gptResponse, @"\[[A-Za-z\s0-9\-._]+\]:([^\[]*)");
        MatchCollection namesMatches = Regex.Matches(gptResponse, @"\[([A-Za-z0-9\s\-._]+)\]:");

        foreach (Match match in namesMatches.Cast<Match>())
        {
            names.Add(match.Groups[1].Value);
            Debug.Log($"Speaker: \n{match.Groups[1].Value}");
        }

        foreach (Match match in speechMatches.Cast<Match>())
        {
            speechs.Add(match.Groups[1].Value);
            Debug.Log($"Line: \n{match.Groups[1].Value}");
        }

        if (speechs.Count == 0) {
            speechs.Add(gptResponse);
            names.Add("Narrator");
            Debug.Log($"ChatGPT formatting error! \n{gptResponse}");
        }

        for (int i = 0; i < speechs.Count; i++)
        {
            if (names[i] == "You" || names[i] == PlayerInfo.GetName())
            {
                Debug.Log($"ChatGPT formatting error! \n{names[i]}\n{speechs[i]}");
                break;
            }

            Line line = new()
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
