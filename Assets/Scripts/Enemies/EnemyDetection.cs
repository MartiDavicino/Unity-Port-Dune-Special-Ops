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
	[Header("- Detection -")]
	public float secondsPerBar;
	[HideInInspector] public float timer = 0.0f;
	private float proportion;

	public float aggroTime;
	private float aggroTimer;
	[HideInInspector] public bool isAggro;

	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;
	public float hearingRadius;

	public float debuffTime;
	private float debuffTimer;
	private Material materialHolder;
	private Transform child;
	[HideInInspector] public float debuffMultiplier;
	[HideInInspector] public bool debuffed;


	[Header("- Detection Multipliers -")]
	public float sightMultiplier;
	public float maxDistanceMultiplier;
	[Range(0.0f, 1.0f)]
	public float notDetectedMultiplier;

	private float playerStateMultipler;
	[HideInInspector] public float multiplierHolder;
	private float distanceMultiplier;

	[Header("- Base -")]
	public DecState state = DecState.STILL;
	[HideInInspector] public bool debug = false;

	private CharacterBaseBehavior baseScript;
	private Camera generalCamera;
	private GameObject player;

	public EnemyBehaviour data;

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

		debuffed = false;
		debuffTimer = 0f;

		isAggro = false;
		aggroTimer = 0f;

		string[] splitArray = name.Split(char.Parse(" "));
		string[] splitArray2 = splitArray[0].Split(char.Parse("("));
		string finalName = splitArray2[0];

		child = transform.Find(finalName + "_low");
		materialHolder = child.GetComponent<Renderer>().material;
	}
    void Update()
    {
		player = generalCamera.GetComponentInChildren<CameraMovement>().focusedPlayer;
		
		if(isAggro)
        {
			while (aggroTimer < aggroTime)
			{
				aggroTimer += Time.deltaTime;
				return;
			}

			aggroTimer = 0;
			isAggro = false;
		}

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
	void CalculateMultiplier(float radius, Transform target)
    {
		distanceMultiplier = 1 + (maxDistanceMultiplier - 1) * (1 - CalculateAbsoluteDistance(target.position).magnitude / radius);
		if (distanceMultiplier < 1) distanceMultiplier = 1;

		//distanceMultiplier = radius * maxDistanceMultiplier / CalculateAbsoluteDistance(data.player.transform.position).magnitude;
		if (distanceMultiplier > maxDistanceMultiplier)
			distanceMultiplier = maxDistanceMultiplier;
    }

    void FindTargetsWithDelay()
	{
		bool playerInView = FindVisibleTargets();
		bool playerHeard = FindNoisyTargets();
		bool mosquitoHeard = FindHunterSeeker();


		if (!playerInView && !playerHeard && !mosquitoHeard && !isAggro)
			TargetsNotFound();
	}
	void TargetsNotFound()
	{
		timer -= proportion * notDetectedMultiplier * Time.deltaTime;

		if(state == DecState.FOUND)
			state = DecState.SEEKING;

		if(state == DecState.SEEKING && timer < 0)
        {
			timer = secondsPerBar;
			state = DecState.STILL;
        }

		if(state == DecState.STILL && timer < 0)
        {
			timer = secondsPerBar;
			state = DecState.STILL;
			timer = 0f;
		}
	}
	void WaitAndAddToList(float delay,Transform target, string targetType)
    {
		timer += proportion * sightMultiplier * playerStateMultipler * debuffMultiplier * distanceMultiplier * Time.deltaTime;

		if (timer > 0 && state == DecState.STILL)
		{
			state = DecState.STILL;

			if(timer >= secondsPerBar)
            {
				state = DecState.SEEKING;
				timer = 0;
            }
		}

		if (timer > 0 && state == DecState.SEEKING)
		{
			state = DecState.SEEKING;
			elapse_time = 0;

			if (timer >= secondsPerBar)
			{
				state = DecState.FOUND;
				isAggro = true;
			}
		}

		if (timer >= secondsPerBar && state == DecState.FOUND)
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

		float finalRadius = Mathf.Sqrt(transform.position.y * transform.position.y + viewRadius * viewRadius);

		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, finalRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{

			if(targetsInViewRadius[i].gameObject == GameObject.Find("HunterSeeker(Clone)"))
            	return playerInView;


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
						CalculateMultiplier(viewRadius, target);
						WaitAndAddToList(secondsPerBar, target, "visible");
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

		float finalRadius = Mathf.Sqrt(transform.position.y * transform.position.y + hearingRadius * hearingRadius);

		Collider[] targetsInHearingRadius = Physics.OverlapSphere(transform.position, finalRadius, targetMask);

		for (int i = 0; i < targetsInHearingRadius.Length; i++)
		{
			GameObject parent = targetsInHearingRadius[i].gameObject;
			Transform target = targetsInHearingRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			

			baseScript = parent.GetComponent<CharacterBaseBehavior>();
			NavMeshAgent pAgent = baseScript.GetComponent<NavMeshAgent>();

			float dstToTarget = Vector3.Distance(transform.position, target.position);

			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, targetMask))
			{
				//Waiting for still behaviour when crouching 

				//if((pAgent.remainingDistance > 0 && !pAgent.pathPending))
                //{
					CalculateMultiplier(hearingRadius, target);
					WaitAndAddToList(secondsPerBar, target, "noisy");
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

			CalculateMultiplier(hearingRadius, target);

			float dstToTarget = Vector3.Distance(transform.position, target.position);

			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, whatIsHunterSeeker))
			{
				WaitAndAddToList(secondsPerBar, target, "noisy");
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

