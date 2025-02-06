using UnityEngine;

public class VoiceGuideManager : MonoBehaviour
{
    public AudioSource audioSource;  // L'AudioSource pour jouer l'instruction
    public AudioClip voixDEntree; // Fichier audio nommé "voix d'entrée"
    public float startDelay = 2f; // Délai avant de jouer l'audio

    void Start()
    {
        if (voixDEntree != null)
        {
            // Lancer l'instruction après un délai
            Invoke(nameof(PlayInstruction), startDelay);
        }
        else
        {
            Debug.LogWarning("Aucun fichier audio 'voix d'entrée' assigné !");
        }
    }

    void PlayInstruction()
    {
        audioSource.clip = voixDEntree;
        audioSource.Play();
        Debug.Log("Lecture de la voix d'entrée.");
    }
}
