using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScreenShit : MonoBehaviour
{
    public float floatSpeed = 1f;  // Speed of floating
    public float floatHeight = 0.5f; // Maximum height difference
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Lerp(-floatHeight, floatHeight, (Mathf.Sin(Time.time * floatSpeed) + 1) / 2);
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}