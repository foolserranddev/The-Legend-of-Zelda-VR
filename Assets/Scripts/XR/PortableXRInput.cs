using System;
using UnityEngine;
using UnityEngine.XR;

public struct ClickedEventArgs
{
    public uint controllerIndex;
    public uint flags;
    public float padX;
    public float padY;
}

/// <summary>
/// Device-neutral controller state used by Zelda gameplay. OpenXR maps the
/// physical Quest Touch, Vive, Index, and other controllers into these usages.
/// </summary>
public sealed class PortableXRInput
{
    private readonly XRNode node;
    private InputDevice device;

    public PortableXRInput(XRNode node)
    {
        this.node = node;
    }

    public bool IsTracked { get { return Read(CommonUsages.isTracked, false); } }
    public Vector2 PrimaryAxis { get { return Read(CommonUsages.primary2DAxis, Vector2.zero); } }
    public float Trigger { get { return Read(CommonUsages.trigger, 0f); } }
    public bool TriggerPressed { get { return Read(CommonUsages.triggerButton, Trigger > 0.55f); } }
    public bool GripPressed { get { return Read(CommonUsages.gripButton, Read(CommonUsages.grip, 0f) > 0.55f); } }
    public bool AxisPressed { get { return Read(CommonUsages.primary2DAxisClick, false); } }
    public bool AxisTouched { get { return Read(CommonUsages.primary2DAxisTouch, AxisPressed); } }
    public bool MenuPressed { get { return Read(CommonUsages.menuButton, false); } }
    public bool PrimaryButtonPressed { get { return Read(CommonUsages.primaryButton, false); } }
    public bool SecondaryButtonPressed { get { return Read(CommonUsages.secondaryButton, false); } }
    public Vector3 Velocity { get { return Read(CommonUsages.deviceVelocity, Vector3.zero); } }
    public Vector3 AngularVelocity { get { return Read(CommonUsages.deviceAngularVelocity, Vector3.zero); } }

    public bool IsOculusTouch
    {
        get
        {
            RefreshDevice();
            string description = (device.name + " " + device.manufacturer).ToLowerInvariant();
            return description.Contains("oculus") || description.Contains("meta") || description.Contains("quest");
        }
    }

    public bool TryGetPose(out Vector3 position, out Quaternion rotation)
    {
        position = Read(CommonUsages.devicePosition, Vector3.zero);
        rotation = Read(CommonUsages.deviceRotation, Quaternion.identity);
        return IsTracked;
    }

    public void SendHapticImpulse(float amplitude, float duration)
    {
        RefreshDevice();
        HapticCapabilities capabilities;
        if (device.isValid && device.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
        {
            device.SendHapticImpulse(0, Mathf.Clamp01(amplitude), Mathf.Max(0f, duration));
        }
    }

    private bool Read(InputFeatureUsage<bool> usage, bool fallback)
    {
        RefreshDevice();
        bool value;
        return device.isValid && device.TryGetFeatureValue(usage, out value) ? value : fallback;
    }

    private float Read(InputFeatureUsage<float> usage, float fallback)
    {
        RefreshDevice();
        float value;
        return device.isValid && device.TryGetFeatureValue(usage, out value) ? value : fallback;
    }

    private Vector2 Read(InputFeatureUsage<Vector2> usage, Vector2 fallback)
    {
        RefreshDevice();
        Vector2 value;
        return device.isValid && device.TryGetFeatureValue(usage, out value) ? value : fallback;
    }

    private Vector3 Read(InputFeatureUsage<Vector3> usage, Vector3 fallback)
    {
        RefreshDevice();
        Vector3 value;
        return device.isValid && device.TryGetFeatureValue(usage, out value) ? value : fallback;
    }

    private Quaternion Read(InputFeatureUsage<Quaternion> usage, Quaternion fallback)
    {
        RefreshDevice();
        Quaternion value;
        return device.isValid && device.TryGetFeatureValue(usage, out value) ? value : fallback;
    }

    private void RefreshDevice()
    {
        if (!device.isValid)
        {
            device = InputDevices.GetDeviceAtXRNode(node);
        }
    }
}
