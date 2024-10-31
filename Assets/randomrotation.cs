using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomrotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 r = transform.localEulerAngles;
        this.transform.localEulerAngles = new Vector3(Random.Range(-4, 4), r.y, r.z);
    }
}
