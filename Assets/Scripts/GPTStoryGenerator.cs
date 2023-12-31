using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Text;
using Unity.Mathematics;

namespace OpenAI
{
    public class GPTStoryGenerator : MonoBehaviour
    {
        private string systemPrompt;
        private readonly OpenAIApi openai = new();
        List<ChatMessage> messages = new();


        public async Task<string> StartStory()
        {
            systemPrompt = @$"Act as though we are playing a Game of Dungeons and Dragons 5th edition. Act as though you are the dungeon master and I am the player. My name is {PlayerInfo.GetName()}, my class is {PlayerInfo.GetClass()}, my race is {PlayerInfo.GetRace()} and my gender is {PlayerInfo.GetGender()}. We will be creating a narrative together, where I make decisions for my character, and you make decisions for all other characters (NPCs) and creatures in the world. You use a mysterious, exciting and interesting language. The story you are telling must be compelling.
You need to refer me as ""you"" when you are talking about my character's actions. Other characters can refer me as {PlayerInfo.GetName()}. 
You MUST use a lot dialogs in your story. You mustn't have inline quotations for other characters. Never speak on behalf of other characters. 
Dialogs must follow this format:
""[PersonA Name]: PersonA's speech
[PersonB Name]: PersonB's speech""    
Also when there is no dialog and you are narrating the story, you MUST output in this format:
""[Narrator]: Story""
An example output is given:
""[Narrator]: You wake up in a deserted planet.
[Stranger]: Wake up! Who are you?
[Narrator]: What are you going to do?""
You mustn't generate any dialog for me. Using [You] or [{PlayerInfo.GetName()}] is strictly forbidden. Instead at the end of your message you should ask what I will do or say and your next message you should shape the story according to my response. My actions should fail sometimes.
Keep it short. Don't exceed 100 words. No talk; just go."; /*+
                "Your responsibilities as dungeon master are to describe the setting, environment, " +
                "Non-player characters (NPCs) and their actions, as well as explain the consequences of " +
                "my actions on all of the above. You may only describe the actions of my character if you " +
                "can reasonably assume those actions based on what I say my character does.\n\nIt is also " +
                "your responsibility to determine whether my character’s actions succeed. Simple, easily " +
                "accomplished actions may succeed automatically. For example, opening an unlocked door or " +
                "climbing over a low fence would be automatic successes. Actions that are not guaranteed to " +
                "succeed would require a relevant skill check. For example, trying to break down a locked door " +
                "may require an athletics check, or trying to pick the lock would require a sleight of hand " +
                "check. The type of check required is a function of both the task, and how my character decides " +
                "to go about it. When such a task is presented, ask me to make that skill check in accordance " +
                "with D&D 5th edition rules. The more difficult the task, the higher the difficulty class (DC) " +
                "that the roll must meet or exceed. Actions that are impossible are just that: impossible. For " +
                "example, trying to pick up a building.\n\nAdditionally, you may not allow my character to make " +
                "decisions that conflict with the context or setting you’ve provided. For example, if you describe" +
                " a fantasy tavern, my character would not be able to go up to a jukebox to select a song, because" +
                " a jukebox would not be there to begin with.\n\nTry to make the setting consistent with previous " +
                "descriptions of it. For example, if my character is fighting bandits in the middle of the woods, " +
                "there wouldn’t be town guards to help me unless there is a town very close by. Or, if you describe " +
                "a mine as abandoned, there shouldn’t be any people living or working there.\n\nWhen my character " +
                "engages in combat with other NPCs or creatures in our story, ask for an initiative roll from my " +
                "character. You can also generate a roll for the other creatures involved in combat. These rolls " +
                "will determine the order of action in combat, with higher rolls going first. Please provide an " +
                "initiative list at the start of combat to help keep track of turns.\n\nFor each creature in combat," +
                " keep track of their health points (HP). Damage dealt to them should reduce their HP by the amount " +
                "of the damage dealt. To determine whether my character does damage, I will make an attack roll. " +
                "This attack roll must meet or exceed the armor class (AC) of the creature. If it does not, then " +
                "it does not hit.\n\nOn the turn of any other creature besides my character, you will decide their " +
                "action. For example, you may decide that they attack my character, run away, or make some other " +
                "decision, keeping in mind that a round of combat is 6 seconds.\n\nIf a creature decides to attack " +
                "my character, you may generate an attack roll for them. If the roll meets or exceeds my own AC, " +
                "then the attack is successful and you can now generate a damage roll. That damage roll will be " +
                "subtracted from my own hp. If the hp of a creature reaches 0, that creature dies. Participants " +
                "in combat are unable to take actions outside of their own turn.\n" +
                "Make sure that you don't exceed 75 words for every story event and finish the story in " +
                "an unexpected way after 5 responses from the player \n" +
                "Also I will provide you my character's some attributes: \n" +
                "Name: " + PlayerInfo.GetName() + "\n" +
                "Race: " + PlayerInfo.GetRace() + "\n" +
                "Gender: " + PlayerInfo.GetGender() + "\n" +
                "Class: " + PlayerInfo.GetClass()*/

            return await SendCompletionRequest(systemPrompt, "system");
        }

        
        public async Task<bool> ModerationRequest(string userInput)
        {
            CreateModerationRequest moderation = new();
            moderation.Input = userInput;
            CreateModerationResponse response = await openai.CreateModeration(moderation);

            if (response.Results != null && response.Results.Count > 0)
            {
                ModerationResult result = response.Results[0];
                Debug.Log($"Is input flagged: {result.Flagged}");
                return result.Flagged;
            }
            else
            {
                Debug.LogWarning($"Moderation API didn't respond.");
                return false;
            }
        }


        public async Task<string> SendCompletionRequestWithoutHistory(string userInput)
        {
            List<ChatMessage> messages = new()
            {
                new ChatMessage()
                {
                Role = "user",
                Content = userInput
                }
            };
            ChatMessage message = await sendCompletionRequest(messages);
            return message.Content;
        }


        public async Task<string> SendCompletionRequest(string userInput, string role = "user")
        {
            ChatMessage newMessage = new()
            {
                Role = role,
                Content = userInput
            };
            messages.Add(newMessage);
            ChatMessage message = await sendCompletionRequest(messages);
            messages.Add(message);
            return message.Content;
        }


        private async Task<ChatMessage> sendCompletionRequest(List<ChatMessage> messages)
        {
            CreateChatCompletionResponse completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages,
            });

            ChatMessage message;
            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim(); //TODO what does this do?

                Debug.Log($"Generated completion: \n{message.Content}");
            }
            else
            {
                message = new ChatMessage()
                {
                    Role = "assistant",
                    Content = "System error, please try again."
                };
                Debug.LogWarning("No text was generated from this prompt.");
            }

            return message;
        }

    }
}
