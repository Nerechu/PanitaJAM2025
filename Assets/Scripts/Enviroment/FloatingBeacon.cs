using UnityEngine;
using UnityEngine.Audio;

public class FloatingBeacon : MonoBehaviour
{
    [Header("Movimiento Vertical")]
    public float floatAmplitude = 1f;
    public float floatFrequency = 1f;

    [Header("Descenso por gancho")]
    public float descendOffset = 0.3f;
    public float descendSpeed = 2f;
    public float ascendSpeed = 1.5f;

    [Header("Hélice")]
    public Transform helice;
    public float heliceSpeed = 360f;

    [Header("Sonido")]
    public AudioSource heliceAudioSource;
    public AudioMixer audioMixer;
    [Tooltip("Nombre del parámetro expuesto en el Audio Mixer")]
    public string mixerVolumeParam = "BeaconVolume";

    private bool isGrappled = false;
    private bool isReturning = false;

    private Vector3 basePosition;
    private Vector3 grappledPosition;

    void Start()
    {
        basePosition = transform.position;

        if (heliceAudioSource != null)
        {
            heliceAudioSource.loop = true;
            heliceAudioSource.playOnAwake = false;
            heliceAudioSource.Play();
        }
    }

    void Update()
    {
        // Movimiento vertical normal si no está enganchado ni regresando
        if (!isGrappled && !isReturning)
        {
            float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = basePosition + new Vector3(0, newY, 0);
        }

        // Hélice gira siempre
        if (helice != null)
            helice.Rotate(Vector3.up * heliceSpeed * Time.deltaTime);

        // Transición hacia abajo al engancharse
        if (isGrappled)
        {
            transform.position = Vector3.Lerp(transform.position, grappledPosition, Time.deltaTime * descendSpeed);
        }

        // Subida suave cuando se desengancha
        if (isReturning)
        {
            transform.position = Vector3.Lerp(transform.position, basePosition, Time.deltaTime * ascendSpeed);

            // Cuando está suficientemente cerca de basePosition, terminamos el retorno
            if (Vector3.Distance(transform.position, basePosition) < 0.01f)
            {
                transform.position = basePosition;
                isReturning = false;
            }
        }

        // Asegura que el audio sigue sonando
        if (heliceAudioSource != null && !heliceAudioSource.isPlaying)
            heliceAudioSource.Play();
    }

    // Llama esto desde el gancho al engancharse
    public void OnGrappled()
    {
        isGrappled = true;
        isReturning = false;

        // Guarda posición actual como base (por si el beacon se movió)
        basePosition = transform.position;

        // Calcula posición descendida
        grappledPosition = basePosition + Vector3.down * descendOffset;
    }

    // Llama esto desde el gancho al soltarse
    public void ResetBeacon()
    {
        isGrappled = false;
        isReturning = true;
    }

    // Para cambiar el volumen desde otro script (opcional)
    public void SetVolume(float normalizedVolume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat(mixerVolumeParam, Mathf.Log10(Mathf.Clamp(normalizedVolume, 0.0001f, 1f)) * 20f);
    }
}
