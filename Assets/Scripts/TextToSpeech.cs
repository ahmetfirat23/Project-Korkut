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

    private List<SpeechConfig> configs = new List<SpeechConfig>();
    private SpeechConfig config;

    SpeechSynthesizer synthesizerNow;
    SpeechSynthesizer synthesizerNext;


    public async void Start()
    {
        synthesizerNow = GenerateSynthesizer();
        synthesizerNext = GenerateSynthesizer();

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

    public void SelectVoice(DialogBoxData dbd, GenderEnum gender)
    {
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
        dbd.speaker = voice;
    }

    public SpeechSynthesizer GenerateSynthesizer()
    {
        AudioConfig audioConfig = AudioConfig.FromStreamOutput(AudioOutputStream.CreatePullStream());
        SpeechConfig config = SpeechConfig.FromSubscription("a7297d7539e14fd4aa773bac0b100455", "westus");
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
        configs.Add(config);
        SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, audioConfig);
        return synthesizer;
    }

    public async void GenerateSentenceAudio(Line line, bool next)
    {
        if (!next){
            await synthesizerNow.StopSpeakingAsync();
            setSynthesizerVoice(line.speaker, 0);
            generateSentenceAudio(line, synthesizerNow); 
        }
        else
        {
            await synthesizerNext.StopSpeakingAsync();
            setSynthesizerVoice(line.speaker, 1);
            generateSentenceAudio(line, synthesizerNext);
        }
    }

    private SpeechSynthesizer generateSynthesizer()
    {
        AudioConfig audioConfig = AudioConfig.FromStreamOutput(AudioOutputStream.CreatePullStream());
        SpeechConfig config = SpeechConfig.FromSubscription("a7297d7539e14fd4aa773bac0b100455", "westus");
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
        SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, audioConfig);
        return synthesizer;
    }

    private void setSynthesizerVoice(string voiceName, int idx)
    {
        config = configs[idx];
        Debug.Log(voiceName);
        config.SpeechSynthesisVoiceName = voiceName;
    }

    private async void generateSentenceAudio(Line line, SpeechSynthesizer synthesizer)
    {
        if (line.audio.Length == 0)
        {
            Debug.Log(line.speaker);
            Debug.Log(config.SpeechSynthesisVoiceName);
            SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(line.line).ConfigureAwait(false);
            await synthesizer.StopSpeakingAsync();
            line.audio = result.AudioData;
            Debug.Log(result);
        }
    }
}
