using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.Networking;
using System.Threading.Tasks;

using OpenAI;

namespace OpenAI
{
    public class DallEImageGenerator : MonoBehaviour
    { 
        private OpenAIApi openai = new OpenAIApi();

        private async void Start()
        {
            DialogTrigger dt = FindObjectOfType<DialogTrigger>();
            Dialog dialog = dt.dialog;
            DialogBoxData dbd = dialog.dialogBoxDatas[0];
            Debug.Log("started");
            await GenerateImage(dbd, "[Narrator]: You wake up in a deserted planet." +
                "[Firat]: Wake up! Who are you?" +
                "[Narrator]: The man standing right besides you is a huge ork!");
            Debug.Log("completed");
        }
        private async Task GenerateImage(DialogBoxData dbd, string str)
        {
            string prompt = await GetComponent<DallEPromptGenerator>().GenerateDallEPrompt(dbd, str);
            await SendImageRequest(dbd, prompt);
        }

        private async Task SendImageRequest(DialogBoxData dbd, string prompt)
        {   
            CreateImageResponse response = await openai.CreateImage(new CreateImageRequest
            {
                Prompt = prompt,
                Size = ImageSize.Size512
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
                    var sprite = Sprite.Create(texture, new Rect(0, 0, 512, 512), Vector2.zero, 1f);
                    dbd.portraitSprite = sprite;
                }
            }
            else
            {
                Debug.LogWarning("No image was created from this prompt.");
            }

        }
    }
}
