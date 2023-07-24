using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PromptManager
{
    private static string prompt;

    public static string GetPrompt()
    {
        return prompt;
    }

    public static void SetPrompt(string newPrompt)
    {
        prompt = newPrompt;
    }
}