using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animStates : MonoBehaviour
{
    private Animator animator;

    CharacterBaseBehavior baseScript;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = gameObject.GetComponent<CharacterBaseBehavior>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.state == PlayerState.IDLE)
        {
            animator.ResetTrigger("isWalking");
            animator.ResetTrigger("isCrouching");

            animator.SetTrigger("hasStopped");
        }

        if (baseScript.state == PlayerState.WALKING)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isCrouching");

            animator.SetTrigger("isWalking");
        }

        if (baseScript.state == PlayerState.CROUCH)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isWalking");

            animator.SetTrigger("isCrouching");
        }

        if (baseScript.state == PlayerState.STEALTH_KILL)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isWalking");
            animator.ResetTrigger("isCrouching");

            animator.SetTrigger("sneakyKill");
        }

        if (baseScript.state == PlayerState.ABILITY1)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isWalking");
            animator.ResetTrigger("isCrouching");

            animator.SetTrigger("ability1");
        }

        if (baseScript.state == PlayerState.ABILITY2)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isWalking");
            animator.ResetTrigger("isCrouching");

            animator.SetTrigger("ability2");
        }

        if (baseScript.state == PlayerState.ABILITY3)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isWalking");
            animator.ResetTrigger("isCrouching");

            animator.SetTrigger("ability3");
        }

        if (baseScript.state == PlayerState.ABILITY3_1)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isWalking");
            animator.ResetTrigger("isCrouching");
            animator.ResetTrigger("ability3");

            animator.SetTrigger("ability3.1");
        }
    }
}
