using UnityEngine;

public class VoiceGuideManager : MonoBehaviour
{
    public AudioSource audioSource;  // L'AudioSource pour jouer l'instruction
    public AudioClip voixDEntree; // Fichier audio nomme "voix d'entree"
    public float startDelay = 2f; // D�lai avant de jouer l'audio

    void Start()
    {
        if (voixDEntree != null)
        {
            // Lancer l'instruction apres un delai
            Invoke(nameof(PlayInstruction), startDelay);
        }
        else
        {
            Debug.LogWarning("Aucun fichier audio 'voix d'entree' assigne !");
        }
    }

    void PlayInstruction()
    {
        audioSource.clip = voixDEntree;
        audioSource.Play();
        Debug.Log("Lecture de la voix d'entree.");
    }
}
