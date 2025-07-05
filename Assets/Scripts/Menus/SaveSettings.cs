using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSettings : MonoBehaviour
{
    #region Singleton
    private static SaveSettings _instance;
    public static SaveSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindFirstObjectByType<SaveSettings>();
                if (_instance == null)
                {
                    Debug.LogError("Save settings Missed");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public float musicVolum = 10;
    public float fxVolum = 10;
}
