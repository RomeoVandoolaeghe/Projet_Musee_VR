using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
    public float deadTime = 1.0f;
    private bool _deadTimeActive = false;

    public UnityEvent onPressed, onReleased;

    // Reference to the GenerateImage script
    public GenerateImage generateImageScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Button" && !_deadTimeActive)
        {
            onPressed?.Invoke();
            Debug.Log("I have been pressed");

            // Call the method to generate an image
            if (generateImageScript != null)
            {
                generateImageScript.GenerateImageOnButtonPress();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Button" && !_deadTimeActive)
        {
            onReleased?.Invoke();
            Debug.Log("I have been released");
            StartCoroutine(WaitForDeadTime());
        }
    }

    IEnumerator WaitForDeadTime()
    {
        _deadTimeActive = true;
        yield return new WaitForSeconds(deadTime);
        _deadTimeActive = false;
    }
}