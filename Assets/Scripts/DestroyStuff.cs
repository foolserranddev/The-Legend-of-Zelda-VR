using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyStuff : MonoBehaviour
{
    public bool stayDestroyed = false;
    public GameObject [] StuffToDestroy;
    public MeshRenderer[][] meshes;
    public SpriteRenderer[][] sprites;
    public float BlinkyTime = 0.05f;
    public float LengthOfBlinkingTime = 1.5f;
    public bool disappearInstead = false;
    public bool ReadyToDestroyOnCollision = false;

    private bool readyToDestroy = false;
    private float destroyTime;
    private float blinkTime;

    private SaveVar<bool> obtained = new SaveVar<bool>(false);

	// Use this for initialization
	void Start ()
    {
        if (stayDestroyed) SaveData.saveData.data.registerBool(StandardStuff.getName(transform), obtained);
        if (obtained.val) destroyStuff();
        meshes = new MeshRenderer[StuffToDestroy.Length][];
        sprites = new SpriteRenderer[StuffToDestroy.Length][];
        for (int i = 0; i < StuffToDestroy.Length; i++)
        {
            bool active = StuffToDestroy[i].activeSelf;
            StuffToDestroy[i].SetActive(true);
            meshes[i] = StuffToDestroy[i].GetComponentsInChildren<MeshRenderer>();
            sprites[i] = StuffToDestroy[i].GetComponentsInChildren<SpriteRenderer>();
            StuffToDestroy[i].SetActive(active);
        }
        //foreach(MeshRenderer[] mrs in meshes)
        //{
        //    foreach (MeshRenderer mr in mrs)
        //    {
        //        Debug.Log(StandardStuff.getName(mr.transform));
        //    }
        //}
    }


    // Update is called once per frame
    private void Update()
    {
        if (readyToDestroy && BlinkyTime > 0)
        {
            if (Time.time > destroyTime) destroyStuff();
            else if (Time.time > blinkTime)
            {
                foreach (MeshRenderer [] M in meshes)
                {
                    foreach (MeshRenderer m in M)
                    {
                        if (m == null) continue;
                        m.enabled = !(m.enabled);
                    }
                }
                foreach (SpriteRenderer[] S in sprites)
                {
                    foreach (SpriteRenderer s in S)
                    {
                        if (s == null) continue;
                        s.enabled = !(s.enabled);
                    }
                }
            }
        }
    }

    public void OnDisable()
    {
        if (readyToDestroy) destroyStuff();
        if (disappearInstead)
        {
            Collider c;
            foreach (GameObject o in StuffToDestroy)
            {
                o.SetActive(true);
                c = o.GetComponent<Collider>();
                if (c != null) c.enabled = true;
                foreach (Transform t in o.transform)
                {
                    c = t.GetComponent<Collider>();
                    if (c != null) c.enabled = true;
                }
            }
            foreach (MeshRenderer[] M in meshes)
            {
                foreach (MeshRenderer m in M)
                {
                    if (m == null) continue;
                    m.enabled = true;
                }
            }
            foreach (SpriteRenderer[] S in sprites)
            {
                foreach (SpriteRenderer s in S)
                {
                    if (s == null) continue;
                    s.enabled = true;
                }
            }
        }
    }

    public bool Destroying()
    {
        return readyToDestroy;
    }

	void destroyStuff ()
    {
        obtained.val = true;
        foreach (GameObject o in StuffToDestroy)
        {
            if (disappearInstead) o.SetActive(false);
            else Destroy(o.gameObject);
        }
        readyToDestroy = false;
    }

    public void GetReadyToDestroy()
    {
        readyToDestroy = true;
        obtained.val = true;
        if (BlinkyTime > 0)
        {
            destroyTime = Time.time + LengthOfBlinkingTime;
            blinkTime = Time.time + BlinkyTime;
            Collider c;
            foreach (GameObject o in StuffToDestroy)
            {
                c = o.GetComponent<Collider>();
                if (c != null) c.enabled = false;
                foreach (Transform t in o.transform)
                {
                    c = t.GetComponent<Collider>();
                    if (c != null) c.enabled = false;
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if ((readyToDestroy || ReadyToDestroyOnCollision) && (col.tag == "Player"))
        {
            destroyStuff();
        }
    }
}
