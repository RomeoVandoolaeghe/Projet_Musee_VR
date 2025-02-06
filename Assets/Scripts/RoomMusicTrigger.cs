using UnityEngine;
using System.Collections;

public class RoomMusicTrigger : MonoBehaviour
{
    public AudioSource roomMusic;
    public float delayBeforePlaying = 2f; // Temps de pause en secondes

    private bool hasEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEntered)
        {
            hasEntered = true; // Empêche de rejouer plusieurs fois la musique
            StartCoroutine(PlayMusicWithDelay());
        }
    }

    private IEnumerator PlayMusicWithDelay()
    {
        Debug.Log($"Attente de {delayBeforePlaying} secondes avant de jouer la musique.");
        yield return new WaitForSeconds(delayBeforePlaying);

        if (roomMusic != null)
        {
            roomMusic.Play();
            Debug.Log("Musique de la pièce déclenchée.");
        }
    }
}
