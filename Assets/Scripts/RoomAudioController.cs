using UnityEngine;
using System.Collections;

public class RoomAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip voixDEntree; // L'audio � jouer
    public float delayBeforeStart = 2.0f; // Temps d'attente avant le lancement (secondes)
    private Coroutine playAudioCoroutine;

    private void Start()
    {
        // R�cup�re ou ajoute un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configuration de l'AudioSource
        audioSource.clip = voixDEntree;
        audioSource.loop = true; // Boucle infinie
        audioSource.playOnAwake = false; // Ne joue pas au d�but
        audioSource.spatialBlend = 1.0f; // Son 3D pour immersion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Si le joueur entre
        {
            if (playAudioCoroutine == null) // �vite plusieurs coroutines
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

            audioSource.Stop(); // Stoppe imm�diatement le son
        }
    }

    private IEnumerator PlayAudioWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart); // Pause avant de d�marrer

        if (!audioSource.isPlaying) // Joue seulement si le son n'est pas d�j� en cours
        {
            audioSource.volume = 1.0f; // Volume fort
            audioSource.Play();
        }

        playAudioCoroutine = null; // Reset apr�s ex�cution
    }
}
