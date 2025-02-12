using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net;

public class GenerateImage : MonoBehaviour
{
    [Header("References (Assign these in the Inspector)")]
    public Renderer tableauRenderer;      // Should be the Renderer component of your "tableau5" GameObject.
    public TextMeshPro loadingText;       // UI text element to indicate loading.
    public Button imageButton;            // The button that, when pressed, triggers image generation.

    private const string API_KEY = "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";
    private string outputDirectory;
    private Texture2D currentTexture;

    private string[] prompts = {
        "An imposing mountain landscape under a dramatic sky",
        "A sunny meadow dotted with wild flowers and a sparkling stream",
        "A surreal desert with high sand dunes at sunset",
        "A mysterious enchanted forest with rays of light piercing through the trees",
        "A crystalline lake reflecting majestic mountains and a brilliant sky",
        "A panoramic view of rolling hills under a vibrant sunset",
        "A coastal cliff overlooking a rough sea and crashing waves",
        "A misty morning in a lush and peaceful valley",
        "A futuristic city set against a surreal natural backdrop",
        "A majestic waterfall cascading into a turquoise pool in the heart of a tropical jungle",
        "A starry sky dominating a peaceful country landscape",
        "A spectacular canyon with impressive rock formations and a winding river",
        "A tranquil winter landscape with snow-covered trees in the twilight",
        "A pastel sunset over a calm sea horizon",
        "The ancient ruins of a temple lost in the middle of a colourful jungle",
        "A rustic village bathed in light, nestling among lush green hills",
        "An autumn forest with leaves blazing in red, orange and gold",
        "A winding river running through a spectacular and varied landscape",
        "A cosmic landscape combining natural elements and futuristic touches",
        "A dreamlike world of floating islands and strangely shaped flora",
    };

    void Start()
    {
        // Set up the output directory for saving generated images.
        outputDirectory = Path.Combine(Application.persistentDataPath, "GeneratedImages");
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Set default loading text.
        if (loadingText != null)
        {
            loadingText.text = "Press Button to\nGenerate an Image";
            loadingText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Loading Text is not assigned in the Inspector!");
        }

        // Automatically add a listener to the button if it is assigned via Inspector.
        if (imageButton != null)
        {
            imageButton.onClick.AddListener(GenerateImageOnButtonPress);
        }
        else
        {
            Debug.LogError("Image Button is not assigned in the Inspector!");
        }
    }

    public void GenerateImageOnButtonPress()
    {
        StartCoroutine(GenerateImageCoroutine());
    }

    private IEnumerator GenerateImageCoroutine()
    {
        Debug.Log("GenerateImageCoroutine started.");

        if (loadingText == null)
        {
            Debug.LogError("Loading Text is not assigned!");
            yield break;
        }

        if (tableauRenderer == null)
        {
            Debug.LogError("Tableau Renderer is not assigned!");
            yield break;
        }

        // Set loading text and disable button
        loadingText.text = "Loading...\nPlease Wait";
        imageButton.interactable = false;

        string randomPrompt = prompts[UnityEngine.Random.Range(0, prompts.Length)];
        Debug.Log($"Generating image for prompt: {randomPrompt}");

        var task = GenerateImageFromAPI(randomPrompt);
        yield return new WaitUntil(() => task.IsCompleted);

        string imagePath = task.Result;
        if (!string.IsNullOrEmpty(imagePath))
        {
            Debug.Log("Image generated successfully.");
            ApplyTexture(imagePath);
        }
        else
        {
            Debug.LogError("Failed to generate image.");
        }

        // Reset UI state
        loadingText.text = "Press Button to\nGenerate an Image";
        imageButton.interactable = true;
    }

    private async Task<string> GenerateImageFromAPI(string prompt)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY.Trim());

            string escapedPrompt = prompt.Replace("\"", "\\\"");
            string requestJson = $"{{\"inputs\": \"{escapedPrompt}\"}}";

            while (true)
            {
                try
                {
                    var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await client.PostAsync(
                        "https://api-inference.huggingface.co/models/stabilityai/stable-diffusion-2",
                        content
                    );

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                        string filePath = Path.Combine(outputDirectory, $"generated_image_{DateTime.Now.Ticks}.png");
                        await File.WriteAllBytesAsync(filePath, imageBytes);
                        return filePath;
                    }
                    else if (response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                            response.StatusCode == (HttpStatusCode)429) // 429 = Too Many Requests
                    {
                        var errorResponse = JsonUtility.FromJson<ErrorResponse>(responseContent);
                        float estimatedTime = errorResponse.estimated_time > 0 ?
                            errorResponse.estimated_time :
                            10f; // Default to 10 seconds if no estimate

                        Debug.Log($"API busy. Retrying in {estimatedTime} seconds...");
                        await Task.Delay((int)(estimatedTime * 1000));
                        continue; // Keep loading text active
                    }
                    else
                    {
                        Debug.LogError($"API Error: {response.StatusCode}\n{responseContent}");
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Network Error: {e.Message}");
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
            Debug.Log("Texture applied successfully.");
        }
        else
        {
            Debug.LogError("Tableau Renderer is not assigned!");
        }
    }

    private void OnDestroy()
    {
        if (currentTexture != null)
        {
            Destroy(currentTexture);
        }
    }

    [Serializable]
    private class ErrorResponse
    {
        public string error;
        public float estimated_time;
    }
}