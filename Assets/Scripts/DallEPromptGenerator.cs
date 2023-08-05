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
-     
    Follow the structure of the example prompts.";


        
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
            //TODO make a call to gpt
            ChatMessage newMessage = new ChatMessage()
            {
                Role = "user",
                Content = $@"In a DnD(Dungeons and Dragons) game, create a short background story the character that has name {dbd.name} in the following dialog : {gptAnswer}"
            };
            messages.Add(newMessage);

            CreateChatCompletionResponse completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages,
                Temperature = 0.6f
            });

            messages.Remove(newMessage);

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                ChatMessage message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim(); //TODO what does this do?

                Debug.Log(message.Content);

                string answer = "Character Name : " + dbd.name + "\n\n\n Description : " + message.Content;
                return answer;
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
                return "Describe a random dnd character";
            }
        }


        public async Task<string> GenerateDallEPrompt(string description, bool character) //if character is false, then it is for background
        {
            string custom_prompt;
            if (character == true){
                custom_prompt = $@"In a DnD(Dungeons and Dragons) game, following example prompts, generate a prompt for the image generation of a character portrait. You should use the following text for extra information. Include word 'portrait'in the prompt. Keep the prompt shorter than 70 words.
                ###Text:'{description}'";
            }else{
                custom_prompt = $@"Generate a captivating background image prompt that captures the essence of a Dungeons and Dragons adventure based on the provided opening message. Keep the prompt shorter than 70 words.
                Opening Message: '{description}'
                Image Specifications: High-resolution, dark colors, highly-realistic scenes. Incorporate elements like mystical creatures, ancient ruins, lush forests, and brave {PlayerInfo.GetClass()}.
                ";
            }


            ChatMessage newMessage = new ChatMessage()
            {
                Role = "user",
                Content = custom_prompt
            };
            messages.Add(newMessage);

            CreateChatCompletionResponse completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages,
                Temperature = 0f

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
                string failed_return;
                if (character == true){
                    failed_return = "Describe a random dnd character";
                }else{
                    failed_return = "Describe a creative image of a background";
                }
                
                return failed_return;
            }
        }


        
    }
}
