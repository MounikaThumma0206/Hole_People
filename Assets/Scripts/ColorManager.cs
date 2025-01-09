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
    //public Material GetColor(ColorEnum colorEnum)
    //{
    //    foreach (var colorMaterial in colorMaterials)
    //    {
    //        if (colorMaterial.colorEnum == colorEnum)
    //        {
    //            return colorMaterial.material;
    //        }
    //    }

    //    Debug.LogWarning($"Material for color {colorEnum} not found!");
    //    return null; // Return null if no material is found
    //}
    public Color GetColor(ColorEnum colorEnum)
    {
        foreach (var colorMaterial in colorMaterials)
        {
            if (colorMaterial.colorEnum == colorEnum)
            {
                if (colorMaterial.material.HasProperty("_Color"))
                {
                    return colorMaterial.material.color;
                }
                else
                {
                    Debug.LogWarning($"Material for color {colorEnum} does not have a _Color property!");
                    return Color.white; // Default color if _Color is not found
                }
            }
        }

        Debug.LogWarning($"Material for color {colorEnum} not found!");
        return Color.white; // Default color if no material is found
    }

}

