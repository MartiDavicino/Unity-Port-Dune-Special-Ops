using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
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
	private float playerStateMultipler;
	[HideInInspector] public float multiplierHolder;

	public float debuffTime;
	private float debuffTimer;
	private Material materialHolder;
	private Transform child;
	[HideInInspector] public float debuffMultiplier;
	[HideInInspector] public bool debuffed;

	public EnemyBehaviour data;

	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;

	public float hearingRadius;

	private CharacterBaseBehavior baseScript;

	private Camera generalCamera;
	private GameObject player;

	[HideInInspector] public float timer = 0.0f;
	public float secondsToDetect;
	private float proportion;

	public DecState state = DecState.STILL;
	[HideInInspector] public bool debug = false;


	public VisualDebug visual;
	public hearingDebug hear;

	private Vector3 angle1;
	private Vector3 angle2;
		
	public LayerMask targetMask;
	public LayerMask obstacleMask;
	public LayerMask whatIsHunterSeeker;

	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
	[HideInInspector] public List<Transform> noisyTargets = new List<Transform>();

	private float elapse_time = 0;

	void Start()
    {
		generalCamera = Camera.main;
		multiplierHolder = sightMultiplier;
		proportion = 1f;

		distanceMultiplier = 1f;
		maxDistanceMultiplier = 1f;

		debuffed = false;
		debuffTimer = 0f;


		string[] splitArray = name.Split(char.Parse(" "));
		string[] splitArray2 = splitArray[0].Split(char.Parse("("));
		string finalName = splitArray2[0];

		child = transform.Find(finalName + "_low");
		materialHolder = child.GetComponent<Renderer>().material;
	}
    void Update()
    {
		player = generalCamera.GetComponentInChildren<CameraMovement>().focusedPlayer;
		
		if(player != null)
        {
			angle1 = DirFromAngle(-viewAngle / 2, false);
			angle2 = DirFromAngle(viewAngle / 2, false);

			string[] splitArray = player.name.Split(char.Parse(" "));
			string[] splitArray2 = splitArray[0].Split(char.Parse("("));
			string finalName = splitArray2[0];

			if (finalName != "HunterSeeker")
			{
				CharacterBaseBehavior playerScript = player.GetComponent<CharacterBaseBehavior>();
				if (playerScript.state == PlayerState.WALKING)
				{
					hearingRadius = playerScript.walkSoundRange;
				}

				else if (playerScript.state == PlayerState.CROUCH)
				{
					hearingRadius = playerScript.crouchSoundRange;
				}

				else if (playerScript.state == PlayerState.RUNNING)
				{
					hearingRadius = playerScript.runSoundRange;
				}
			
				playerStateMultipler = playerScript.detectionMultiplier;
			} else
            {
				HunterSeeker hunterSeekerScript = player.GetComponent<HunterSeeker>();
				hearingRadius = hunterSeekerScript.baseScript.soundRange;

				playerStateMultipler = hunterSeekerScript.baseScript.soundMultiplier;
			}
		}

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


		if (!debuffed)
		{
			debuffMultiplier = 1f;

			child.GetComponent<Renderer>().material = materialHolder;
		}
		else
		{
			string[] splitArray = name.Split(char.Parse(" "));
			string[] splitArray2 = splitArray[0].Split(char.Parse("("));
			string finalName = splitArray2[0];

			child.GetComponent<Renderer>().material = Resources.Load(finalName + "sleep", typeof(Material)) as Material;

			while (debuffTimer < debuffTime)
			{
				debuffTimer += Time.deltaTime;
				return;
			}

			debuffTimer = 0;
			debuffed = false;
		}
	}

    private void LateUpdate()
    {
		visibleTargets.Clear();
		noisyTargets.Clear();
	}
	void CalculateMultiplier()
    {
		
		if(data.player != null)
        {
			distanceMultiplier = viewRadius * maxDistanceMultiplier / CalculateAbsoluteDistance(data.player.transform.position).magnitude;
			if (distanceMultiplier > maxMultiplier)
				distanceMultiplier = maxMultiplier;
        }
    }

    void FindTargetsWithDelay()
	{
		bool playerInView = FindVisibleTargets();
		bool playerHeard = FindNoisyTargets();
		bool mosquitoHeard = FindHunterSeeker();


		if (!playerInView && !playerHeard && !mosquitoHeard)
			TargetsNotFound();
	}
	void TargetsNotFound()
	{
		timer -= proportion * Time.deltaTime;

		if(state == DecState.FOUND)
			state = DecState.SEEKING;

		if(state == DecState.SEEKING && timer < 0)
        {
			timer = secondsToDetect;
			state = DecState.STILL;
        }

		if(state == DecState.STILL && timer < 0)
        {
			timer = secondsToDetect;
			state = DecState.STILL;
			timer = 0f;
		}
	}
	void WaitAndAddToList(float delay,Transform target, string targetType)
    {
		timer += proportion * sightMultiplier * playerStateMultipler * debuffMultiplier /** distanceMultiplier*/ * Time.deltaTime;

		if (timer > 0 && state == DecState.STILL)
		{
			state = DecState.STILL;

			if(timer >= secondsToDetect)
            {
				state = DecState.SEEKING;
				timer = 0;
            }
		}

		if (timer > 0 && state == DecState.SEEKING)
		{
			state = DecState.SEEKING;
			elapse_time = 0;

			if (timer >= secondsToDetect)
			{
				state = DecState.FOUND;
			}
		}

		if (timer >= secondsToDetect && state == DecState.FOUND)
        {
			if(target.gameObject == GameObject.Find("HunterSeeker(Clone)"))
            {
				timer = delay;
				while (elapse_time < 0.6f)
				{
					elapse_time += Time.deltaTime;
					return;
				}

				elapse_time = 0;
				HunterSeeker hunterScript = target.gameObject.GetComponent<HunterSeeker>();
				hunterScript.DisableHunterSeeker();
			}
			else
            {
				timer = delay;
				if(targetType == "noisy") noisyTargets.Add(target);
				if(targetType == "visible") visibleTargets.Add(target);

            }
        }


	}


	bool FindVisibleTargets()
	{
		bool playerInView = false;
		
		sightMultiplier = 1f;

		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{

			if(targetsInViewRadius[i].gameObject == GameObject.Find("HunterSeeker(Clone)"))
            	return playerInView;

			CalculateMultiplier();

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
						WaitAndAddToList(secondsToDetect, target, "visible");
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

        Collider[] targetsInHearingRadius = Physics.OverlapSphere(transform.position, hearingRadius, targetMask);

		for (int i = 0; i < targetsInHearingRadius.Length; i++)
		{
			GameObject parent = targetsInHearingRadius[i].gameObject;
			Transform target = targetsInHearingRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			
			CalculateMultiplier();

			baseScript = parent.GetComponent<CharacterBaseBehavior>();
			NavMeshAgent pAgent = baseScript.GetComponent<NavMeshAgent>();

			float dstToTarget = Vector3.Distance(transform.position, target.position);

			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, targetMask))
			{
				//Waiting for still behaviour when crouching 

				//if((pAgent.remainingDistance > 0 && !pAgent.pathPending))
                //{
					WaitAndAddToList(secondsToDetect, target, "noisy");
					playerHeard = true;
                //}
			}
		}

		return playerHeard;
	}

	bool FindHunterSeeker()
	{
		bool playerHeard = false;

		sightMultiplier = 1f;


		Collider[] targetsInHearingRadius = Physics.OverlapSphere(transform.position, hearingRadius, whatIsHunterSeeker);

		if (targetsInHearingRadius.Length > 0)
			playerHeard = true;

		for (int i = 0; i < targetsInHearingRadius.Length; i++)
		{
			GameObject parent = targetsInHearingRadius[i].gameObject;
			Transform target = targetsInHearingRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			CalculateMultiplier();

			float dstToTarget = Vector3.Distance(transform.position, target.position);

			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, whatIsHunterSeeker))
			{
				WaitAndAddToList(secondsToDetect, target, "noisy");
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

