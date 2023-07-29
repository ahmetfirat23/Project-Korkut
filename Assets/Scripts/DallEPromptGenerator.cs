using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenAI
{
    public class DallEPromptGenerator : MonoBehaviour
    {
  
        private OpenAIApi openai = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private ChatMessage template;
        private string systemPrompt = @"Stable Diffusion is an AI art generation model similar to DALLE-2.
You are a DallE prompt generator and only return the prompt you generated.
Below is a list of prompts that can be used to generate images with Stable Diffusion:
- a fantasy style portrait painting of rachel lane / alison brie hybrid in the style of francois boucher oil painting unreal 5 daz.rpg portrait, extremely detailed artgerm greg rutkowski alphonse mucha greg hildebrandt tim hildebrandt
- athena, greek goddess, claudia black, art by artgerm and greg rutkowski and magali villeneuve, bronze greek armor, owl crown, d & d, fantasy, intricate, portrait, highly detailed, headshot, digital painting, trending on artstation, concept art, sharp focus, illustration
- a close up of a woman with a braid in her hair, fantasy concept art portrait, a portrait of an elf, character art portrait, portrait of an elf, dnd character art portrait, dnd portrait, portrait dnd, detailed matte fantasy portrait, detailed character portrait, fantasy art portrait, digital fantasy portrait, side portrait of elven royalty, portrait of a female elf warlock
    Follow the structure of the example prompts.";


        private string prompt = "Generate a DallE prompt so that DallE model can generate a photo of the character whose information will be provided suitable to display in a Dungeons & Dragons(dnd) game, the information of the character is as follows. Name: " + PlayerInfo.GetName() + " Gender: " + PlayerInfo.GetGender() + " Class: " + PlayerInfo.GetClass() + " Race: " + PlayerInfo.GetRace();
        

        void Start()
        {
            template = new ChatMessage()
            {
                Role = "system",
                Content = systemPrompt
            };
            messages.Add(template);
        }

        public async Task<string> GenerateDallEPrompt(DialogBoxData dbd, string str)
        {
            ChatMessage newMessage = new ChatMessage()
            {
                Role = "user",
                Content = $@"Following example prompts, generate a prompt for {dbd.name}'s portrait image generation. You may use the following text for extra information. Include word 'portrait'in the prompt. No talk; just go. Keep the prompt shorter than 70 words.
###Text:'{str}'"
            };
            messages.Add(newMessage);

            CreateChatCompletionResponse completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            messages.Remove(newMessage);

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                ChatMessage message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim(); //TODO what does this do?

                Debug.Log(message.Content);

                return message.Content;
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
                return "Portrait of a man";
            }
        }
    }
}
