using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GenerateImage : MonoBehaviour
{
    public Renderer tableauRenderer; // Assign the renderer of tableau8 in the Unity Inspector
    private string apiKey = "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"; // Replace with your Hugging Face API key
    private string outputDirectory;

    private string[] prompts = {
        "A futuristic cityscape",
        "An ancient Egyptian temple",
        "A Van Gogh-style starry night",
        "A surreal fantasy castle",
        "A cyberpunk neon street",
        "A scenic mountain landscape",
        "An alien planet with strange creatures",
        "A historical Roman colosseum"
    };

    void Start()
    {
        outputDirectory = Application.persistentDataPath + "/GeneratedImages";

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
            yield return GenerateAndSetImage();
            yield return new WaitForSeconds(30f); // Wait for 30 seconds before generating a new image
        }
    }

    private async Task GenerateAndSetImage()
    {
        string randomPrompt = prompts[Random.Range(0, prompts.Length)];
        string imagePath = await GenerateImageFromAPI(randomPrompt);

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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestData = new
            {
                inputs = prompt,
                options = new { wait_for_model = true }
            };

            string requestJson = JsonUtility.ToJson(requestData);

            var response = await client.PostAsync(
                "https://api-inference.huggingface.co/models/stabilityai/stable-diffusion-2",
                new StringContent(requestJson, Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                string filePath = Path.Combine(outputDirectory, "generated_image.png");
                await File.WriteAllBytesAsync(filePath, imageBytes);
                return filePath;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Error: {response.StatusCode}, {errorContent}");
                return null;
            }
        }
    }

    private void ApplyTexture(string imagePath)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        if (tableauRenderer != null)
        {
            tableauRenderer.material.mainTexture = texture; // Assign to Albedo (main texture)
        }
        else
        {
            Debug.LogError("Tableau Renderer is not assigned!");
        }
    }
}
