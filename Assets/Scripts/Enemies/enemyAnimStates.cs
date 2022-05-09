using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAnimStates : MonoBehaviour
{
    private Animator animator;

    EnemyBehaviour baseScript;

    // Start is called before the first frame update
    void Start()
    {
        baseScript = gameObject.GetComponent<EnemyBehaviour>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseScript.state == EnemyState.IDLE)
        {
            animator.ResetTrigger("isWalking");
            animator.ResetTrigger("isAttacking");

            animator.SetTrigger("hasStopped");
        }

        if (baseScript.state == EnemyState.WALKING)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isAttacking");
            animator.ResetTrigger("isWalking");

            animator.SetTrigger("isWalking");
        }

        if (baseScript.state == EnemyState.ATTACKING)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isWalking");

            animator.SetTrigger("isAttacking");
        }

        if (baseScript.state == EnemyState.RANGED)
        {
            animator.ResetTrigger("hasStopped");
            animator.ResetTrigger("isWalking");

            animator.SetTrigger("rangeAttack");
        }
    }
}
