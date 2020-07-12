using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource hightlightAudio;
    [SerializeField]
    private AudioClip highlightClip;
    AudioSource selectedAudio;
    [SerializeField]
    private AudioClip SelectedClip;
    AudioSource swapAudio;
    [SerializeField]
    private AudioClip SwapedClip;

    private void Start()
    {
        swapAudio = transform.Find("SwapAudio").GetComponent<AudioSource>();
        hightlightAudio = transform.Find("HighlightAudio").GetComponent<AudioSource>();
        selectedAudio = transform.Find("SelectedAudio").GetComponent<AudioSource>();
    }
    public void PlayHighlightAudio()
    {
        float randPitch = Random.Range(10, 20) * 0.2f;
        hightlightAudio.pitch = randPitch;
        hightlightAudio.PlayOneShot(highlightClip);
    }
    public void PlaySelectedAudio()
    {
            float randPitch = Random.Range(0.9f, 1.2f);
        selectedAudio.pitch = randPitch;
        selectedAudio.PlayOneShot(SelectedClip);
    }
    public void PlaySwapAudio()
    {
      //  float randPitch = Random.Range(0.9f, 1.2f);
      //  swapAudio.pitch = randPitch;
        swapAudio.PlayOneShot(SwapedClip);
    }

    private void Update()
    {
    //    float xPos = Mathf.InverseLerp(0.0f, Screen.width, Input.mousePosition.x);
    //    float yPos = Mathf.InverseLerp(0.0f, Screen.height, Input.mousePosition.y);

     //   hightlightAudio.pitch = xPos + yPos + 1;
    }
}
