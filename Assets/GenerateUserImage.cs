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

public class GenerateUserImage : MonoBehaviour
{
    [Header("References (Assign these in the Inspector)")]
    public Renderer tableauRenderer;      // Should be the Renderer component of your "tableau5" GameObject.
    public Renderer salleRenderer;        // Should be the Renderer component of your "salle" GameObject.
    public TextMeshPro loadingText;       // UI text element to indicate loading.
    public InputField userInput;

    private const string API_KEY = "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";
    private string outputDirectory;
    private Texture2D currentTexture;

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
            loadingText.text = "";
            loadingText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Loading Text is not assigned in the Inspector!");
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

        if (salleRenderer == null)
        {
            Debug.LogError("Tableau Renderer is not assigned!");
            yield break;
        }

        // Set loading text and disable button
        loadingText.text = "I am generating an Image. Please Wait...";

        string userPrompt = userInput.text;
        Debug.Log($"Generating image for prompt: {userPrompt}");

        var task = GenerateImageFromAPI(userPrompt);
        yield return new WaitUntil(() => task.IsCompleted);

        string imagePath = task.Result;
        if (!string.IsNullOrEmpty(imagePath))
        {
            Debug.Log("Image generated successfully.");
            ApplyTexture(imagePath);
            // Reset UI state
            loadingText.text = userPrompt;
        }
        else
        {
            loadingText.text = "Failed. Try Again.";
            StartCoroutine(ResetLoadingText());
        }
    }

    private IEnumerator ResetLoadingText()
    {
        yield return new WaitForSeconds(15);
        loadingText.text = "";
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
            StartCoroutine(ResetLoadingText());
            Debug.Log("Texture applied successfully.");
        }
        else
        {
            Debug.LogError("Tableau Renderer is not assigned!");
        }
        if (salleRenderer != null)
        {
            salleRenderer.material.mainTexture = currentTexture;
            StartCoroutine(ResetLoadingText());
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