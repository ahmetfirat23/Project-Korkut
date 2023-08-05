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
    public string userInput;
    public GameObject submitButton;
    public GameObject skipButton;
    public GameObject inputField;

    DialogTrigger dialogTrigger;
    PlayerInput playerInput;

    void Awake()
    {
        dialogTrigger = FindObjectOfType<DialogTrigger>();
        playerInput = GetComponent<PlayerInput>();
    }
    void Start()
    {
        playerInput.actions.FindAction("Player/Submit").Disable();
        SwitchSubmitButton(false);
        SwitchStartButton(false);
    }


    public void OnSubmit() { onSubmit(); }

    public void OnSubmit(InputAction.CallbackContext context) { if (context.performed) onSubmit();}

    private void onSubmit()
    {
        if (inputField.GetComponent<TMP_InputField>().text != "")
        {
            inputField.GetComponent<TMP_InputField>().interactable = false;
            submitButton.GetComponent<Button>().interactable = false;
            skipButton.GetComponent<Button>().interactable = true;
            EventSystem.current.SetSelectedGameObject(null);
            SwitchSkipButton(true);
            SwitchSubmitButton(false);
            userInput = inputField.GetComponent<TMP_InputField>().text;
            Debug.Log($"User entered: \n{userInput}");
            dialogTrigger.NextDialog(userInput);
        }
    }


    public void SwitchSubmitButton(bool enable)
    {
        if (enable)
            playerInput.actions.FindAction("Player/Submit").Enable();
        else
            playerInput.actions.FindAction("Player/Submit").Disable();

    }

    public void SwitchSkipButton(bool enable)
    {
        if (enable)
            playerInput.actions.FindAction("Skip").Enable();
        else
            playerInput.actions.FindAction("Skip").Disable();
    }

    public void SwitchStartButton(bool enable)
    {
        if (enable)
            playerInput.actions.FindAction("Start").Enable();
        else
            playerInput.actions.FindAction("Start").Disable();
    }


    public void CreatePlayer(InputAction.CallbackContext context) { if (context.performed) createPlayer(); }
    public void CreatePlayer() { createPlayer(); }
    private void createPlayer()
    {
        PlayerInfo.SetName(GameObject.Find("NameInput").GetComponent<TMP_InputField>().text);
        PlayerInfo.SetGender(GameObject.Find("GenderInput").GetComponent<TMP_Dropdown>().value);
        PlayerInfo.SetClass(GameObject.Find("ClassInput").GetComponent<TMP_Dropdown>().value);
        PlayerInfo.SetRace(GameObject.Find("RaceInput").GetComponent<TMP_Dropdown>().value);

        SwitchStartButton(false);
        FindObjectOfType<GameManager>().StartGame();
    }

}
