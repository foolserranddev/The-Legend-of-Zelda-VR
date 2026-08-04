using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class HandController : MonoBehaviour
{
    [SerializeField] private XRNode controllerHand = XRNode.RightHand;
    [SerializeField] private bool driveTransformFromXR = true;
    [SerializeField] private Vector3 oculusRotationOffset = new Vector3(35f, 0f, 0f);
    [SerializeField] private float stickNavigationThreshold = 0.65f;
    [SerializeField] private float stickNavigationInitialDelay = 0.45f;
    [SerializeField] private float stickNavigationRepeatDelay = 0.16f;

    public Vector3 velocity { get { return xrInput == null ? Vector3.zero : xrInput.Velocity; } }
    public Vector3 angularVelocity { get { return xrInput == null ? Vector3.zero : xrInput.AngularVelocity; } }
    public bool adjustMixedRealityCam = false;
    public MixedRealityCamera mixedRealityCam;
    public GameObject ItemsFrame;

    // Use this for initialization

    public GameObject[] items;
    public HandController otherHand;
    public GameObject shieldPrefab;
    public enum HandPositionsEnum
    {
        Open,
        Sword,
        Shield,
        Bomb,
        Candle,
    }

    protected bool isMainHand = false;
    public bool isGripped = false;
    public GameObject heldObject;
    public Menu[] MenuList;
    private InteractableObject collidedObject;
    private GameObject shield;
    private InteractableObject SecondaryItem;
    private bool TriggerHeld = false;
    //private bool cyclingItems = false;
    private float itemCyclingPrevAngle;
    private float padY;
    private float padX;
    private AudioSource audioSource;
    private Animator animator;
    private PortableXRInput xrInput;
    private bool previousTrigger;
    private bool previousGrip;
    private bool previousPadPress;
    private bool previousPadTouch;
    private bool previousMenu;
    private bool previousPrimaryButton;
    private bool previousSecondaryButton;
    private bool stickNavigationActive;
    private float nextStickNavigationTime;

//    public void FixedUpdate()
//    {
//        if (cyclingItems)
//        {
//            Vector2 currTouchpos = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
//            float currentAngle = Mathf.Rad2Deg * Mathf.Atan2(currTouchpos.y, currTouchpos.x);
//            if (currentAngle < 0) currentAngle += 360;
//            //Debug.Log(currentAngle);
//            float diff;
//            if (itemCyclingPrevAngle > 270 && currentAngle < 90) diff = currentAngle - itemCyclingPrevAngle + 360;
//            else diff = Mathf.Abs(currentAngle - itemCyclingPrevAngle);
//            if (diff > 45f)
//            {
//                bool clockwise = ((currentAngle < itemCyclingPrevAngle && !(itemCyclingPrevAngle > 270 && currentAngle < 90)) || (currentAngle > 270 && itemCyclingPrevAngle < 90));
////                if (clockwise) Debug.Log("Going clockwise because current Angle is " + currentAngle + " and previous angle was " + itemCyclingPrevAngle);
////                else Debug.Log("Going counter-clockwise because current Angle is " + currentAngle + " and previous angle was " + itemCyclingPrevAngle);
//                GameObject go = player.ChangeSecondary(clockwise);
//                SwapItem(go);
//                itemCyclingPrevAngle = currentAngle;
//            }
//        }
//    }

    public void OnGripped(ClickedEventArgs e)
    {
        if (MenuList.Length > 0)
        {
            foreach (Menu m in MenuList)
            {
                if (m?.gameObject.activeSelf ?? false)
                {
                    m.OnMakeSelection();
                    return;
                }
            }
        }
        // Quest A: use the secondary item while it is out. X remains available
        // for the legacy grip/save behavior on the left controller.
        if (controllerHand == XRNode.RightHand && isMainHand && TriggerHeld && heldObject != null)
        {
            ActivateSecondaryItem();
            return;
        }
        if (otherHand.isGripped)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0) SaveData.saveData.Save();
        }
        else
        {
            isGripped = true;
        }
    }

    public void OnUngripped(ClickedEventArgs e)
    {
        isGripped = false;
    }

    public void Awake()
    {
        if (CompareTag("LeftHand")) controllerHand = XRNode.LeftHand;
        else if (CompareTag("RightHand")) controllerHand = XRNode.RightHand;
        xrInput = new PortableXRInput(controllerHand);
        animator = FindHandAnimator();
        if (animator != null)
        {
            // Nearby held items create unstable mobile shadow-map patterns on
            // the legacy hand mesh. The hand can still cast a ground shadow.
            Renderer[] handRenderers = animator.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer handRenderer in handRenderers)
            {
                handRenderer.receiveShadows = false;
            }
        }
    }

    private Animator FindHandAnimator()
    {
        Animator[] candidates = GetComponentsInChildren<Animator>(true);
        foreach (Animator candidate in candidates)
        {
            foreach (AnimatorControllerParameter parameter in candidate.parameters)
            {
                if (parameter.name == "HandPosition") return candidate;
            }
        }
        return candidates.Length > 0 ? candidates[0] : null;
    }

    private void ActivateSecondaryItem()
    {
        InteractableObject io = heldObject.GetComponent<InteractableObject>();
        if (io == null) return;

        io.performAction();
        if (io.prefabEnum == ObjectList.prefabObjects.Bomb && Player.player.pd.NumBombs == 0)
        {
            GameObject go = Player.player.ChangeSecondary(true);
            if (go != heldObject && Player.player.pd.secondaryObjectIndex != -1) SwapItem(go);
            else heldObject = null;
        }
    }

    public void Update()
    {
        if (xrInput == null) xrInput = new PortableXRInput(controllerHand);

        Vector3 position;
        Quaternion rotation;
        if (driveTransformFromXR && xrInput.TryGetPose(out position, out rotation))
        {
            transform.localPosition = position;
            transform.localRotation = rotation * (xrInput.IsOculusTouch
                ? Quaternion.Euler(oculusRotationOffset)
                : Quaternion.identity);
        }

        Vector2 axis = xrInput.PrimaryAxis;
        ClickedEventArgs e = new ClickedEventArgs { padX = axis.x, padY = axis.y };
        DispatchButton(xrInput.TriggerPressed, ref previousTrigger, OnTriggerClicked, OnTriggerUnclicked, e);
        DispatchButton(xrInput.GripPressed, ref previousGrip, OnGripped, OnUngripped, e);
        DispatchButton(xrInput.AxisPressed, ref previousPadPress, OnPadClicked, OnPadUnclicked, e);
        DispatchButton(xrInput.AxisTouched, ref previousPadTouch, OnPadTouched, OnPadUntouched, e);
        DispatchButton(xrInput.MenuPressed, ref previousMenu, OnMenuClicked, null, e);
        DispatchButton(xrInput.PrimaryButtonPressed, ref previousPrimaryButton, OnGripped, OnUngripped, e);
        DispatchButton(xrInput.SecondaryButtonPressed, ref previousSecondaryButton, OnMenuClicked, null, e);
        UpdateStickMenuNavigation(axis);
        if (xrInput.Trigger > 0.001f) OnTriggerPulled(xrInput.Trigger);
    }

    private void UpdateStickMenuNavigation(Vector2 axis)
    {
        // The Vive touchpad also selected secondary items while the main-hand
        // trigger (the legacy "Z button") was held, before the item frame was
        // visible. Preserve that behavior for the controller stick.
        bool canNavigate = HasActiveMenu() || (isMainHand && TriggerHeld);
        if (!canNavigate || axis.magnitude < stickNavigationThreshold)
        {
            stickNavigationActive = false;
            return;
        }

        if (!stickNavigationActive || Time.unscaledTime >= nextStickNavigationTime)
        {
            Vector2 cardinal = Mathf.Abs(axis.x) > Mathf.Abs(axis.y)
                ? new Vector2(Mathf.Sign(axis.x), 0f)
                : new Vector2(0f, Mathf.Sign(axis.y));
            OnPadClicked(new ClickedEventArgs { padX = cardinal.x, padY = cardinal.y });
            nextStickNavigationTime = Time.unscaledTime +
                (stickNavigationActive ? stickNavigationRepeatDelay : stickNavigationInitialDelay);
            stickNavigationActive = true;
        }
    }

    private bool HasActiveMenu()
    {
        if (MenuList == null) return false;
        foreach (Menu menu in MenuList)
            if (menu != null && menu.gameObject.activeSelf) return true;
        return false;
    }

    private static void DispatchButton(bool current, ref bool previous, System.Action<ClickedEventArgs> pressed,
        System.Action<ClickedEventArgs> released, ClickedEventArgs e)
    {
        if (current && !previous && pressed != null) pressed(e);
        else if (!current && previous && released != null) released(e);
        previous = current;
    }

    public void SendHapticImpulse(float amplitude = 0.5f, float duration = 0.08f)
    {
        if (xrInput != null) xrInput.SendHapticImpulse(amplitude, duration);
    }

    public void OnTriggerPulled(float triggerPullPercent)
    {
        if (MenuList.Length > 0)
        {
            foreach (Menu m in MenuList)
            {
                if (m?.gameObject.activeSelf ?? false)
                {
                    return;
                }
            }
        }

        if (Player.player.Dead) return;
        if (adjustMixedRealityCam)
        {
            if (tag == "RightHand") mixedRealityCam.AdjustCam(isGripped, triggerPullPercent, padX, padY);
            return;
        }
        //        Debug.Log("Should Move - MainHand is " + isMainHand + " pullPercent is " + triggerPullPercent);
        if (!isMainHand && triggerPullPercent > 0.05)
        {
            triggerPullPercent = Mathf.Min(triggerPullPercent, 0.9f);
            Vector3 goForward = transform.forward;
            Player.player.Move(goForward, triggerPullPercent);
        }
    }

    public void Empty()
    {
        if (heldObject != null) heldObject.SetActive(false);
        if (otherHand.heldObject != null) otherHand.heldObject.SetActive(false);
        if (shield != null) shield.SetActive(false);
        if (otherHand.shield != null) otherHand.shield.SetActive(false);
        ChangeHandPosition(HandPositionsEnum.Open);
        otherHand.ChangeHandPosition(HandPositionsEnum.Open);
        isMainHand = false;
        otherHand.isMainHand = false;
    }

    public bool isMain()
    {
        return isMainHand;
    }

    public void setMain()
    {
        //Debug.Log("Setting " + transform.tag + " as Main Hand");
        audioSource = GetComponent<AudioSource>();
        isMainHand = true;
        otherHand.setShield();
        Player.player.pd.handednessDetermined = true;
        Vector3 localPos = ItemFrame.itemFrame.transform.localPosition;
        Vector3 localEuler = ItemFrame.itemFrame.transform.localEulerAngles;
        ItemFrame.itemFrame.transform.parent = transform;
        ItemFrame.itemFrame.transform.localPosition = localPos;
        ItemFrame.itemFrame.transform.localEulerAngles = localEuler;
        ItemFrame.itemFrame.gameObject.SetActive(false);
        if (Player.player.pd.primaryObjectIndex != -1) SwapItem(ObjectList.objectList.prefabs[Player.player.pd.primaryObjectIndex]);
        else ChangeHandPosition(HandPositionsEnum.Open);
    }

    public void setMain(bool DoNothing)
    {
        if (DoNothing)
        {
            isMainHand = true;
        }
        else
        {
            setMain();
        }
    }

    public void setShield()
    {
        isMainHand = false;
        shield = ObjectList.objectList.prefabs[(int)ObjectList.prefabObjects.Shield];
        shield.transform.SetParent(transform);
        int reverse = 1;
        if (tag == "LeftHand") reverse = -1;
        shield.transform.localPosition = new Vector3(0.02944458f * reverse, -0.06738106f, -0.07952332f);
        if (Player.player.HasBigShield.val)
        {
            shield.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            shield.layer = LayerMask.NameToLayer("Big Shield");
        }
        else
        {
            shield.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            shield.layer = LayerMask.NameToLayer("Shield");
        }
        shield.transform.localEulerAngles = new Vector3(37.348f, 90.632f * reverse, 26.819f * reverse);
        //Transform map = StatusWindow.statusWindow.TriforcePieces_ExplorationMap.transform;
        //Vector3 pos = map.localPosition;
        //pos.x *= reverse;
        //pos.z *= reverse;
        //Vector3 rot = map.localEulerAngles;
        //rot *= reverse;
        //map.localPosition = pos;
        //map.localEulerAngles = rot;

        ChangeHandPosition(HandPositionsEnum.Shield);
        shield.SetActive(true);

        //******************
        // Position Status Window
        //******************
        StatusWindow s = StatusWindow.statusWindow;
        s.transform.SetParent(shield.transform);

        s.transform.localPosition = new Vector3(0, 1.6f, -0.314f);
        s.transform.localEulerAngles = new Vector3(0, 0, 0);
        s.transform.localScale = Vector3.one * 0.4f;
    }

    public void OnMenuClicked(ClickedEventArgs e)
    {
        if (Player.player.Dead) return;
        if (MenuList?[0].GetComponent<SaveMenu>()??false)
        {
            MenuList[0].gameObject.SetActive(!(MenuList[0].gameObject.activeSelf));
        }
    }

    public void OnTriggerClicked(ClickedEventArgs e)
    {
        if (MenuList.Length > 0)
        {
            foreach (Menu m in MenuList)
            {
                if (m?.gameObject.activeSelf ?? false)
                {
                    return;
                }
            }
        }
        if (Player.player.Dead) return;
        //base.OnTriggerClicked(e);
        if (adjustMixedRealityCam)
        {
            if (tag == "LeftHand")
            {
                mixedRealityCam.placeMarker(transform);
            }
            return;
        }
        TriggerHeld = true;
        if (isMainHand)
        {
            if (heldObject != null) heldObject.GetComponent<InteractableObject>().TurnOff();
            GameObject go = Player.player.GetSecondaryItem();
            if (go != null && !(go.GetComponent<InteractableObject>().prefabEnum == ObjectList.prefabObjects.Bomb && Player.player.pd.NumBombs == 0))
            {
                heldObject = go;
                InteractableObject io = heldObject.GetComponent<InteractableObject>();
                io.InitialSetup(this, otherHand);
                io.TurnOn();
                ChangeHandPosition(io.handPosition);
            }
        }
    }

    public void OnTriggerUnclicked(ClickedEventArgs e)
    {
        if (Player.player.Dead) return;
        if (adjustMixedRealityCam) return;
        TriggerHeld = false;
        if (isMainHand)
        {
            //heldObject.GetComponent<InteractableObject>().performTriggerUnclicked(this, e);
            if (heldObject != null)
            {
                heldObject.GetComponent<InteractableObject>().TurnOff();
            }
            GameObject go = Player.player.GetPrimaryItem();
            if (go != null)
            {
                heldObject = go;
                InteractableObject io = heldObject.GetComponent<InteractableObject>();
                io.InitialSetup(this, otherHand);
                io.TurnOn();
                ChangeHandPosition(io.handPosition);
            }
        }
        if (!isMainHand) return;
        //cyclingItems = false;
        if (ItemsFrame != null) ItemsFrame.SetActive(false);
    }


    public void OnPadUntouched(ClickedEventArgs e)
    {
        if (!isMainHand) return;
        //cyclingItems = false;
        //ItemsFrame.SetActive(false);
    }

    public void OnPadClicked(ClickedEventArgs e)
    {
        if (MenuList.Length > 0)
        {
            foreach (Menu m in MenuList)
            {
                if (m?.gameObject.activeSelf?? false)
                {
                    m.OnPadClicked(e);
                    return;
                }
            }
        }
        if (Player.player.Dead) return;
        if (adjustMixedRealityCam)
        {
            padX = e.padX;
            padY = e.padY;
            return;
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Zelda Opening"))
        {
            return;
        }
        if (!isMainHand && !TriggerHeld && !otherHand.TriggerHeld)
        {
            setMain();
        }
        if(isMainHand)
        {
            // The pad is 4 buttons: left, right, top, bottom separated by 45-degree lines.
            if ((TriggerHeld || ItemsFrame.activeSelf) && e.padX > Mathf.Abs(e.padY)) // Right
            {
                //cyclingItems = true;
                ItemsFrame.SetActive(true);
                GameObject go = Player.player.ChangeSecondary(true);
                if (go != heldObject && Player.player.pd.secondaryObjectIndex != -1)
                {
                    audioSource.Play();
                    SwapItem(go);
                }
            }
            else if ((TriggerHeld || ItemsFrame.activeSelf) && e.padX < -Mathf.Abs(e.padY)) // Left
            {
                //cyclingItems = true;
                ItemsFrame.SetActive(true);
                GameObject go = Player.player.ChangeSecondary(false);
                if (go != heldObject && Player.player.pd.secondaryObjectIndex != -1)
                {
                    audioSource.Play();
                    SwapItem(go);
                }
            }
            else if (e.padY > Mathf.Abs(e.padX)) // Top
            {
                //cyclingItems = true;
                ItemsFrame.SetActive(!ItemsFrame.activeSelf);
            }
            else if (TriggerHeld && e.padY < -Mathf.Abs(e.padX)) // Bottom
            {
                if (heldObject != null) ActivateSecondaryItem();
            }
            else // probably impossible, but right on a line
            {

            }
        }
        Vector2 v = xrInput == null ? Vector2.zero : xrInput.PrimaryAxis;
        itemCyclingPrevAngle = Mathf.Rad2Deg * Mathf.Atan2(v.y, v.x);
        if (itemCyclingPrevAngle < 0) itemCyclingPrevAngle += 360;

        //        Debug.Log("Held Object " + heldObject + " null = " + heldObject == null);
    }

    private void SwapItem(GameObject go)
    {
        //Debug.Log("Swapping item with " + go);
        if (heldObject != null) heldObject.GetComponent<InteractableObject>().TurnOff();
        heldObject = go;
        InteractableObject io = heldObject.GetComponent<InteractableObject>();
        io.InitialSetup(this, otherHand);
        io.TurnOn();
        ChangeHandPosition(io.handPosition);
    }

    public void OnPadTouched(ClickedEventArgs e)
    {
    }

    public void OnPadUnclicked(ClickedEventArgs e)
    {
    }

    public void OnTriggerEnter(Collider col)
    {
        if (Player.player.Dead) return;
        // The hand only cares if we trigger an interactable object
        InteractableObject io = col.GetComponent<InteractableObject>();
        if (io == null)
        {
            if (!isMainHand && !otherHand.isMain() && col.GetComponent<Purchaseable>() != null) setMain();
            return;
        }

        if (io.tag == "Item")
        {
            ObjectList.prefabObjects num = io.ObtainPrefab();
            bool updateSword = Player.player.pd.primaryObjectIndex == -1 && num == ObjectList.prefabObjects.Sword;
            Player.player.ObtainPrefab(num, io.itemLevelIndex);
            if (!isMainHand && !otherHand.isMain()) setMain();
            Collectible c = io.GetComponent<Collectible>();
            if (c != null) c.Received();
            Destroy(io.gameObject);
            if (updateSword)
            {
                if (TriggerHeld) otherHand.SwapItem(ObjectList.objectList.prefabs[(int)num]);
                else SwapItem(ObjectList.objectList.prefabs[(int)num]);
            }
        }
    }

    public void ChangeHandPosition(HandPositionsEnum handPosition)
    {
        if (animator == null) animator = FindHandAnimator();
        if (animator == null) return;

        animator.SetInteger("HandPosition", (int)handPosition);
        string stateName = handPosition == HandPositionsEnum.Shield ? "Shield"
            : handPosition == HandPositionsEnum.Sword ? "Sword"
            : "Open";
        // These are authored static grip poses. Apply the requested state
        // immediately so modern Animator transition timing cannot leave the
        // hand in its previous/default shape.
        animator.Play(stateName, 0, 1f);
        animator.Update(0f);
    }
}
