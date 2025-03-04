using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocals : MonoBehaviour
{
    public static Vocals instance;

    private AudioSource source;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    public void Say(AudioClip clip)
    {
        if (source.isPlaying)
            source.Stop();

        source.PlayOneShot(clip);
    }
}
