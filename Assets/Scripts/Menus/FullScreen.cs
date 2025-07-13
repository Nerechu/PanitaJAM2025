using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
using TMPro;
//

public class FullScreen : MonoBehaviour
{
    public Toggle toggle;

    //
    public TMP_Dropdown resolucionesDropDown;
    Resolution[] resoluciones;
    //

    private void Awake()
    {
        //Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    void Start()
    {
        if (Screen.fullScreen)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        //
        RevisarResolucion();
        //
    }


    void Update()
    {

    }

    public void ActivarFullScreen(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;

    }

    //
    public void RevisarResolucion()
    {
        resoluciones = Screen.resolutions;
        resolucionesDropDown.ClearOptions();
        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);


            if (Screen.fullScreen && resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }

        }

        resolucionesDropDown.AddOptions(opciones);
        resolucionesDropDown.value = resolucionActual;
        resolucionesDropDown.RefreshShownValue();


        //
        //resolucionesDropDown.value = PlayerPrefs.GetInt("numeroResolucion", 9);
        //

        CambiarResolucion(resoluciones.Length - 1);
    }

    public void CambiarResolucion(int indiceResolucion)
    {
        //
        PlayerPrefs.SetInt("numeroResolucion", resoluciones.Length-1);
        //


        Resolution resolution = resoluciones[indiceResolucion];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    //
}