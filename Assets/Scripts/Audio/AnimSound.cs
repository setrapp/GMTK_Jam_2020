using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSound : MonoBehaviour
{
    AudioManager audioManager;

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void PlayHighlightSound()
    {
        audioManager.PlayHighlightAudio();
    }
    public void PlaySelectedSound()
    {
        audioManager.PlaySelectedAudio();
    }
    public void PlaySwapSound()
    {
        audioManager.PlaySwapAudio();
    }
}
