using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    AudioSource hightlightAudio;
    [SerializeField]
    private AudioClip highlightClip;
    AudioSource selectedAudio;
    [SerializeField]
    private AudioClip SelectedClip;
    AudioSource swapAudio;
    [SerializeField]
    private AudioClip SwapedClip;

    AudioSource beat;

    AudioSource aNote;
    [SerializeField]
    private AudioClip ANote;

    [SerializeField]
    List<AudioSource> audios = new List<AudioSource>();
    int AudioNumber;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddNewAduio();
//        Debug.Log("OnSceneLoaded: " + scene.name);
//        Debug.Log(mode);
    }

    void AddNewAduio()
    {
        if (AudioNumber < audios.Count)
        {
            if (!audios[AudioNumber].isPlaying)
                audios[AudioNumber].Play();

            AudioNumber++;
            if (AudioNumber == 6)
            {
                StartCoroutine(FadeAudio(audios[0]));
            }
        }
    }

    IEnumerator FadeAudio(AudioSource audios)
    {
        float time = 1;
        while (time > 0)
        {
            audios.volume = time;
            time -= 0.05f;
            yield return null;
        }
        audios.Stop();
    }
    private void Start()
    {
        aNote = transform.Find("ANote").GetComponent<AudioSource>();

        beat = transform.Find("beat").GetComponent<AudioSource>();
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
    public void PlayBeat()
    {
        if (!beat.isPlaying)
        {
            StartCoroutine(beatLength());
//            if(beat.pitch < 1.5f)
//            beat.pitch += 0.1f;
        }
    }
    IEnumerator beatLength()
    {
        float time = 1;
        beat.volume = 1;
        beat.Play();

        while (time > 0)
        {
            if (beat.volume > 0)
                beat.volume = time;

            time -= 0.01f;
            yield return null;
        }
        beat.Stop();
    }
    private void Update()
    {
    //    float xPos = Mathf.InverseLerp(0.0f, Screen.width, Input.mousePosition.x);
    //    float yPos = Mathf.InverseLerp(0.0f, Screen.height, Input.mousePosition.y);

     //   hightlightAudio.pitch = xPos + yPos + 1;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
