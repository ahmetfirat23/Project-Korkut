using Microsoft.CognitiveServices.Speech;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Collections.ObjectModel;

public class TextToSpeech : MonoBehaviour
{

    public List<string> maleVoices = new List<string>();
    public List<string> femaleVoices = new List<string>();
    public List<string> unknownVoices = new List<string>();

    private List<string> usedVoices = new List<string>();


    private async void Awake()
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
        Debug.Log(maleVoices.Count);
        Debug.Log(femaleVoices.Count);
        Debug.Log(unknownVoices.Count);
    }

    public void GenerateSynthesizer(DialogBoxData dbd, GenderEnum gender)
    {
        AudioConfig audioConfig = AudioConfig.FromStreamOutput(AudioOutputStream.CreatePullStream());
        SpeechConfig config = SpeechConfig.FromSubscription("a7297d7539e14fd4aa773bac0b100455", "westus");
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
        string voice; 
        int idx;
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
        SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, audioConfig);
        dbd.synthesizer = synthesizer;
    }

    public void GenerateSentenceAudio(Line line, SpeechSynthesizer synthesizer)
    {
        generateSentenceAudio(line, synthesizer); 
    }

    private SpeechSynthesizer generateSynthesizer()
    {
        AudioConfig audioConfig = AudioConfig.FromStreamOutput(AudioOutputStream.CreatePullStream());
        SpeechConfig config = SpeechConfig.FromSubscription("a7297d7539e14fd4aa773bac0b100455", "westus");
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
        SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, audioConfig);
        return synthesizer;
    }



    private async void generateSentenceAudio(Line line, SpeechSynthesizer synthesizer)
    {
        Debug.Log(line.voiceName);
        SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(line.line).ConfigureAwait(false);
        line.audio = result.AudioData;
        Debug.Log(result);
    }
}
