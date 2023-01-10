using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
    private Rigidbody rb;

    private float velocity;

    [HideInInspector] public float soundRange;

    private float harkonnenUnaware;
    private float harkonnenAware;
    private float harkonnenDetected;

    private float sardaukarUnaware;
    private float sardaukarAware;
    private float sardaukarDetected;

    private float mentatUnaware;
    private float mentatAware;
    private float mentatDetected;

    public LayerMask whatIsEnemy;
    public GameObject spicePrefab;

    private bool hit;

    private GameObject zhib;
    private ThrowingKnifeAbility baseScript;


    // Start is called before the first frame update
    void Start()
    {
        zhib = GameObject.Find("Zhib");
        baseScript = zhib.GetComponent<ThrowingKnifeAbility>();

        soundRange = baseScript.soundRange;
        harkonnenUnaware = baseScript.harkonnenUnaware;
        harkonnenAware = baseScript.harkonnenAware;
        harkonnenDetected = baseScript.harkonnenDetected;

        sardaukarUnaware = baseScript.sardaukarUnaware;
        sardaukarAware = baseScript.sardaukarAware;
        sardaukarDetected = baseScript.sardaukarDetected;

        mentatUnaware = baseScript.mentatUnaware;
        mentatAware = baseScript.mentatAware;
        mentatDetected = baseScript.mentatDetected;

        velocity = baseScript.knifeVelocity;
        hit = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        if (!hit)
        {
            transform.position += transform.rotation * Vector3.forward * velocity * Time.deltaTime;

            RaycastHit rayHit;

            if ((Physics.Raycast(transform.position, -Vector3.up, out rayHit, 10f)))
            {

                float desiredDistance = 1.5f;

                if (rayHit.distance > desiredDistance)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + desiredDistance - rayHit.distance, transform.position.z);
                }
                else if (rayHit.distance < desiredDistance)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + desiredDistance - rayHit.distance, transform.position.z);
                }

            }
        }


    }

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.tag == "Enemy")
        {
            EnemyBehaviour eBehaviour = baseScript.targetEnemy.GetComponent<EnemyBehaviour>();
            EnemyDetection eDetection = baseScript.targetEnemy.GetComponent<EnemyDetection>();

            switch (eBehaviour.type)
            {
                case EnemyType.HARKONNEN:
                    switch (eDetection.state)
                    {
                        case DecState.STILL:
                            if (Random.value <= harkonnenUnaware)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                EmitSound();
                            }
                            break;

                        case DecState.SEEKING:
                            if (Random.value <= harkonnenAware)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                EmitSound();
                            }
                            break;

                        case DecState.FOUND:
                            if (Random.value <= harkonnenDetected)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                EmitSound();
                            }
                            break;
                    }
                    break;
                case EnemyType.SARDAUKAR:
                    switch (eDetection.state)
                    {
                        case DecState.STILL:
                            if (Random.value <= sardaukarUnaware)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                hit = true;
                                EmitSound();
                            }
                            break;

                        case DecState.SEEKING:
                            if (Random.value <= sardaukarAware)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                hit = true;
                                EmitSound();
                            }
                            break;

                        case DecState.FOUND:
                            if (Random.value <= sardaukarDetected)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                hit = true;
                                EmitSound();
                            }
                            break;
                    }
                    break;
                case EnemyType.MENTAT:
                    switch (eDetection.state)
                    {
                        case DecState.STILL:
                            if (Random.value <= mentatUnaware)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                hit = true;
                                EmitSound();
                            }
                            break;

                        case DecState.SEEKING:
                            if (Random.value <= mentatAware)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                hit = true;
                                EmitSound();
                            }
                            break;

                        case DecState.FOUND:
                            if (Random.value <= mentatDetected)
                            {
                                if (gameObject.layer != 10)
                                {
                                    SpawnSpice(eBehaviour, spicePrefab, baseScript.targetEnemy.transform.position, baseScript.targetEnemy.transform.rotation);
                                    UploadController.instance.SendKill((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.z);
                                    Destroy(coll.gameObject);
                                }
                            }
                            else
                            {
                                hit = true;
                                EmitSound();
                            }
                            break;
                    }
                    break;
            }

            hit = true;

            KnifeToGround();
        }


        if (coll.gameObject.tag == "Floor")
        {
            transform.gameObject.layer = 10;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }


        if (coll.gameObject.tag == "Wall")
        {
            hit = true;
            rb.useGravity = true;
            EmitSound();
        }


    }

    void SpawnSpice(EnemyBehaviour eBehaviour, GameObject spicePrefab, Vector3 pos, Quaternion rot)
    {
        switch (eBehaviour.type)
        {
            case EnemyType.HARKONNEN:
                if (Random.value < eBehaviour.harkonnenDropChance)
                {
                    GameObject spiceDropped = Instantiate(spicePrefab, pos, rot);
                    Spice spiceScript = spiceDropped.GetComponent<Spice>();
                    spiceScript.spiceAmmount = Random.Range(eBehaviour.harkonnenMinDrop, eBehaviour.harkonnenMaxDrop);
                }
                break;
            case EnemyType.SARDAUKAR:
                if (Random.value < eBehaviour.sardaukarDropChance)
                {
                    GameObject spiceDropped = Instantiate(spicePrefab, pos, rot);
                    Spice spiceScript = spiceDropped.GetComponent<Spice>();
                    spiceScript.spiceAmmount = Random.Range(eBehaviour.sardaukarMinDrop, eBehaviour.sardaukarMaxDrop);
                }
                break;
            case EnemyType.MENTAT:
                if (Random.value < eBehaviour.mentatDropChance)
                {
                    Quaternion spawnRot = baseScript.targetEnemy.transform.rotation;
                    spawnRot.x = 90;
                    GameObject spiceDropped = Instantiate(spicePrefab, pos, spawnRot);
                    spiceDropped.GetComponent<Spice>().spiceAmmount = Random.Range(eBehaviour.mentatMinDrop, eBehaviour.mentatMaxDrop); ;
                }
                break;
        }
    }
    void KnifeToGround()
    {
        rb.useGravity = true;

        RaycastHit rayHit;

        // If the ray casted from this object (in your case, the tree) to below it hits something...
        if ((Physics.Raycast(transform.position, -Vector3.up, out rayHit, 10f)))
        {

            // and if the distance between object and hit is larger than 0.3 (I judge it nearly unnoticeable otherwise)
            if (rayHit.distance > 0.3f)
            {
                // Then bring object down by distance value.
                transform.position = new Vector3(transform.position.x, transform.position.y - rayHit.distance, transform.position.z);
            }

        }
    }
    void EmitSound()
    {
        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, soundRange, whatIsEnemy);

        for (int i = 0; i < affectedEnemies.Length; i++)
        {
            affectedEnemies[i].GetComponent<EnemyDetection>().state = DecState.SEEKING;
            affectedEnemies[i].GetComponent<EnemyDetection>().timer = affectedEnemies[i].GetComponent<EnemyDetection>().secondsPerBar;

        }
    }
}
