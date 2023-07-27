using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptManager : MonoBehaviour
{
    private static string dallePrompt;
    private static string gptPromptToSendDalle;
    private static string gptPrompt;



    public static string GetDallePrompt()
    {
        return dallePrompt;
    }

    public static void SetDallePrompt(string newDallePrompt)
    {
        dallePrompt = newDallePrompt;
    }

    public static string GetGptPromptToSendDalle()
    {
        return gptPromptToSendDalle;
    }

    public static void SetGptPromptToSendDalle(string newgptPromptToSendDalle)
    {
        gptPromptToSendDalle = newgptPromptToSendDalle;
    }



}