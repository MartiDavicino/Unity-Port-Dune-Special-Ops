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
	public float sightMultiplier;
	public float distanceMultiplier;
	public float maxDistanceMultiplier;
	public float maxMultiplier = 5.0f;
	[HideInInspector] public float multiplierHolder;
	public EnemyBehaviour data;

	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;

	public float hearingRadius;

	private CharacterBaseBehavior baseScript;

	[HideInInspector] public float timer = 0.0f;
	public float secondsToDetect;
	private float proportion;

	[HideInInspector] public DecState state = DecState.STILL;
	[HideInInspector] public bool debug = false;

	public VisualDebug visual;
	public hearingDebug hear;

	private Vector3 angle1;
	private Vector3 angle2;
		
	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
	[HideInInspector] public List<Transform> noisyTargets = new List<Transform>();
    void Start()
    {
		multiplierHolder = sightMultiplier;
		proportion = 1f;

		distanceMultiplier = 1f;
		maxDistanceMultiplier = 1f;

	}
    void Update()
    {
		angle1 = DirFromAngle(-viewAngle / 2, false);
		angle2 = DirFromAngle(viewAngle / 2, false);
		FindTargetsWithDelay();

		if(debug)
        {
			hear.DebugDraw();
			visual.DebugDraw();
		}
		else if(!debug)
        {
			hear.DebugDelete();
			visual.DebugDelete();

		}
		if (Input.GetKeyDown(KeyCode.F10))
			debug = !debug;
		
	}
	
	void CalculateMultiplier()
    {
		
		distanceMultiplier = viewRadius * maxDistanceMultiplier / CalculateAbsoluteDistance(data.player.transform.position).magnitude;

    }

    void FindTargetsWithDelay()
	{
		bool playerInView = FindVisibleTargets();
		bool playerHeard = FindNoisyTargets();

		if (!playerInView && !playerHeard)
			TargetsNotFound();
	}
	void TargetsNotFound()
	{
		if (timer > 0 && timer < secondsToDetect / 2)
		{
			state = DecState.STILL;
			timer -= proportion * Time.deltaTime;
		} else if (timer >= timer / 2 && timer <= secondsToDetect)
		{
			state = DecState.SEEKING;
			timer -= proportion * Time.deltaTime;
		} else if (timer < 0f)
        {
			timer = 0f;
			state = DecState.STILL;
		}
	}
	void WaitAndAddToList(float delay,Transform target,List<Transform>targets)
    {
		timer += proportion  * Time.deltaTime;


		if (timer > 0 && timer < secondsToDetect / 2)
		{
			state = DecState.STILL;
		}
		else if (timer >= timer / 2 && timer < secondsToDetect)
		{
			state = DecState.SEEKING;
		}

		if (timer >= delay)
        {
			timer = delay;
			targets.Add(target);
			state = DecState.FOUND;
        }


	}


	bool FindVisibleTargets()
	{
		bool playerInView = false;
		
		sightMultiplier = 1f;

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
					CharacterBaseBehavior cB = targetsInViewRadius[i].gameObject.GetComponent<CharacterBaseBehavior>();
					if(!cB.invisible)
                    {
						sightMultiplier = multiplierHolder;
						WaitAndAddToList(secondsToDetect, target,visibleTargets);
						playerInView = true;
                    }
				}
			}
		}

		return playerInView;
	}

	bool FindNoisyTargets()
	{
		bool playerHeard = false;

		sightMultiplier = 1f;

		noisyTargets.Clear();
		Collider[] targetsInHearingRadius = Physics.OverlapSphere(transform.position, hearingRadius, targetMask);

		if (targetsInHearingRadius.Length > 0)
			playerHeard = true;

		for (int i = 0; i < targetsInHearingRadius.Length; i++)
		{
			GameObject parent = targetsInHearingRadius[i].gameObject;
			Transform target = targetsInHearingRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			baseScript = parent.GetComponent<CharacterBaseBehavior>();

			float dstToTarget = Vector3.Distance(transform.position, target.position);

			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, targetMask)&& baseScript.state != PlayerState.CROUCH)
			{
				WaitAndAddToList(secondsToDetect, target, noisyTargets);
			} 
		}

		return playerHeard;
	}
	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	Vector3 CalculateAbsoluteDistance(Vector3 targetPos)
	{
		Vector3 distance = new Vector3(0f, 0f, 0f);

		distance.x = Mathf.Abs(transform.position.x - targetPos.x);
		distance.z = Mathf.Abs(transform.position.z - targetPos.z);

		return distance;
	}

}

