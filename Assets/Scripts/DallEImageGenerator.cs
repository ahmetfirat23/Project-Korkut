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


        //private async void Start()
        void Start()
        {

        }


        public string GetProfileInfo(){
            string player_info = "Name: " + PlayerInfo.GetName() +
                                    " Gender: " + PlayerInfo.GetGender().ToString() +
                                    " Class: " + PlayerInfo.GetClass().ToString() +
                                    " Race: " + PlayerInfo.GetRace().ToString();
            return player_info;
        }

        
        public async Task GeneratePlayerImage(DialogBoxData dbd)
        {
            string player_information = GetProfileInfo();
            string prompt = await GetComponent<DallEPromptGenerator>().GenerateDallEPrompt(player_information);
            await SendImageRequest(dbd, prompt);
        }


        public async Task GenerateNPCImage(DialogBoxData dbd, string str)
        {
            string description = await GetComponent<DallEPromptGenerator>().DescribeCharacter(dbd, str);
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
