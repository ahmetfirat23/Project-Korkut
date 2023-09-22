using Microsoft.CognitiveServices.Speech;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Collections.ObjectModel;
using UnityEngine.SceneManagement;
using System.Linq;

public class TextToSpeech : MonoBehaviour
{

    public List<string> maleVoices = new();
    public List<string> femaleVoices = new();
    public List<string> unknownVoices = new();
    int maleVoicesCount;
    int femaleVoicesCount;

    private List<string> usedVoices = new();

    private GameManager gm;

    private async void Awake()
    { 
        gm = FindObjectOfType<GameManager>();
        if (gm.generateVoice)
        {
            SpeechSynthesizer synthesizer = generateSynthesizer();
            SynthesisVoicesResult result = await synthesizer.GetVoicesAsync();
            ReadOnlyCollection<VoiceInfo> voices = result.Voices;
            Debug.Log(voices.Count);
            foreach (VoiceInfo voice in voices)
            {
                if (voice.ShortName.StartsWith("en"))
                {
                    if (voice.Gender.ToString() == "Male")
                        maleVoices.Add(voice.ShortName);
                    else if (voice.Gender.ToString() == "Female")
                        femaleVoices.Add(voice.ShortName);
                    else
                        unknownVoices.Add(voice.ShortName);
                }
            }
            synthesizer.Dispose();
            if (maleVoices.Count > 0)
                gm.EnableStartButton();
            else
                gm.OnAzureInitError();

            maleVoicesCount = maleVoices.Count;
            femaleVoicesCount = femaleVoices.Count;
        }
        else
        {
            await Task.Delay(10);
            gm.EnableStartButton();
        }
    }

    public void GenerateSynthesizer(DialogBoxData dbd, GenderEnum gender)
    {
        AudioConfig audioConfig = AudioConfig.FromStreamOutput(AudioOutputStream.CreatePullStream());
        SpeechConfig config = SpeechConfig.FromSubscription("api key here", "westus"); // old key won't work
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
        string voice; 
        int idx;
        if (usedVoices.Intersect(maleVoices).Count() == maleVoicesCount ||
            usedVoices.Intersect(femaleVoices).Count() == femaleVoicesCount)
        {
            Debug.Log("All voices used");
            usedVoices = new();
        }

        do
        {
            if (gender == GenderEnum.Male)
            {
                idx = (int)(Random.value * (maleVoices.Count - 1));
                voice = maleVoices[idx];
            }
            else if (gender == GenderEnum.Female)
            {
                idx = (int)(Random.value * (femaleVoices.Count - 1));
                voice = femaleVoices[idx];
            }
            else
            {
                idx = (int)(Random.value * (unknownVoices.Count - 1));
                voice = unknownVoices[idx];
            }
            usedVoices.Add(voice);
        }
        while (!usedVoices.Contains(voice));
        dbd.voiceName = voice;
        config.SpeechSynthesisVoiceName = voice;
        SpeechSynthesizer synthesizer = new(config, audioConfig);
        dbd.synthesizer = synthesizer;
    }

    public void GenerateSentenceAudio(Line line, SpeechSynthesizer synthesizer)
    {
        generateSentenceAudio(line, synthesizer); 
    }

    private async void generateSentenceAudio(Line line, SpeechSynthesizer synthesizer)
    {
        Debug.Log(line.voiceName);
        SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(line.line).ConfigureAwait(false);
        line.audio = result.AudioData;
        Debug.Log(result);
    }

    private SpeechSynthesizer generateSynthesizer()
    {
        AudioConfig audioConfig = AudioConfig.FromStreamOutput(AudioOutputStream.CreatePullStream());
        SpeechConfig config = SpeechConfig.FromSubscription("a7297d7539e14fd4aa773bac0b100455", "westus");
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
        SpeechSynthesizer synthesizer = new(config, audioConfig);
        return synthesizer;
    }



}
