using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;

public class DialogManager : MonoBehaviour
{
    [HideInInspector] public Dialog dialog;
    [HideInInspector] public DialogData dialogData;

    [HideInInspector] public Line line;
    [HideInInspector] public DialogBoxData boxData;
    [HideInInspector] public DialogBoxData nextBoxData;

    [HideInInspector] public bool finished = false;
    [HideInInspector] public bool started = false;

    [SerializeField] private GameObject inputFieldGO;
    [SerializeField] private AudioSource speechSource;

    GameObject portraitGO;
    Animator portraitAnim;

    bool autoClick = false;
    bool skipClick = false;
    bool isTyped = false;

    int nextLineID = 0;
    Line nextLine;
    string sentence;
    TextToSpeech tts;




    void Awake()
    {
        started = false;
        tts = FindObjectOfType<TextToSpeech>();

    }

    private void Update()
    {
        if (started)
        {
            if (autoClick || (skipClick && isTyped))
            {
                skipClick = false;
                autoClick = false;
                isTyped = false;
                nextLineID++;
                if (line.final)
                {
                    nextLineID = -1;
                }
                portraitAnim.SetBool("speaking", false);
 
                DisplayNextSentence(nextLineID);
            }

            else if (skipClick && !isTyped)
            {
                skipClick = false;
                autoClick = false;
                isTyped = false;
                StopAllCoroutines();
                StartCoroutine(DisplayWholeText());
            }
        }
    }

    public void StartDialog(Dialog dialog)
    {
        finished = false;
        started = true;
        nextLineID = 0;
        this.dialog = dialog;
        dialogData = dialog.dialogData;

        DisplayNextSentence(nextLineID);
    }

    public void DisplayNextSentence(int lineID)
    {
        speechSource.Stop();
        skipClick = false;
        autoClick = false;
        isTyped = false;

        if (lineID == -1)
        {
            StopAllCoroutines();
            boxData.dialogText.text = "";
            nextLine = null;
            EndDialog();
            return;
        }


        line = dialogData.lines[lineID];
        if (!line.final)
        {
            nextLine = dialogData.lines[lineID + 1];
            nextBoxData = dialog.GetDialogBoxDataWithName(nextLine.name);
            nextLine.speaker = nextBoxData.speaker;
            tts.GenerateSentenceAudio(nextLine, next: true);
        }

        boxData = dialog.GetDialogBoxDataWithName(line.name);
        line.speaker = boxData.speaker;
        boxData.dialogBox.SetActive(true);

        portraitGO = dialog.GetPortraitGOWithOrientation(boxData.portraitOrientation);
        portraitAnim = portraitGO.GetComponent<Animator>();

        dialog.SetVisuals(line.name, boxData.color, boxData.portraitOrientation);

        if (!portraitGO.activeSelf)
            portraitGO.SetActive(true);
        else
            portraitAnim.SetTrigger("speak");

        portraitAnim.SetBool("speaking", true);
        
        boxData.nameText.text = line.name;

        StopAllCoroutines();


        sentence = line.line;

        tts.GenerateSentenceAudio(line, next: false);
        StartCoroutine(TypeSentence(line));
    }


    IEnumerator TypeSentence(Line line)
    {
        boxData.dialogText.text = "";
        sentence = line.line;

        yield return new WaitUntil(() => (line.audio.Length != 0));
        speechSource.clip = WavUtility.ToAudioClip(line.audio);
        speechSource.Play();
          

        foreach (char letter in sentence.ToCharArray())
        {
            boxData.dialogText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        isTyped = true;
        yield return new WaitForSeconds(10);
        autoClick = true;
        boxData.dialogBox.SetActive(false);
    }

    void EndDialog()
    {
        finished = true;
        started = false;

        if (inputFieldGO.activeSelf == false)
            inputFieldGO.SetActive(true);
        inputFieldGO.GetComponent<TMP_InputField>().interactable=true;
        inputFieldGO.GetComponent<TMP_InputField>().text = "";

    }

    public void OnSkipClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (finished == false)
            {
                skipClick = true;
            }
            else
            {
                return;
            }
        }
    }


    public void OnSkipClick()
    {
        if (finished == false)
        {
            skipClick = true;
        }
        else
        {
            return;
        }
    }


    public IEnumerator DisplayWholeText()
    {
        boxData.dialogText.text = "";
        boxData.dialogText.text = sentence;
        isTyped = true;

        if (!speechSource.isPlaying)
        {
            yield return new WaitUntil(() => (line.audio.Length != 0));
            speechSource.clip = WavUtility.ToAudioClip(line.audio);
            speechSource.Play();
        }

        yield return new WaitForSeconds(5f);
        autoClick = true;
        boxData.dialogBox.SetActive(false);
    }
}
