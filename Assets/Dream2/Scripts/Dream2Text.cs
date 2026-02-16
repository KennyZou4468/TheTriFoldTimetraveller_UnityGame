using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dream2Text : MonoBehaviour
{
    [SerializeField] private Transform interactableTransform;    
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = interactableTransform.localScale;
    }
}
