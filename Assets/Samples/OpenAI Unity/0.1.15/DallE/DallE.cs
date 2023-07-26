using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.Networking;
using System.Threading.Tasks;

namespace OpenAI
{
    public class DallE : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private Image image;
        //[SerializeField] private GameObject loadingLabel;

        private OpenAIApi openai = new OpenAIApi();

        private void Start()
        {
            button.onClick.AddListener(SendImageRequest);
        }

        public void OnTextEntry(string text)
        {
             PromptManager.SetDallePrompt(text);
        }

        private async void SendImageRequest()
        {
            image.sprite = null;
            button.enabled = false;
            inputField.enabled = false;
            //loadingLabel.SetActive(true);

            var response = await openai.CreateImage(new CreateImageRequest
            {
                Prompt = PromptManager.GetDallePrompt(),
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
                    image.sprite = sprite;
                }
            }
            else
            {
                Debug.LogWarning("No image was created from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
            //loadingLabel.SetActive(false);
        }
    }
}
