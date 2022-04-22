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
    }
}
