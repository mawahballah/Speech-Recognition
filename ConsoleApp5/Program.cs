using System;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.SpeechRecognition;

namespace SpeechRecognition
{
    public class Authentication
    {
        public static readonly string FetchTokenUri = "https://api.cognitive.microsoft.com/sts/v1.0";
        private string subscriptionKey;
        private string token;

        public Authentication(string subscriptionKey)
        {
            this.subscriptionKey = subscriptionKey;
            this.token = FetchTokenAsync(FetchTokenUri, subscriptionKey).Result;
        }

        public string GetAccessToken()
        {
            return this.token;
        }

        private async Task<string> FetchTokenAsync(string fetchUri, string subscriptionKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                UriBuilder uriBuilder = new UriBuilder(fetchUri);
                uriBuilder.Path += "/issueToken";

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
                Console.WriteLine("Token Uri: {0}", uriBuilder.Uri.AbsoluteUri);
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {            
            recorder myrecorder = new recorder();
            string input = Console.ReadLine();
            while(input!="exit")
            {
                myrecorder.CreateMicrophoneRecoClient();
                myrecorder.StartMicAndRecognition();
                input = Console.ReadLine();
            }
        }
    }
    public class recorder
    {
        private MicrophoneRecognitionClient micClient;
        private SpeechRecognitionMode Mode=SpeechRecognitionMode.ShortPhrase;
        private string DefaultLocale="en-US";
        private string SubscriptionKey = "Add your Key subscription key here";
        private string AuthenticationUri = "";
        private void OnMicrophoneStatus(object sender, MicrophoneEventArgs e)
        {                          
                if (e.Recording)
                {
                    Console.WriteLine("Please start speaking.");
                }          
        }
        private void OnPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            Console.WriteLine("--- Partial result received by OnPartialResponseReceivedHandler() ---");
            Console.WriteLine("{0}", e.PartialResult);
            Console.WriteLine();
        }
        private void WriteResponseResult(SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.Results.Length == 0)
            {
                Console.WriteLine("No phrase response is available.");
            }
            else
            {
                Console.WriteLine("********* Final n-BEST Results *********");
                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {
                    Console.WriteLine(
                        "[{0}] Confidence={1}, Text=\"{2}\"",
                        i,
                        e.PhraseResponse.Results[i].Confidence,
                        e.PhraseResponse.Results[i].DisplayText);

                }

                SpeechSynthesizer speaker = new SpeechSynthesizer();
                speaker.Speak(e.PhraseResponse.Results[0].DisplayText);
                Console.WriteLine();
            }
        }
        private void OnMicShortPhraseResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
                Console.WriteLine("--- OnMicShortPhraseResponseReceivedHandler ---");

                // we got the final result, so it we can end the mic reco.  No need to do this
                // for dataReco, since we already called endAudio() on it as soon as we were done
                // sending all the data.
                this.micClient.EndMicAndRecognition();

                this.WriteResponseResult(e);            
        }
        private void OnMicDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            Console.WriteLine("--- OnMicDictationResponseReceivedHandler ---");
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.EndOfDictation ||
                e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
            {                
                        // we got the final result, so it we can end the mic reco.  No need to do this
                        // for dataReco, since we already called endAudio() on it as soon as we were done
                        // sending all the data.
                        this.micClient.EndMicAndRecognition();
            }

            this.WriteResponseResult(e);
        }
        private void OnConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {           
            Console.WriteLine("--- Error received by OnConversationErrorHandler() ---");
            Console.WriteLine("Error code: {0}", e.SpeechErrorCode.ToString());
            Console.WriteLine("Error text: {0}", e.SpeechErrorText);
            Console.WriteLine();
        }
        public void StartMicAndRecognition()
        {
            this.micClient.StartMicAndRecognition();
        }
        public void CreateMicrophoneRecoClient()
        {
            this.micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                this.Mode,
                this.DefaultLocale,
                this.SubscriptionKey);
            this.micClient.AuthenticationUri = this.AuthenticationUri;

            // Event handlers for speech recognition results
            this.micClient.OnMicrophoneStatus += this.OnMicrophoneStatus;
            this.micClient.OnPartialResponseReceived += this.OnPartialResponseReceivedHandler;
            if (this.Mode == SpeechRecognitionMode.ShortPhrase)
            {
                this.micClient.OnResponseReceived += this.OnMicShortPhraseResponseReceivedHandler;
            }
            else if (this.Mode == SpeechRecognitionMode.LongDictation)
            {
                this.micClient.OnResponseReceived += this.OnMicDictationResponseReceivedHandler;
            }

            this.micClient.OnConversationError += this.OnConversationErrorHandler;
        }

    }
}
