using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

/// <summary>Drives the existing main camera from the OpenXR head pose.</summary>
public sealed class PortableXRHead : MonoBehaviour
{
    private InputDevice head;

    private void LateUpdate()
    {
        if (!head.isValid) head = InputDevices.GetDeviceAtXRNode(XRNode.Head);

        Vector3 position;
        Quaternion rotation;
        if (head.TryGetFeatureValue(CommonUsages.devicePosition, out position)) transform.localPosition = position;
        if (head.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation)) transform.localRotation = rotation;
    }
}

public static class PortableXRBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallHeadTracking()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        AttachToMainCamera();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachToMainCamera();
    }

    private static void AttachToMainCamera()
    {
        Camera camera = Camera.main;
        if (camera == null || camera.orthographic)
        {
            Camera[] cameras = Object.FindObjectsOfType<Camera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].enabled && !cameras[i].orthographic)
                {
                    camera = cameras[i];
                    break;
                }
            }
        }

        if (camera == null) return;

        // The legacy SteamVR rig keeps its render camera below a "Camera (head)"
        // tracking transform. Drive that parent so the HMD pose and controller
        // poses remain in the same local tracking-origin coordinate system.
        Transform poseTarget = camera.transform;
        if (poseTarget.parent != null && poseTarget.parent.name == "Camera (head)")
            poseTarget = poseTarget.parent;

        if (poseTarget.GetComponent<PortableXRHead>() == null)
            poseTarget.gameObject.AddComponent<PortableXRHead>();
    }
}
