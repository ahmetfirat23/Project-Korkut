using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TextInputManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public string userInput;
    private GameObject submitButton;

    PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Player/Submit").Disable();
        submitButton = GameObject.Find("SubmitButton");
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

}
