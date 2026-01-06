using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTrigger : MonoBehaviour
{
    public bool outOfRange = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Wall")
        {
            outOfRange = true;
        }
    }
}
