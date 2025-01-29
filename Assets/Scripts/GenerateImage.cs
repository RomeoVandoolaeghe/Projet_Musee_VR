using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GenerateImage : MonoBehaviour
{
    public Renderer tableauRenderer;
    // Make sure to paste your API key here without any spaces or quotes around it
    private const string API_KEY = "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";
    private string outputDirectory;
    private Texture2D currentTexture;

    private string[] prompts = {
        "A 10x10 realistic, artistic art A futuristic cityscape",
        "A 10x10 realistic, artistic art An ancient Egyptian temple",
        "A 10x10 realistic, artistic art A Van Gogh-style starry night",
        "A 10x10 realistic, artistic art A surreal fantasy castle",
        "A 10x10 realistic, artistic art A cyberpunk neon street",
        "A 10x10 realistic, artistic art A scenic mountain landscape",
        "A 10x10 realistic, artistic art An alien planet with strange creatures",
        "A 10x10 realistic, artistic art A historical Roman colosseum",
        "A 10x10 realistic, artistic art A tiny 10x10  art of a sunny beach with palm trees.",
        "10x10 realistic, artistic art of a cozy cabin in a snowy forest.",
        "A 10x10 realistic, artistic art of a futuristic city skyline at night.",
        "A 10x10 realistic, artistic art of a magical glowing forest.",
        "A 10x10 realistic, artistic art of a medieval castle on a hill.",
        "A 10x10 realistic, artistic art of a spaceship flying through a starry sky.",
        "A 10x10 realistic, artistic art of a serene mountain lake at sunrise.",
        "A 10x10 realistic, artistic art of a bustling marketplace in a desert town.",
        "A 10x10 realistic, artistic art of a pirate ship sailing on stormy seas.",
        "A 10x10 realistic, artistic art of a futuristic robot in a neon-lit alley.",
        "A 10x10 realistic, artistic art of a mystical dragon perched on a cliff.",
        "A 10x10 realistic, artistic art of a vibrant coral reef underwater.",
        "A 10x10 realistic, artistic art of a haunted house on a foggy night.",
        "A 10x10 realistic, artistic art of a sunflower field under a blue sky.",
        "A 10x10 realistic, artistic art of a Viking longship sailing through icy waters.",
        "A 10x10 realistic, artistic art of a futuristic hovercar in a cyberpunk city.",
        "A 10x10 realistic, artistic art of a tranquil Japanese zen garden.",
        "A 10x10 realistic, artistic art of a fiery volcano erupting at night.",
        "A 10x10 realistic, artistic art of a whimsical candy land with lollipops.",
        "A 10x10 realistic, artistic art of a lone astronaut on a distant planet."
    };

    void Start()
    {
        outputDirectory = Path.Combine(Application.persistentDataPath, "GeneratedImages");
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        StartCoroutine(UpdateImageRoutine());
    }

    IEnumerator UpdateImageRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(GenerateAndSetImageCoroutine());
            yield return new WaitForSeconds(15f);
        }
    }

    private IEnumerator GenerateAndSetImageCoroutine()
    {
        string randomPrompt = prompts[Random.Range(0, prompts.Length)];
        Debug.Log($"Generating image for prompt: {randomPrompt}");
        var task = GenerateImageFromAPI(randomPrompt);
        yield return new WaitUntil(() => task.IsCompleted);

        string imagePath = task.Result;
        if (!string.IsNullOrEmpty(imagePath))
        {
            ApplyTexture(imagePath);
        }
        else
        {
            Debug.LogError("Failed to generate image.");
        }
    }

    private async Task<string> GenerateImageFromAPI(string prompt)
    {
        using (HttpClient client = new HttpClient())
        {
            // Ensure proper formatting of the Authorization header
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY.Trim());

            // Escape special characters in the prompt
            string escapedPrompt = prompt.Replace("\"", "\\\"");
            string requestJson = $"{{\"inputs\": \"{escapedPrompt}\"}}";

            while (true)
            {
                try
                {
                    Debug.Log($"Using API Key: {API_KEY}"); // Be careful with logging API keys in production
                    Debug.Log($"Sending request with content: {requestJson}");

                    var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    // Add additional headers that might be required
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await client.PostAsync(
                        "https://api-inference.huggingface.co/models/stabilityai/stable-diffusion-2",
                        content
                    );

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Debug.Log($"Response status: {response.StatusCode}");
                    Debug.Log($"Response content: {responseContent}");

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                        string filePath = Path.Combine(outputDirectory, $"generated_image_{System.DateTime.Now.Ticks}.png");
                        await File.WriteAllBytesAsync(filePath, imageBytes);
                        Debug.Log($"Image saved to: {filePath}");
                        return filePath;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        // Parse the estimated time from the response
                        var errorResponse = JsonUtility.FromJson<ErrorResponse>(responseContent);
                        float estimatedTime = errorResponse.estimated_time;

                        Debug.Log($"Model is loading. Retrying in {estimatedTime} seconds...");
                        await Task.Delay((int)(estimatedTime * 1000)); // Convert to milliseconds
                    }
                    else
                    {
                        Debug.LogError($"Error: {response.StatusCode}, {responseContent}");
                        Debug.LogError($"Headers sent: {client.DefaultRequestHeaders}");
                        return null;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error generating image: {e.Message}");
                    Debug.LogError($"Stack trace: {e.StackTrace}");
                    return null;
                }
            }
        }
    }

    private void ApplyTexture(string imagePath)
    {
        if (currentTexture != null)
        {
            Destroy(currentTexture);
        }

        byte[] imageBytes = File.ReadAllBytes(imagePath);
        currentTexture = new Texture2D(2, 2);
        currentTexture.LoadImage(imageBytes);

        if (tableauRenderer != null)
        {
            tableauRenderer.material.mainTexture = currentTexture;
        }
        else
        {
            Debug.LogError("Tableau Renderer is not assigned!");
        }
    }

    void OnDestroy()
    {
        if (currentTexture != null)
        {
            Destroy(currentTexture);
        }
    }

    [System.Serializable]
    private class ErrorResponse
    {
        public string error;
        public float estimated_time;
    }
}