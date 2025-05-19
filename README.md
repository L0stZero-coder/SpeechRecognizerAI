# 🧠 SpeechRecognizerAI

**SpeechRecognizerAI** is a lightweight C# console application that listens to live voice input, transcribes it using Azure Cognitive Services, and uses an AI model to extract and learn personal details. The extracted information is stored in a persistent, JSON-backed memory file, forming an evolving user profile over time.

This tool demonstrates how real-time speech, AI understanding, and persistent memory can be combined into a dynamic, privacy-conscious voice interface.

---

## 🧩 Core Architecture

The solution is built around **three core components**:

### 🎙️ 1. Audio Capture & Speech-to-Text
- Uses **Azure Cognitive Services Speech SDK** to continuously listen to the default microphone.
- Streams audio to Azure and returns **real-time transcription**.
- Leverages Microsoft’s neural network models for enhanced noise resilience and accuracy.

### 🧠 2. AI Understanding & Information Extraction
- Each transcript is sent to an LLM (e.g., OpenAI GPT) via .NET client libraries.
- The prompt instructs the model to extract structured personal attributes as JSON, such as:
  - Identity (e.g., name, age)
  - Preferences (e.g., favorite food, topics)
  - Mood and sentiment
  - Interests (e.g., hobbies, current projects)
- The returned JSON is parsed and prepared for memory updates.

### 💾 3. Memory Management & Learning Loop
- Persistent memory is stored as a local JSON file.
- Internally structured as a dictionary: `user_id → {attribute → value}`.
- New data is **merged** into the profile on every utterance:
  - Existing values are updated.
  - New attributes are added.
- Profiles evolve over time for **personalized, long-term engagement**.

---

## 🧪 Console Experience

- Transcribed text is printed live.
- AI-parsed personal data is displayed in structured format.
- The updated memory snapshot is shown after every interaction.
- Basic error handling ensures resilience and data integrity.

---

## 🔧 Extensibility Options

| Component           | Upgrade Ideas |
|--------------------|---------------|
| User Identification| Replace static ID with real participant identifiers from your chat/VoIP platform |
| Storage Backend     | Swap flat file with SQL/NoSQL database for scalability |
| AI Prompt           | Customize prompt to extract traits relevant to your use case |
| Error Handling      | Add retries, fallbacks, and timeouts |
| Security            | Implement encryption, HTTPS/TLS, and access control mechanisms |

---

## ⚠️ Privacy, Security & Compliance

This application collects and stores **potentially sensitive personal information**. Before using or deploying, consider the following:

- ✅ **Explicit Consent**: Always inform users about data collection and storage.
- 📉 **Data Minimization**: Store only what’s necessary for your application.
- 🔒 **Encryption**: Protect data at rest and in transit.
- 👥 **Access Control**: Restrict memory file/database access to authorized processes only.
- 📜 **Legal Compliance**: Align with GDPR, CCPA, HIPAA, or local regulations.
- 🧹 **Retention Policy**: Define and enforce deletion policies for stale profiles.

---

## 🚀 Summary

VoiceProfileAI combines:
- ✅ Real-time transcription
- ✅ AI-powered information parsing
- ✅ Persistent, user-centric memory

This makes it ideal for intelligent assistants, adaptive voice interfaces, personalized AI companions, or behavioral analysis tools—with full transparency and control over user data.

---

## 📄 License

Open for research, development, and ethical experimentation. A formal license may be added in future versions.

---

## 🤝 Contributions

Pull requests and feature suggestions are welcome! Help improve this foundation for more responsible and intelligent voice-enabled AI.
