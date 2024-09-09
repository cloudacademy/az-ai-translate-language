
using Azure.Security.KeyVault.Secrets;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
class Program
{
    static async Task Main(string[] args)
    {
        string speechRegion = "westus";
        string sourceLanguage = "en-US";
        string keyVaultUrl = "https://yourkeyvault.vault.azure.net/";
        string secretName = "resource-key1";
        var keyVaultManager = new KeyVaultManager(keyVaultUrl);
        KeyVaultSecret speechKey = await keyVaultManager.GetSecretAsync(secretName);

        var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey.Value, speechRegion);        
        speechTranslationConfig.SpeechRecognitionLanguage = sourceLanguage;
        speechTranslationConfig.AddTargetLanguage("de");
        speechTranslationConfig.AddTargetLanguage("fr");

        SpeakToMe.speechKey = speechKey.Value;
        SpeakToMe.speechRegion = speechRegion;

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
        
        string quitText = string.Empty;
        while (quitText != "quit")
        {      
            Console.WriteLine("Speak into your microphone.");
            var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
            List<TranslatedSpeech> translatedTexts = TalkToMe.OutputSpeechRecognitionResult(translationRecognitionResult);
            if (translatedTexts[0].Text.ToLower() == "quit")
            {
                quitText = "quit";
            }	
            else
            {
                foreach (TranslatedSpeech translatedText in translatedTexts)
                {
                    Console.WriteLine($"TRANSLATED to {translatedText.Language}: {translatedText.Text}");
                    Thread.Sleep(1000); 
                    SpeakToMe.SetSpokenLanguage(translatedText.Language);
                    SpeakToMe.SynthesisToSpeakerAsync(translatedText.Text).Wait();
                }
            } 
        }
    }
}