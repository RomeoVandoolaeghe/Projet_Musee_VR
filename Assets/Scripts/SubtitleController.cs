using UnityEngine;
using TMPro;

public class SubtitleController : MonoBehaviour
{
    // Reference to the Subtitle Object
    public GameObject subtitleObject;

    // Reference to the TextMeshPro component (SuWen uses TMP for subtitles)
    private TextMeshProUGUI subtitleText;

    // Duration for showing the subtitle
    public float subtitleDuration = 5f;

    private void Start()
    {
        // Find the TextMeshProUGUI component from the subtitle object
        if (subtitleObject != null)
        {
            subtitleText = subtitleObject.GetComponentInChildren<TextMeshProUGUI>();
            subtitleObject.SetActive(false); // Hide initially
        }
    }

    // Method to show subtitle with custom text
    public void ShowSubtitle(string text)
    {
        if (subtitleText != null)
        {
            subtitleText.text = text;
            subtitleObject.SetActive(true);
            Invoke(nameof(HideSubtitle), subtitleDuration);
        }
    }

    // Method to hide subtitle
    private void HideSubtitle()
    {
        if (subtitleObject != null)
        {
            subtitleObject.SetActive(false);
        }
    }
}
