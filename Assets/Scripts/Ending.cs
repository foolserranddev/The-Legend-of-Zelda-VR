using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public GameObject[] ThingsToDestroy;
    public GameObject[] ThingsToKeep;
    public GameObject EndStory;
    public GameObject[] Triforces;
    public GameObject TriforceDestination;
    private Vector3 destination;
    public AudioClip EndMusic;

    private int endStep;
    private float [] triforceScaling = new float[3];

    AudioSource audioSource;
	// Use this for initialization
	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        destination = TriforceDestination.transform.position; // (Triforces[0].transform.position + Triforces[1].transform.position + Triforces[2].transform.position) / 3;
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (endStep)
        {
            case 1:
                if (Vector3.Distance(Triforces[0].transform.position, destination) < 0.1f)
                {
                    Triforces[0].transform.position = destination;
                    Triforces[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 0.2f, 0);
                    triforceScaling[0] = Triforces[0].transform.localScale.x;
                    triforceScaling[1] = triforceScaling[0] * 3;
                    triforceScaling[2] = Time.deltaTime;
                    Triforces[1].SetActive(false);
                    Triforces[2].SetActive(false);
                    endStep++;
                }
                break;
            case 2:
                triforceScaling[2] += Time.deltaTime;
                Triforces[0].transform.localScale = Vector3.one * Mathf.Lerp(triforceScaling[0], triforceScaling[1], triforceScaling[2]/2);
                if (!audioSource.isPlaying && triforceScaling[2] >= 2)
                {
                    endStep++;
                }
                break;
            case 3:
                EndStory.SetActive(true);
                EndStory.GetComponent<Rigidbody>().velocity = new Vector3(0, 17, 0);
                audioSource.clip = EndMusic;
                audioSource.Play();
                endStep++;
                break;
            case 4:
                if (!audioSource.isPlaying)
                {
                    Destroy(StandardStuff.ss.gameObject);
                    Destroy(SaveData.saveData.gameObject);
                    SceneManager.LoadScene(0);
                }
                break;
            default:
                break;
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (endStep == 0)
        {
            if (col.tag == "Sword" || col.tag == "Player")
            {
                endStep = 1;
                Player.player.Immobilize();
                Player.player.StopMusic();
                audioSource.Play();
                foreach (GameObject t in Triforces)
                {
                    t.SetActive(true);
                    t.transform.SetParent(null);
                    t.GetComponent<Rigidbody>().velocity = (destination - t.transform.position).normalized * Vector3.Distance(destination, t.transform.position) * 0.175f;
                }
                foreach (GameObject go in ThingsToKeep)
                {
                    go.transform.SetParent(null);
                }
                foreach (GameObject go in ThingsToDestroy)
                {
                    go.SetActive(false);
                    Destroy(go);
                }
            }
        }
    }
}
