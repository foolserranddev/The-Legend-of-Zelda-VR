using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAndArrow : InteractableObject {

    public GameObject ProjectilePrefab;
    public Transform arrowTransform;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public AudioSource audioSource;
    public AudioClip arrowShoot;
    public AudioClip[] Twangs;
    public Material[] ArrowMaterials;
    private Projectile p;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        CheckDisplayArrow();
    }

    public override void UpdateLevel(int level)
    {
        base.UpdateLevel(level);
        arrowTransform.GetComponentInChildren<MeshRenderer>().material = ArrowMaterials[level];
    }

    public override void performAction()
    {
        if (ProjectilePrefab == null) return;
        if (p != null || Player.player.pd.NumRupees == 0 || !Player.player.pd.hasArrows)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = Twangs[Random.Range(0, Twangs.Length)];
                audioSource.Play();
            }
            return;
        }
        Player.player.AddRupees(-1);
        p = Instantiate(ProjectilePrefab, arrowTransform.position - (arrowTransform.up * 0.5f), arrowTransform.rotation).GetComponent<Projectile>();
        p.GetComponentInChildren<MeshRenderer>().material = ArrowMaterials[itemLevelIndex];
        p.tag = "Arrow";
        p.damagePerHit = 1;
        p.transform.localScale += new Vector3(-0.5f, 0, 0);
        p.Shoot(-transform.right, this.gameObject);
        audioSource.clip = arrowShoot;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update ()
    {
        CheckDisplayArrow();
    }

    private void CheckDisplayArrow()
    {
        // If the Display arrow is on, determine if we should turn it off
        if (arrowTransform.gameObject.activeSelf)
        {
            // If an arrow is flying, player doesn't have arrows, or player ran out of rupees--turn it off
            if (p != null || !Player.player.pd.hasArrows || Player.player.pd.NumRupees == 0) arrowTransform.gameObject.SetActive(false);
        }
        // If Display arrow is off, determine if we should turn it back on
        else
        {
            // If there is not an arrow flying, the player has arrows, and can afford arrows--turn back on
            if (p == null && Player.player.pd.hasArrows && Player.player.pd.NumRupees > 0) arrowTransform.gameObject.SetActive(true);
        }

    }
}
