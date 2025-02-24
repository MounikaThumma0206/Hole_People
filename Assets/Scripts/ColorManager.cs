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
    
    public List<ColorMaterial> colorMaterials;

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

