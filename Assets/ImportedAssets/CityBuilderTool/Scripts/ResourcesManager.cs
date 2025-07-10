using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager :MonoBehaviour
{
    public List<ResourcesData> resourcesData;
}
[System.Serializable]
public struct ResourcesData
{
    public Texture2D Icon;
    public GameObject cyan,blue,green,grey,orange,Red,purple,white;
}