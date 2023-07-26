using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextInputManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public string userInput;
    private GameObject submitButton;
    private bool isNewUser = false; // A boolean variable to track if the user is new or already enrolled
    private HashSet<string> usedNames = new HashSet<string>(); // HashSet to store used names

    PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        isNewUser = PlayerPrefs.GetInt("IsNewUser", 1) == 1; // Default value is 1 (true) for a new user
        // Show or hide the button for generating profile photos based on the isNewUser value
        submitButton.SetActive(isNewUser);

        playerInput = GetComponent<PlayerInput>();
        if (SceneManager.GetActiveScene().name=="SampleScene")
            playerInput.actions.FindAction("Player/Submit").Disable();
        submitButton = GameObject.Find("SubmitButton");

        string usedNamesJson = PlayerPrefs.GetString("UsedNames", "[]");
        usedNames = new HashSet<string>(JsonUtility.FromJson<List<string>>(usedNamesJson));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSubmit() { onSubmit(); }

    public void OnSubmit(InputAction.CallbackContext context) { if (context.performed) onSubmit();}

    private void onSubmit()
    {
        if (inputField.text != "")
        {
            inputField.interactable = false;
            submitButton.GetComponent<Button>().interactable = false;
            EventSystem.current.SetSelectedGameObject(null);
            EnableActionMap();
            userInput = inputField.text;
            Debug.Log(userInput);
            //TODO implement logic here
        }
    }

    public void OnSkipClick(InputAction.CallbackContext context)
    {
        if (context.performed && inputField != null)
        {
            userInput = inputField.text;
        }
    }

    public void DisableActionMap()
    {
        playerInput.actions.FindAction("Player/Submit").Enable();
        playerInput.actions.FindAction("Skip").Disable();
        playerInput.actions.FindAction("Next").Disable();
    }

    public void EnableActionMap()
    {
        playerInput.actions.FindAction("Player/Submit").Disable();
        playerInput.actions.FindAction("Skip").Enable();
        playerInput.actions.FindAction("Next").Enable();
    }

    public void CreatePlayer(InputAction.CallbackContext context) { if (context.performed) createPlayer(); }
    public void CreatePlayer() { createPlayer(); }
    private void createPlayer()
    {
        PlayerInfo.SetName(GameObject.Find("NameInput").GetComponent<TMP_InputField>().text);
        PlayerInfo.SetGender(GameObject.Find("GenderInput").GetComponent<TMP_InputField>().text);
        PlayerInfo.SetClass(GameObject.Find("ClassInput").GetComponent<TMP_Dropdown>().value);
        PlayerInfo.SetRace(GameObject.Find("RaceInput").GetComponent<TMP_Dropdown>().value);

        string characterizedPrompt = "Generate a DallE prompt so that DallE model can generate a photo of the character whose information will be provided suitable to display in a Dungeons & Dragons(dnd) game, the information of the character is as follows. Name: " + PlayerInfo.GetName() + " Gender: " + PlayerInfo.GetGender() + " Class: " + PlayerInfo.GetClass() + " Race: " + PlayerInfo.GetRace();
        PromptManager.SetGptPrompt(characterizedPrompt);

        SceneManager.LoadScene("SampleScene");

        // Check if the name is already used
        if (usedNames.Contains(PlayerInfo.GetName()))
        {
            Debug.LogWarning("This name is already used. Please choose another name.");
            return;
        }

        // Save the new name to the HashSet
        usedNames.Add(PlayerInfo.GetName());
        string usedNamesJson = JsonUtility.ToJson(new List<string>(usedNames));
        PlayerPrefs.SetString("UsedNames", usedNamesJson);

        // If the user logs in (not a new user), set the PlayerPrefs value to false
        PlayerPrefs.SetInt("IsNewUser", 0);
        PlayerPrefs.Save();


    }

}
