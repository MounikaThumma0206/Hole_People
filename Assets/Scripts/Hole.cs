//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Hole : MonoBehaviour
//{
//    public ColorEnum colorEnum;
//   // public PlayerGridGenerator playerGridGenerator;
//    private void OnMouseDown()
//    {
//        if (GameManager.Instance != null)
//        {
//            GameManager.Instance.IsPlayerMovable(colorEnum, gameObject);
//            //if (playerGridGenerator != null)
//            //{
//            //    Debug.Log("Hole clicked");
//            //    playerGridGenerator.movePlayerToHole(gameObject);
//            //}
//        }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public ColorEnum colorEnum;
    public PlayerGenerator playerGenerator; // Reference to PlayerGridGenerator

    private void OnMouseDown()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IsPlayerMovable(colorEnum, gameObject);

            // Use the reference to move the player to the hole if playerGridGenerator is assigned
            if (playerGenerator != null)
            {
                Debug.Log("Hole clicked");
                playerGenerator.movePlayerToHole(gameObject);
            }
            else
            {
                Debug.LogWarning("PlayerGridGenerator reference is missing in Hole.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Jump
        Debug.Log(other.gameObject.name);
       /* if (other.gameObject.CompareTag("Stickman"))
            other.gameObject.SetActive(false);*/
    }
}


