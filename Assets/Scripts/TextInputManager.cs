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
    [HideInInspector] public GameObject submitButton;
    [HideInInspector] public GameObject skipButton;
    DialogTrigger dialogTrigger;
    

    PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        dialogTrigger = FindObjectOfType<DialogTrigger>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Player/Submit").Disable();
        submitButton = GameObject.Find("SubmitButton");
        skipButton = GameObject.Find("SkipButton");
    }


    public void OnSubmit() { onSubmit(); }

    public void OnSubmit(InputAction.CallbackContext context) { if (context.performed) onSubmit();}

    private void onSubmit()
    {
        if (inputField.text != "")
        {
            inputField.interactable = false;
            submitButton.GetComponent<Button>().interactable = false;
            skipButton.GetComponent<Button>().interactable = true;
            EventSystem.current.SetSelectedGameObject(null);
            SwitchActionMap(true);
            SwitchSubmitButton(false);
            userInput = inputField.text;
            Debug.Log(userInput);
            dialogTrigger.NextDialog(userInput);
        }
    }
    /*
    public void OnSkipClick(InputAction.CallbackContext context)
    {
        if (context.performed && inputField != null)
        {
            userInput = inputField.text;
        }
    }*/

    public void SwitchSubmitButton(bool enable)
    {
        if (enable)
        {
            playerInput.actions.FindAction("Player/Submit").Enable();
        }
        else
        {
            playerInput.actions.FindAction("Player/Submit").Disable();
        }

    }

    public void SwitchActionMap(bool enable)
    {
        if (enable)
        {
            playerInput.actions.FindAction("Skip").Enable();
        }
        else
        {
            playerInput.actions.FindAction("Skip").Disable();
        }
    }

    public void CreatePlayer(InputAction.CallbackContext context) { if (context.performed) createPlayer(); }
    public void CreatePlayer() { createPlayer(); }
    private void createPlayer()
    {
        PlayerInfo.SetName(GameObject.Find("NameInput").GetComponent<TMP_InputField>().text);
        PlayerInfo.SetGender(GameObject.Find("GenderInput").GetComponent<TMP_Dropdown>().value);
        PlayerInfo.SetClass(GameObject.Find("ClassInput").GetComponent<TMP_Dropdown>().value);
        PlayerInfo.SetRace(GameObject.Find("RaceInput").GetComponent<TMP_Dropdown>().value);

        playerInput.actions.FindAction("Start").Disable();
        GameObject.Find("CharacterGeneration").SetActive(false);
        FindObjectOfType<DialogTrigger>().TriggerDialog();
    }

}
