using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject CharacterGeneration;
    public GameObject GameCanvas;
    public GameObject StartButton;
    public GameObject WaitText;
    public GameObject LeftPortrait;
    public GameObject MidPortrait;
    public GameObject RightPortrait;
    public GameObject TextInput;

    public bool generateBackground = true;
    public bool generateVoice = true;
    public bool doubleGeneration = true;

    private TextInputManager tim;

    private void Awake()
    {
        tim = FindObjectOfType<TextInputManager>();
    }

    private void Start()
    {
        LeftPortrait.SetActive(false);
        MidPortrait.SetActive(false);
        RightPortrait.SetActive(false);
        TextInput.SetActive(false);
        CharacterGeneration.SetActive(true);
        StartButton.GetComponent<Button>().interactable = false;
    }

    public void EnableStartButton()
    {
        WaitText.SetActive(false);
        StartButton.GetComponent<Button>().interactable = true;
        tim.SwitchStartButton(true);
    }

    public void OnAzureInitError()
    {
        Debug.LogError("No voices found!");
        WaitText.GetComponent<TMP_Text>().text = "No voices found! Reload the scene and retrying again.\nIf the error continues, check your Azure API key and account.";
        StartButton.GetComponent<Button>().interactable = true;
        StartButton.GetComponentInChildren<TMP_Text>().text = "Reload";
        StartButton.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
    }

    public void StartGame()
    {
        CharacterGeneration.SetActive(false);
        FindObjectOfType<DialogTrigger>().TriggerDialog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
