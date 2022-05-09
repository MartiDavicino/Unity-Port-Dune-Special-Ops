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
	private float playerStateMultipler;
	[HideInInspector] public float multiplierHolder;

	public float debuffTime;
	private float debuffTimer;
	private Material materialHolder;
	private Transform child;
	[HideInInspector] public float sightDebuffMultiplier;
	[HideInInspector] public bool debuffed;

	public EnemyBehaviour data;

	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;

	public float hearingRadius;

	private CharacterBaseBehavior baseScript;

	private Camera camera;
	private GameObject player;

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
	public LayerMask whatIsHunterSeeker;

	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
	[HideInInspector] public List<Transform> noisyTargets = new List<Transform>();

	private float elapse_time = 0;

	void Start()
    {
		camera = Camera.main;
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
		player = camera.GetComponentInChildren<CameraMovement>().focusedPlayer;
		
		if(player != null)
        {
			angle1 = DirFromAngle(-viewAngle / 2, false);
			angle2 = DirFromAngle(viewAngle / 2, false);

			string[] splitArray = player.name.Split(char.Parse(" "));
			string[] splitArray2 = splitArray[0].Split(char.Parse("("));
			string finalName = splitArray2[0];

			if (finalName != "HunterSeeker")
			{
				if (player.GetComponent<CharacterBaseBehavior>().state == PlayerState.WALKING)
				{
					hearingRadius = 7.5f;
				}

				else if (player.GetComponent<CharacterBaseBehavior>().state == PlayerState.CROUCH)
				{
					hearingRadius = 7.5f;
				}

				else if (player.GetComponent<CharacterBaseBehavior>().state == PlayerState.RUNNING)
				{
					hearingRadius = 11.5f;
				}
			
				playerStateMultipler = player.GetComponent<CharacterBaseBehavior>().detectionMultiplier;
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
			sightDebuffMultiplier = 1f;


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
		timer += proportion  * distanceMultiplier * playerStateMultipler * sightDebuffMultiplier * Time.deltaTime;


		if (timer > 0 && timer < secondsToDetect / 2)
		{
			state = DecState.STILL;
		}
		else if (timer >= timer / 2 && timer < secondsToDetect)
		{
			state = DecState.SEEKING;
			elapse_time = 0;
		}

		if (timer >= secondsToDetect)
        {
			if(target.gameObject == GameObject.Find("HunterSeeker(Clone)"))
            {
				timer = delay;
				state = DecState.FOUND;
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
				targets.Add(target);
				state = DecState.FOUND;
            }
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
			
			CalculateMultiplier();

			baseScript = parent.GetComponent<CharacterBaseBehavior>();

			float dstToTarget = Vector3.Distance(transform.position, target.position);

			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, targetMask))
			{
				WaitAndAddToList(secondsToDetect, target, noisyTargets);
			} 
		}

		return playerHeard;
	}

	bool FindHunterSeeker()
	{
		bool playerHeard = false;

		sightMultiplier = 1f;

		noisyTargets.Clear();

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

