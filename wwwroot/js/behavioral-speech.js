document.addEventListener("DOMContentLoaded", () => {
    const micBtn = document.getElementById("micBtn");
    const status = document.getElementById("status");

    micBtn.addEventListener("click", async () => {
        try {
            status.innerText = "Getting speech token...";

            const tokenRes = await fetch("/api/speech/token");
            if (!tokenRes.ok) throw new Error("Failed to get speech token");
            const { token, region } = await tokenRes.json();

            status.innerText = "Starting speech recognition...";

            const speechConfig = SpeechSDK.SpeechConfig.fromAuthorizationToken(token, region);
            speechConfig.speechRecognitionLanguage = "en-US";

            const audioConfig = SpeechSDK.AudioConfig.fromDefaultMicrophoneInput();
            const recognizer = new SpeechSDK.SpeechRecognizer(speechConfig, audioConfig);

            recognizer.recognizeOnceAsync(async (result) => {
                if (result.reason === SpeechSDK.ResultReason.RecognizedSpeech) {
                    const text = result.text?.trim();
                    if (!text) {
                        status.innerText = "No speech recognized. Please try again.";
                        recognizer.close();
                        return;
                    }

                    addMessage("user", text);
                    status.innerText = "Processing response...";

                    // Space for AI processing transcript
                }
                else {
                    status.innerText = "Speech not recognized. Please try again.";
                }
                recognizer.close();
            })
        } catch (e) {
            console.error(e);
            status.innerText = "An error occurred. Please try again.";
        }
    });
});

function addMessage(sender, text) {
    const li = document.createElement("li");
    li.className = sender;
    li.innerText = text;
    document.getElementById("chat").appendChild(li);
}