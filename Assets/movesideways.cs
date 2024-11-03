using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movesideways : MonoBehaviour
{
    public Transform left, right;
    public float minSpeed, maxSpeed, speed;
    private int direction = 1;

    private void Start()
    {
        direction = Random.Range(0, 10) > 5f ? direction * -1 : direction;
        speed = Random.Range(minSpeed, maxSpeed); 
    }

    private void Update()
    {
        if (transform.position.x < left.transform.position.x)
        {
            transform.position = new Vector3(right.transform.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > right.transform.position.x)
        {
            transform.position = new Vector3(left.transform.position.x, transform.position.y, transform.position.z);

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(speed * direction, 0, 0);
    }
}
