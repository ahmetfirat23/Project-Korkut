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
  
        private readonly OpenAIApi openai = new();
        private List<ChatMessage> messages = new();
        private ChatMessage template;
        private string systemPrompt = @"DALLE-2 is an AI art generation model. You are a DALLE prompt generator and only return the prompt you generated.Below is a list of example prompts that can be used to generate images with DALLE:
- a fantasy style portrait painting of rachel lane / alison brie hybrid in the style of francois boucher oil painting unreal 5 daz.rpg portrait, extremely detailed artgerm greg rutkowski alphonse mucha greg hildebrandt tim hildebrandt
- athena, greek goddess, claudia black, art by artgerm and greg rutkowski and magali villeneuve, bronze greek armor, owl crown, d & d, fantasy, intricate, portrait, highly detailed, headshot, digital painting, trending on artstation, concept art, sharp focus, illustration
- a close up of a woman with a braid in her hair, fantasy concept art portrait, a portrait of an elf, character art portrait, portrait of an elf, dnd character art portrait, dnd portrait, portrait dnd, detailed matte fantasy portrait, detailed character portrait, fantasy art portrait, digital fantasy portrait, side portrait of elven royalty, portrait of a female elf warlock 
You generate prompts similar to these examples.";

        
        void Start()
        {
            template = new ChatMessage()
            {
                Role = "system",
                Content = systemPrompt
            };
            messages.Add(template);
        }


        public async Task<string> DescribeCharacter(DialogBoxData dbd, string gptAnswer)
        {
            ChatMessage newMessage = new ChatMessage()
            {
                Role = "user",
                Content = $"In a Dungeons and Dragons game, create a short background story for the character whose name is {dbd.name}. Include their class and race. Don't exceed 5 sentences. This character is from the following dialog:\r\n{gptAnswer}"
            };
            messages.Add(newMessage);

            CreateChatCompletionResponse completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages,
                Temperature = 0.4f
            });
            messages.Remove(newMessage);

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                ChatMessage message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim(); //TODO what does this do?

                Debug.Log($"Background story for the character {dbd.name}: \n{message.Content}");

                string answer = "Character Name: " + dbd.name + "\n\n\n Description: " + message.Content;
                return answer;
            }
            else
            {
                Debug.LogWarning($"No text was generated from this prompt.\n{gptAnswer}");
                return $"Describe a {dbd.name} from Dungeons and Dragons game";
            }
        }

        // If character is false, then it is for background
        public async Task<string> GenerateDallEPrompt(string description, DialogBoxData dbd) 
        {
            string custom_prompt;
            if (dbd != null)
            {
                custom_prompt = $@"In a Dungeons and Dragons game, following example prompts, generate a prompt for image generation of the character {dbd.name}'s portrait. You should use the following text for extra information. Include word 'portrait' in the prompt. Keep the prompt shorter than 70 words.
                ###Text:'{description}'";
            }
            else
            {
                custom_prompt = $@"Generate a captivating background image prompt that captures the essence of a Dungeons and Dragons adventure based on the provided message. Keep the prompt shorter than 70 words.
                ###Message: '{description}'
                ###Image Specifications: High-resolution, dark colors, highly-realistic scenes. Incorporate elements like mystical creatures, ancient ruins, lush forests, and brave {PlayerInfo.GetClass()}.";
            }


            ChatMessage newMessage = new()
            {
                Role = "user",
                Content = custom_prompt
            };
            messages.Add(newMessage);

            CreateChatCompletionResponse completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages,
                Temperature = 0.2f

            });
            messages.Remove(newMessage);

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                ChatMessage message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim(); //TODO what does this do?

                Debug.Log($"Dalle prompt for {dbd.name}: \n {message.Content}");

                return message.Content;
            }
            else
            {
                Debug.LogWarning($"No text was generated from this prompt:\n{description}");
                return description;
            }
        }
    }
}
