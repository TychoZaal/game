using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomeAnimation : MonoBehaviour
{
    public Animator animator;

    public IEnumerator Play()
    {
        animator.SetTrigger("Play");
        yield return new WaitForSeconds(3.0f);
        Vector3 r = transform.localEulerAngles;
        this.transform.localEulerAngles = new Vector3(r.x, Random.Range(0, 360), r.z);
    }
}
