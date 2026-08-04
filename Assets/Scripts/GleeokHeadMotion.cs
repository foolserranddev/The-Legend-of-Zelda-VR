using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GleeokHeadMotion : MonoBehaviour
{

    private enum parts
    {
        Structure,
        Static,
        Seg1,
        Seg2,
        Seg3,
        Seg4,
        Head,
        NumParts
    }
    public float MovementSpeed = 5;
    public float ChangeDirectionTime = 10;
    public float chainMultiplier = 1.5f;
    public Vector3[] MinRotation = new Vector3[(int)parts.NumParts];
    public Vector3[] MaxRotation = new Vector3[(int)parts.NumParts];
    public Transform[] Joints = new Transform[(int)parts.NumParts];
    public Transform[] SpikeBalls = new Transform[(int)parts.NumParts];
    private Vector3[] directions = new Vector3[(int)parts.NumParts];
    public Vector3[] trackedRotation = new Vector3[(int)parts.NumParts];
    private bool sideways = true;
    private const float Rad2Deg = 57.2958f;
    private const int UP = 0;
    private const int RIGHT = 1;

    // Use this for initialization
    void Start ()
    {
        Joints[0] = transform.GetChild(0); // Static, invisible joint. No ball
        directions[0][UP] = Random.Range(8.0f, 10.0f) * (Random.Range(0, 2) == 1 ? 1 : -1);
        directions[0][RIGHT] = Random.Range(0.0f, 1.0f) * (Random.Range(0, 2) == 1 ? 1 : -1);
        for (int i = 1; i < (int)parts.NumParts; i++)
        {
            directions[i][UP] = directions[i-1][0] * 0.9f;
            directions[i][RIGHT] = directions[i-1][0] * 0.9f;
            Joints[i] = Joints[i - 1].GetChild(1);
            SpikeBalls[i] = Joints[i-1].GetChild(0);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        sideways = !sideways;
        for (int i = 1; i < 5; i++)
        {
            movePart(i, sideways);
        }
    }


    void movePart(int i, bool up)
    {
        float dt = Time.deltaTime;
        Vector3 rot;
        Vector3 axis;
        int j = 0;

        if (sideways)
        {
            rot = new Vector3(0, 1, 0) * directions[i][1] * dt * MovementSpeed;
            axis = Vector3.up;
            j = 1;
        }
        else
        {
            rot = new Vector3(1,0,0) * directions[i][0] * dt * MovementSpeed;
            axis = Vector3.right;
        }
        Vector3 angle = Joints[i].position- Joints[i - 1].position;
        trackedRotation[i][UP] = Mathf.Atan(Mathf.Abs(-angle.y / Mathf.Abs(angle.z))) * Rad2Deg * (angle.y < 0 ? -1 : 1);
        trackedRotation[i][RIGHT] = Mathf.Atan(angle.x / Mathf.Abs(angle.z)) * Rad2Deg;
        if ((directions[i][j] > 0 && trackedRotation[i][j] + (directions[i][j] * dt * MovementSpeed) > MaxRotation[i][j]) ||
            (directions[i][j] < 0 && trackedRotation[i][j] + (directions[i][j] * dt * MovementSpeed) < MinRotation[i][j]))
        {
            rot *= -1;
            directions[i][j] *= -1;
        }
        float rotCounts = directions[i][j] * dt * MovementSpeed * (sideways ? 1 : -1);

        Joints[i].transform.RotateAround(Joints[i - 1].position, axis, rotCounts);
        for (int k = i; k < (int)parts.NumParts; k++)
        {
            SpikeBalls[k].LookAt(SpikeBalls[k].transform.position + Vector3.up);
        }
    }
}
