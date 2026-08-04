using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    public bool bigShield = false;
    public AudioClip ShieldBlock;

    private AudioSource audioSource;

	// Use this for initialization
	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = ShieldBlock;

        // The legacy shield's very shallow layered geometry produces severe
        // mobile shadow-map artifacts in OpenXR (especially from the nearby
        // sword). It can still cast a shadow, but should not receive one.
        Renderer[] shieldRenderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer shieldRenderer in shieldRenderers)
        {
            shieldRenderer.receiveShadows = false;
        }
    }
	
    void OnCollisionEnter(Collision collision)
    {
//        Collider col = collision.collider;
        audioSource.Play();
/*        if (col.tag == "Enemy Projectile")
        {
            //Debug.Log("Shield colliding with " + col.gameObject);
            //Projectile p = col.GetComponent<Projectile>();
            //if (p.CanSmallShieldBlock || (bigShield && p.CanLargeShieldBlock))
            //{
                //if (!audioSource.isPlaying)
                //{
                //col.GetComponent<Rigidbody>().useGravity = true;
                //    player.ignoreHit(col);
                //}
            //}
            //else
            //{
            //    Physics.IgnoreCollision(p.GetComponent<Collider>(), GetComponent<Collider>());
            //    //player.HandleCollisions(col);
            //}
        }
        else if (col.tag == "Collectible")
        {
            Player.player.OnTriggerEnter(col);
        }
        */
    }
}
