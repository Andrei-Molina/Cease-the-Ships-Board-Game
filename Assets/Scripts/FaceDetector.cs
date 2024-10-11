using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDetector : MonoBehaviour
{
    DiceRoll dice;

    private void OnTriggerStay(Collider other)
    {
        dice = FindObjectOfType<DiceRoll>();
        if (dice != null && dice.GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            // Safely try to parse the collider's name into an integer
            if (int.TryParse(other.name, out int faceValue))
            {
                // Successfully parsed the dice face value
                dice.diceFaceNum = faceValue;
            }
        }
    }
}
