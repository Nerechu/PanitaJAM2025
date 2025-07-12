using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    [SerializeField] private int targetFPS = 60;

    private void Awake()
    {
        // Desactiva VSync (de lo contrario Application.targetFrameRate no hace efecto)
        QualitySettings.vSyncCount = 0;

        // Aplica el límite de FPS
        Application.targetFrameRate = targetFPS;

        // (Opcional) Mantener este objeto entre escenas
        // DontDestroyOnLoad(gameObject);
    }

    // (Opcional) Mostrar los FPS actuales en pantalla
    /*
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"FPS: {(1f / Time.deltaTime):F0}");
    }
    */
}
