using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    private AudioSource m_cameraSource;

    [SerializeField] private AudioClip m_music;

    // Start is called before the first frame update
    void Start()
    {
        m_cameraSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_cameraSource.isPlaying)
        {
            m_cameraSource.PlayOneShot(m_music);
            m_cameraSource.volume = 0.8f;
        }
    }
}
