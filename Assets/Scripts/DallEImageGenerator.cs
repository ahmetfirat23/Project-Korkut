using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Networking;
using System.Threading.Tasks;

using OpenAI;

namespace OpenAI
{
    public class DallEImageGenerator : MonoBehaviour
    { 
        private OpenAIApi openai = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        Dialog dialog;

        //private async void Start()
        void Start()
        {
            dialog = FindObjectOfType<Dialog>();


            /*DialogTrigger dt = FindObjectOfType<DialogTrigger>();
            Dialog dialog = dt.dialog;
            DialogBoxData dbd = dialog.dialogBoxDatas[0];
            Debug.Log("started");
            await GenerateImage(dbd, @"[Narrator]: You wake up in a deserted planet. There are two orks looking at you.
[Firat]: Wake up stranger! Who are you?
[Narrator]: The second ork screams at you!
[Ahmet]: Stranger! You are under arrest!");
            Debug.Log("completed");*/
        }
        public string GetProfileInfo(){
            string player_info = "Name: " + PlayerInfo.GetName() +
                                    " Gender: " + PlayerInfo.GetGender().ToString() +
                                    " Class: " + PlayerInfo.GetClass().ToString() +
                                    " Race: " + PlayerInfo.GetRace().ToString();
            return player_info;
        }
        public async Task<string> DescribeCharacter(DialogBoxData dbd, string gptAnswer){
            //TODO make a call to gpt
            ChatMessage newMessage = new ChatMessage()
            {
                Role = "user",
                Content = $@"In a DnD(Dungeons and Dragons) game, please create a background story the character that has name {dbd.name} in the following dialog : {gptAnswer}"
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
        
        public async Task GeneratePlayerImage()
        {
            DialogBoxData playerDbd = dialog.GetDialogBoxDataWithName("you");

            string player_information = GetProfileInfo();
            string prompt = await GetComponent<DallEPromptGenerator>().GenerateDallEPrompt(player_information);
            await SendImageRequest(playerDbd, prompt);
        }
        public async Task GenerateNpcImage(DialogBoxData dbd, string str)
        {
            string description = await DescribeCharacter(dbd, str);
            string prompt = await GetComponent<DallEPromptGenerator>().GenerateDallEPrompt(description);
            await SendImageRequest(dbd, prompt);
        }

        private async Task SendImageRequest(DialogBoxData dbd, string prompt)
        {   
            CreateImageResponse response = await openai.CreateImage(new CreateImageRequest
            {
                Prompt = prompt,
                Size = ImageSize.Size256
            });

            if (response.Data != null && response.Data.Count > 0)
            {
                using(var request = new UnityWebRequest(response.Data[0].Url))
                {
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Access-Control-Allow-Origin", "*");
                    request.SendWebRequest();

                    while (!request.isDone) await Task.Yield();

                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(request.downloadHandler.data);
                    var sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero, 1f);
                    dbd.portraitSprite = sprite;
                    Debug.Log("completed");
                }
            }
            else
            {
                Debug.LogWarning("No image was created from this prompt.");
            }

        }
    }
}
