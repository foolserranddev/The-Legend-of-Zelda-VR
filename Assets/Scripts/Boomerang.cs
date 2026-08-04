using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : InteractableObject
{
    public AudioClip stunSound;
    public AudioClip blockSound;
    public AudioClip throwSound;
    public float [] speed = {1000f, 1500f};
    public float [] maxDistance = {6f, 12f};
    public float rotateSpeed = 1000;
    public Vector3 rot = new Vector3(0, 1, 0);
    public float VelocityLow = 0.5f;
    public float VelocityHigh = 2;

    private Rigidbody rigidBody;
    private bool velocityAboveThreshold;
    private bool isThrown;
    private bool isReturning;
    private Vector3 thrownPos;
    private bool isDisabled = true;
    private AudioSource throwAudioSource;
    private AudioSource hitAudioSource;


    public override void InitialSetup(HandController MainHand, HandController Offhand)
    {
        if (isThrown) return;
        base.InitialSetup(MainHand, Offhand);
        isDisabled = false;
        velocityAboveThreshold = false;
    }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        throwAudioSource = GetComponents<AudioSource>()[0];
        throwAudioSource.clip = throwSound;
        throwAudioSource.loop = true;
        hitAudioSource = GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update () {
        if (isThrown)
        {
            transform.Rotate(rot * rotateSpeed * Time.deltaTime);
            if (isReturning)
            {
                rigidBody.velocity = (mainHand.transform.position - transform.position).normalized * speed[itemLevelIndex];
            }
            else if (Vector3.Distance(thrownPos, transform.position) > maxDistance[itemLevelIndex])
            {
                isReturning = true;
            }
        }
        else if (mainHand != null)
        {
            float currSpeed = Vector3.Distance(mainHand.velocity, Vector3.zero);
            //            Debug.Log("Current Velocity = " + currSpeed);
            if (currSpeed > VelocityHigh)
            {
                velocityAboveThreshold = true;
            }
            else if (velocityAboveThreshold == true && currSpeed < VelocityLow)
            { 
                Debug.Log("Throwing");
                velocityAboveThreshold = false;
                Throw();
            }
        }
    }

    public bool IsThrown()
    {
        return isThrown;
    }

    void Throw()
    {
        isThrown = true;
        thrownPos = transform.position;
        transform.SetParent(null);
        rigidBody.constraints = ~RigidbodyConstraints.FreezeAll;
        rigidBody.velocity = transform.right * speed[itemLevelIndex];
        if (throwAudioSource != null) throwAudioSource.Play();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Collectible" && col.GetComponent<Collectible>().ArrowOrBoomerangCanCollect) Player.player.HandleCollisions(col);

        if (!isThrown) return;

        if (col.tag == "Zora" || col.tag == "Enemy")
        {
            if (col.GetComponent<Enemy>().isStunnable)
            {
                hitAudioSource.clip = stunSound;
                hitAudioSource.Play();
            }
            else
            {
                hitAudioSource.clip = blockSound;
                hitAudioSource.Play();
            }
            isReturning = true;
        }
        else if (isReturning && col.tag == "RightHand" || col.tag == "LeftHand" || col.tag == "Player")
        {
            throwAudioSource.Stop();
            isThrown = false;
            isReturning = false;
            rigidBody.velocity = Vector3.zero;
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            if (isDisabled) gameObject.SetActive(false);
            else InitialSetup(null, null);
        }
    }

    public override void TurnOff()
    {
        if (isThrown)
        {
            isDisabled = true;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
