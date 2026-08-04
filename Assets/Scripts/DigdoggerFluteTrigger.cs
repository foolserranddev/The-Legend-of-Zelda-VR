using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigdoggerFluteTrigger : FluteTrigger
{
    public GameObject miniDigdoggerPrefab;
    public int numMiniDigdoggersToSpawn = 2;

	// Update is called once per frame
	void Update ()
    {
		if (responding)
        {
            activated = true;
            for (int i = 0; i < numMiniDigdoggersToSpawn; i++)
            {
                Digdogger d = Instantiate(miniDigdoggerPrefab, transform.position, miniDigdoggerPrefab.transform.rotation, transform.parent).GetComponent<Digdogger>();
                d.parent = GetComponent<Digdogger>();
            }
            responding = false;
            gameObject.SetActive(false);
        }
	}
}
