using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudioTest : MonoBehaviour
{
    public AudioClip cliptoPlay;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hello, i am struggling");
        if (other.CompareTag("Player"))
            Vocals.instance.Say(cliptoPlay);
    }
}
