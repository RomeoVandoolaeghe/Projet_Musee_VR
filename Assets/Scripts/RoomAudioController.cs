using UnityEngine;
using System.Collections;

public class RoomAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip voixDEntree; // L'audio à jouer
    public float delayBeforeStart = 2.0f; // Temps d'attente avant le lancement (secondes)
    private Coroutine playAudioCoroutine;

    private void Start()
    {
        // Récupère ou ajoute un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configuration de l'AudioSource
        audioSource.clip = voixDEntree;
        audioSource.loop = true; // Boucle infinie
        audioSource.playOnAwake = false; // Ne joue pas au début
        audioSource.spatialBlend = 1.0f; // Son 3D pour immersion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Si le joueur entre
        {
            if (playAudioCoroutine == null) // Évite plusieurs coroutines
            {
                playAudioCoroutine = StartCoroutine(PlayAudioWithDelay());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Si le joueur sort
        {
            if (playAudioCoroutine != null)
            {
                StopCoroutine(playAudioCoroutine);
                playAudioCoroutine = null;
            }

            audioSource.Stop(); // Stoppe immédiatement le son
        }
    }

    private IEnumerator PlayAudioWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart); // Pause avant de démarrer

        if (!audioSource.isPlaying) // Joue seulement si le son n'est pas déjà en cours
        {
            audioSource.volume = 1.0f; // Volume fort
            audioSource.Play();
        }

        playAudioCoroutine = null; // Reset après exécution
    }
}
