using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Translation;

public class TranslatedSpeech
{
    public string Text { get; set; }
    public string Language { get; set; }

    public TranslatedSpeech (string language, string text)
    {
        Text = text;
        Language = language;
    }
}

public static class TalkToMe
{
    public static string SourceLanguage = "en";
    public static List<TranslatedSpeech> OutputSpeechRecognitionResult(TranslationRecognitionResult translationRecognitionResult)
    {
        List<TranslatedSpeech > output = new List<TranslatedSpeech>();
        switch (translationRecognitionResult.Reason)
        {
            case ResultReason.TranslatedSpeech:
                if (translationRecognitionResult.Text.ToLower().StartsWith("quit"))
                {
                    output.Add(new TranslatedSpeech(SourceLanguage, "quit"));
                }
                else
                {
                    output.Add(new TranslatedSpeech(SourceLanguage, $"RECOGNIZED: Text = {translationRecognitionResult.Text}"));
                    foreach (var element in translationRecognitionResult.Translations)
                    {
                        output.Add(new TranslatedSpeech(element.Key, element.Value));
                    }
                }
                break;
            case ResultReason.NoMatch:
                output.Add(new TranslatedSpeech(SourceLanguage, $"NOMATCH: Speech could not be recognized."));
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(translationRecognitionResult);
                output.Add(new TranslatedSpeech(SourceLanguage, $"CANCELED: Reason={cancellation.Reason}"));

                if (cancellation.Reason == CancellationReason.Error)
                {
                    output.Add(new TranslatedSpeech(SourceLanguage, $"CANCELED: ErrorCode={cancellation.ErrorCode}"));
                    output.Add(new TranslatedSpeech(SourceLanguage, $"CANCELED: ErrorDetails={cancellation.ErrorDetails}"));
                    output.Add(new TranslatedSpeech(SourceLanguage, $"CANCELED: Did you set the speech resource key and region values?"));
                }
                break;
        }
        return output;
    }
}