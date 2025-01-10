using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Transform Player;
    public Vector3 offset;
        // Start is called before the first frame update
    void LateUpdate()
    {
        if (Player != null)
        {
            transform.position = Player.position + offset;
        }
    }
}
