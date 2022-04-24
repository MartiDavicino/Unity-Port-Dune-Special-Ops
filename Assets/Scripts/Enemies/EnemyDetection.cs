using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DecState
{
	STILL,
	SEEKING,
	FOUND,
	NONE
}

public class EnemyDetection : MonoBehaviour
{


	public float viewRadius;
	public float hearingRadius;
	private CharacterBaseBehavior baseScript;
	public float timer = 0.0f;
	public float delay = 3.0f;
	public float proportion = 0.2f;
	public DecState state = DecState.STILL;

	[Range(0, 360)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	public List<Transform> visibleTargets = new List<Transform>();
	public List<Transform> noisyTargets = new List<Transform>();
	[HideInInspector]

	void Start()
	{
		StartCoroutine("FindTargetsWithDelay", .2f);
	}


	IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
			FindNoisyTargets();
		}
	}

	void FindVisibleTargets()
	{
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
	

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);

				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask) )
				{
					StartCoroutine(WaitAndAddToList(delay, target,visibleTargets));
				}
			}
		}
	}

	IEnumerator WaitAndAddToList(float delay,Transform target,List<Transform>targets)
    {
		state = DecState.SEEKING;
		while (timer < delay)
        {
			timer += proportion;
			yield return new WaitForEndOfFrame();
        }

		targets.Add(target);
		state = DecState.FOUND;
	}

	void FindNoisyTargets()
	{
		noisyTargets.Clear();
		Collider[] targetsInHearingRadius = Physics.OverlapSphere(transform.position, hearingRadius, targetMask);

		for (int i = 0; i < targetsInHearingRadius.Length; i++)
		{
			GameObject parent = targetsInHearingRadius[i].gameObject;
			Transform target = targetsInHearingRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			baseScript = parent.GetComponent<CharacterBaseBehavior>();

			float dstToTarget = Vector3.Distance(transform.position, target.position);

			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, targetMask)&& baseScript.state != PlayerState.CROUCH)
			{
				StartCoroutine(WaitAndAddToList(delay, target, noisyTargets));
			}

		}
	}


	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
