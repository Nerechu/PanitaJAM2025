using UnityEngine;
using UnityEngine.Audio;

public class FloatingBeacon : MonoBehaviour
{
    [Header("Movimiento Vertical")]
    public float floatAmplitude = 1f;
    public float floatFrequency = 1f;

    [Header("Hélice")]
    public Transform helice;
    public float heliceSpeed = 360f;

    [Header("Sonido")]
    public AudioSource heliceAudioSource;
    public AudioMixer audioMixer;
    [Tooltip("Nombre del parámetro expuesto en el Audio Mixer")]
    public string mixerVolumeParam = "BeaconVolume";

    [Header("Gancho")]
    private bool isGrappled = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        // Asegura que el AudioSource está bien configurado
        if (heliceAudioSource != null)
        {
            heliceAudioSource.loop = true;
            heliceAudioSource.playOnAwake = false;
            heliceAudioSource.Play();
        }
    }

    void Update()
    {
        if (!isGrappled)
        {
            // Movimiento vertical senoidal
            float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = startPosition + new Vector3(0, newY, 0);

            // Rotación de la hélice
            if (helice != null)
                helice.Rotate(Vector3.up * heliceSpeed * Time.deltaTime);

            // Reanudar sonido si está pausado
            if (heliceAudioSource != null && !heliceAudioSource.isPlaying)
                heliceAudioSource.Play();
        }
        else
        {
            // Pausar sonido al enganchar
            if (heliceAudioSource != null && heliceAudioSource.isPlaying)
                heliceAudioSource.Pause();
        }
    }

    // Llama esto desde el gancho cuando se enganche
    public void OnGrappled()
    {
        isGrappled = true;
        startPosition = transform.position;

        // También podrías reducir volumen del mixer si prefieres en lugar de pausar
        // audioMixer.SetFloat(mixerVolumeParam, -80f); // Silencio total
    }

    // Opcional: permite volver a activar el beacon
    public void ResetBeacon()
    {
        isGrappled = false;
        startPosition = transform.position;

        if (heliceAudioSource != null && !heliceAudioSource.isPlaying)
            heliceAudioSource.Play();

        // audioMixer.SetFloat(mixerVolumeParam, 0f); // Restaurar volumen si usaste SetFloat
    }

    // Opcional: cambiar volumen desde otro script
    public void SetVolume(float normalizedVolume)
    {
        // normalizedVolume: entre 0.0001 y 1
        if (audioMixer != null)
            audioMixer.SetFloat(mixerVolumeParam, Mathf.Log10(Mathf.Clamp(normalizedVolume, 0.0001f, 1f)) * 20f);
    }
}
