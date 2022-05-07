using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animStatesSadiq : MonoBehaviour
{
    private Animator animator;

    SadiqBehaviour baseScript;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = gameObject.GetComponent<SadiqBehaviour>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.state == SadiqState.IDLE)
        {
            animator.ResetTrigger("isDevouring");
            animator.ResetTrigger("backToGround");
            animator.ResetTrigger("outOfGround");
            animator.ResetTrigger("isDevouring");
            animator.ResetTrigger("isSpitting");

            animator.SetTrigger("isIdle");
        }

        if (baseScript.state == SadiqState.COMINGOUT)
        {
            animator.ResetTrigger("isIdle");

            animator.SetTrigger("outOfGround");
        }

        if (baseScript.state == SadiqState.GETTINGIN)
        {
            animator.ResetTrigger("isIdle");

            animator.SetTrigger("backToGround");
        }

        if (baseScript.state == SadiqState.DEVOURING)
        {
            animator.ResetTrigger("isIdle");

            animator.SetTrigger("isDevouring");
        }

        if (baseScript.state == SadiqState.SPITTING)
        {
            animator.ResetTrigger("isIdle");
            animator.ResetTrigger("isDevouring");
            animator.ResetTrigger("backToGround");


            animator.SetTrigger("isSpitting");
        }
    }
}
