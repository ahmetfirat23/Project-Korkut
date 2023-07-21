using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [HideInInspector] public Dialog dialog;
    [HideInInspector] public DialogData dialogData;

    [HideInInspector] public Line line;
    [HideInInspector] public DialogBoxData boxData;

    [HideInInspector] public bool finished = false;
    [HideInInspector] public bool started = false;

    [SerializeField] private AudioSource[] blipSound;
    [SerializeField] private GameObject inputFieldGO;

    GameObject portraitGO;
    Animator portraitAnim;

    bool autoClick = false;
    bool skipClick = false;
    bool isTyped = false;

    int nextLineID = 0;
    int currentSound = 0;
    int blipArrayLength;
    string sentence;

    // Start is called before the first frame update
    void Awake()
    {
        blipArrayLength = blipSound.Length;
        started = false;
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
        skipClick = false;
        autoClick = false;
        isTyped = false;

        if (lineID == -1)
        {
            StopAllCoroutines();
            boxData.dialogText.text = "";
            EndDialog();
            return;
        }

        line = dialogData.lines[lineID];


        boxData = dialog.GetDialogBoxDataWithName(line.name);
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

        sentence = line.line;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        blipSound[currentSound].Stop();
        boxData.dialogText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            boxData.dialogText.text += letter;
            currentSound = Random.Range(0, blipArrayLength);
            blipSound[currentSound].volume = Random.Range(0.17f, 0.20f);
            blipSound[currentSound].pitch = Random.Range(1.25f, 1.35f);
            blipSound[currentSound].Play();
            yield return new WaitForSeconds(0.06f);
        }
        isTyped = true;
        yield return new WaitForSeconds(5);
        autoClick = true;
        boxData.dialogBox.SetActive(false);
    }

    void EndDialog()
    {
        finished = true;
        started = false;
        blipSound[currentSound].Stop();
        /*boxData.dialogBox.SetActive(false);
        foreach (GameObject portraitGO in dialog.portraitGameObjects) 
        {
            portraitAnim = portraitGO.GetComponent<Animator>();
            portraitAnim.SetTrigger("exit");
        }*/
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

        yield return new WaitForSeconds(3f);
        autoClick = true;
        boxData.dialogBox.SetActive(false);
    }
}
