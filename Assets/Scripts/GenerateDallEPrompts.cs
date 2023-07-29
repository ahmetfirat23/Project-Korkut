using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenAI
{
    public class GenerateDallEPrompts : MonoBehaviour
    {
  
        private OpenAIApi openai = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Generate a DallE prompt so that DallE model can generate a photo of the character whose information will be provided suitable to display in a Dungeons & Dragons(dnd) game, the information of the character is as follows. Name: " + PlayerInfo.GetName() + " Gender: " + PlayerInfo.GetGender() + " Class: " + PlayerInfo.GetClass() + " Race: " + PlayerInfo.GetRace();
        /*private void Start()
        {
            SendReply();
        }*/

        public async Task SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = " "
            };

            if (messages.Count == 0) newMessage.Content = prompt;

            messages.Add(newMessage);

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);
                Debug.Log(message.Content);

                PromptManager.SetGptPromptToSendDalle(message.Content);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }
    }
}
