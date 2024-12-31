using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public ColorEnum colorEnum;
    public PlayerGridGenerator playerGridGenerator;
    private void OnMouseDown()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IsPlayerMovable(colorEnum, gameObject);
            //if (playerGridGenerator != null)
            //{
            //    Debug.Log("Hole clicked");
            //    playerGridGenerator.movePlayerToHole(gameObject);
            //}
        }
    }
}


