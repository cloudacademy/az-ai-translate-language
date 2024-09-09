using Microsoft.CognitiveServices.Speech;
public static class SpeakToMe
{
        private static string speechLanguage = string.Empty;
        public static void SetSpokenLanguage(string language)
        {
            switch (language)
            {
                case "de":
                    speechLanguage = "de-DE-ConradNeural";
                    break;
                case "fr":
                    speechLanguage = "fr-FR-DeniseNeural";
                    break;
                case "en":
                default:
                    speechLanguage = "en-US-AriaNeural";
                    break;
            }
        }
        public static string speechKey = string.Empty;
        public static string speechRegion = string.Empty;

        public static async Task SynthesisToSpeakerAsync(string text)
        {
            // To support Chinese Characters on Windows platform
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Console.InputEncoding = System.Text.Encoding.Unicode;
                Console.OutputEncoding = System.Text.Encoding.Unicode;
            }

            var config = SpeechConfig.FromSubscription(speechKey, speechRegion);

            // Set the voice name, refer to https://aka.ms/speech/voices/neural for full list.
            config.SpeechSynthesisVoiceName = speechLanguage;//"en-US-AriaNeural";

            // Creates a speech synthesizer using the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                using (var result = await synthesizer.SpeakTextAsync(text))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Speech synthesized to speaker for text [{text}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }
}