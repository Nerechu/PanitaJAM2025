using System.Collections.Generic;
using UnityEngine;

public class MeshGroup
{
    public string materialName;
    public Color color;
    public Color colorEnd;
    public List<Mesh> meshes;
    public List<Transform> transforms;

    public MeshGroup(string materialName, Color color, Color colorEnd)
    {
        this.materialName = materialName;
        this.color = color;
        this.colorEnd = colorEnd;
        meshes = new List<Mesh>();
        transforms = new List<Transform>();
    }
}
