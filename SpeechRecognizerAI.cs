using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure;
using Azure.AI.TextAnalytics;               // For optional post-processing
using Microsoft.CognitiveServices.Speech;    // Install Microsoft.CognitiveServices.Speech
using Microsoft.CognitiveServices.Speech.Audio;
using OpenAI_API;                            // Install OpenAI-API-dotnet
using Newtonsoft.Json;

namespace VoiceChatAI
{
    class Program
    {
        // 1) CONFIGURATION: your keys & endpoints
        private static readonly string speechKey    = "YOUR_AZURE_SPEECH_KEY";
        private static readonly string speechRegion = "YOUR_AZURE_SPEECH_REGION";
        private static readonly string openAiKey    = "YOUR_OPENAI_API_KEY";
        private static readonly string memoryPath   = "user_memory.json";

        // In-memory cache of user profiles
        private static Dictionary<string, Dictionary<string, object>> Memory;

        static async Task Main(string[] args)
        {
            LoadMemory();

            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            using var recognizer = new SpeechRecognizer(speechConfig, AudioConfig.FromDefaultMicrophoneInput());

            Console.WriteLine("Say something ...");

            recognizer.Recognized += async (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    string userId = "default_user"; // extend to multiple users
                    Console.WriteLine($"User said: {e.Result.Text}");

                    // 2) PROCESS with AI
                    var extractedFacts = await AnalyzeWithAI(e.Result.Text);

                    // 3) UPDATE MEMORY
                    UpdateMemory(userId, extractedFacts);

                    Console.WriteLine("Memory updated:");
                    Console.WriteLine(JsonConvert.SerializeObject(Memory[userId], Formatting.Indented));
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine("Speech could not be recognized.");
                }
            };

            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

            SaveMemory();
        }

        // Load existing memory from disk (if any)
        private static void LoadMemory()
        {
            if (File.Exists(memoryPath))
            {
                var json = File.ReadAllText(memoryPath);
                Memory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
            }
            else
            {
                Memory = new Dictionary<string, Dictionary<string, object>>();
            }
        }

        // Save memory back to disk
        private static void SaveMemory()
        {
            var json = JsonConvert.SerializeObject(Memory, Formatting.Indented);
            File.WriteAllText(memoryPath, json);
        }

        // Calls OpenAI (or any LLM) to extract structured info from raw transcript
        private static async Task<Dictionary<string, object>> AnalyzeWithAI(string transcript)
        {
            var api = new OpenAIAPI(openAiKey);

            // Prompt to extract name, preferences, mood, topics, etc.
            string prompt = $@"
You are a personal assistant. Extract any personal details you can about the speaker from this transcript.
Respond with a JSON object containing keys like 'name', 'age', 'preferences', 'hobbies', 'mood', etc. 
If unknown, omit the field.
Transcript: ""{transcript}""";

            var chat = api.Chat.CreateConversation();
            chat.AppendSystemMessage("You are a helpful assistant that outputs pure JSON.");
            chat.AppendUserInput(prompt);

            var response = await chat.GetResponseFromChatbotAsync();
            try
            {
                // Parse returned JSON
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            }
            catch
            {
                Console.WriteLine("AI response was not valid JSON. Raw response:");
                Console.WriteLine(response);
                return new Dictionary<string, object>();
            }
        }

        // Merge new facts into existing memory for a given user
        private static void UpdateMemory(string userId, Dictionary<string, object> newFacts)
        {
            if (!Memory.ContainsKey(userId))
                Memory[userId] = new Dictionary<string, object>();

            foreach (var kv in newFacts)
            {
                Memory[userId][kv.Key] = kv.Value;
            }
            SaveMemory();
        }
    }
}
