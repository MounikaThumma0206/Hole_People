using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorMaterial
{
    public ColorEnum colorEnum;
    public Material material;
}
[CreateAssetMenu(fileName = "ColorManager", menuName = "ScriptableObjects/ColorManager", order = 1)]
public class ColorManager : ScriptableObject
{
    // List of ColorMaterial, which contains the enum and its corresponding material
    public List<ColorMaterial> colorMaterials;

    // Method to get the material based on color type
 
}
