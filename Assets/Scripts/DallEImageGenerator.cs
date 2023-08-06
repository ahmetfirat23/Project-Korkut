using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Networking;
using System.Threading.Tasks;

using OpenAI;
using Unity.VisualScripting;

namespace OpenAI
{
    public class DallEImageGenerator : MonoBehaviour
    { 

        [SerializeField] private Image backgroundImage;
        private readonly OpenAIApi openai = new();
        GameManager gm;
        Connector connector;


        private void Awake()
        {
            gm = FindObjectOfType<GameManager>();
            connector = FindAnyObjectByType<Connector>();
        }


        public string GetProfileInfo(){
            string player_info = @$"Name: { PlayerInfo.GetName()}
Gender: {PlayerInfo.GetGender()}
Class: {PlayerInfo.GetClass()}
Race: {PlayerInfo.GetRace()}";
            return player_info;
        }

        
        public async Task GeneratePlayerImage(DialogBoxData dbd)
        {
            string player_information = GetProfileInfo();
            string prompt = await GetComponent<DallEPromptGenerator>().GenerateDallEPrompt(player_information, dbd);
            await SendImageRequest(dbd, prompt);
        }


        public async Task GenerateNPCImage(DialogBoxData dbd, string str)
        {
            string prompt;
            if (gm.doubleGeneration)
            {
                string description = await GetComponent<DallEPromptGenerator>().DescribeCharacter(dbd, str);
                try
                {
                    List<string> NPCProperties = connector.SplitBackgroundStory(description);
                    description = $"Character's name is {NPCProperties[0]}, race is {NPCProperties[1]}, class is {NPCProperties[2]}, and gender is {NPCProperties[3]}. Background story as follows: {NPCProperties[4]}";
                    if (NPCProperties[3] == "Male" || NPCProperties[3] == "male")
                        dbd.gender = GenderEnum.Male;
                    else
                        dbd.gender = GenderEnum.Female;
                }
                catch (Exception e){
                    Debug.LogWarning($"Background story formatting failed\n{e}\n {str}");
                }
                prompt = await GetComponent<DallEPromptGenerator>().GenerateDallEPrompt(description, dbd);
            }
            else
                prompt = await GetComponent<DallEPromptGenerator>().GenerateDallEPrompt(str, dbd);

            await SendImageRequest(dbd, prompt);
        }

        public async Task GenerateBackgroundImage(string gptFirstAnswer)
        {
            string prompt = await GetComponent<DallEPromptGenerator>().GenerateDallEPrompt(gptFirstAnswer, null);
            await SendBackGroundImageRequest(prompt);
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
                using UnityWebRequest request = new(response.Data[0].Url);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Access-Control-Allow-Origin", "*");
                request.SendWebRequest();

                while (!request.isDone) await Task.Yield();

                Texture2D texture = new(2, 2);
                texture.LoadImage(request.downloadHandler.data);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero, 1f);
                dbd.portraitSprite = sprite;
                Debug.Log($"Image generated for {dbd.name}!");
            }
            else
            {
                Debug.LogError($"No image was created from this prompt.\n{prompt}");
            }

        }

        private async Task SendBackGroundImageRequest(string prompt)
        {   
            CreateImageResponse response = await openai.CreateImage(new CreateImageRequest
            {
                Prompt = prompt,
                Size = ImageSize.Size256
            });

            if (response.Data != null && response.Data.Count > 0)
            {
                using UnityWebRequest request = new(response.Data[0].Url);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Access-Control-Allow-Origin", "*");
                request.SendWebRequest();

                while (!request.isDone) await Task.Yield();

                Texture2D texture = new(2, 2);
                texture.LoadImage(request.downloadHandler.data);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero, 1f);
                backgroundImage.sprite = sprite;
                Debug.Log("Background image generated!");
            }
            else
            {
                Debug.LogError($"No background image was created from this prompt.\n{prompt}");
            }

        }
    }
}
