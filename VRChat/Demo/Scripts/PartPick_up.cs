
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[RequireComponent(typeof(VRC_Pickup))]
public class PartPick_up : UdonSharpBehaviour
{
    public GameObject Position;
    public Animator TipAnimator;
    public string TipParameter = "index";

    [Tooltip("Distance of Resetting position"), Range(0f, .2f)]
    public float Distance = .1f;

    [HideInInspector]
    public bool isInside = false;

    private Rigidbody rig;
    private VRCPlayerApi LocalPlayer;

    private void Start()
    {
        //checking
        bool isExist = false;
        foreach (var _parameter in TipAnimator.parameters)
        {
            if (_parameter.name == TipParameter)
                isExist = true;
        }
        if (!isExist)
            return;

        //setting
        rig = this.GetComponent<Rigidbody>();
        LocalPlayer = Networking.LocalPlayer;
    }

    public override void OnDrop()
    {
        bool isOwner = Networking.IsOwner(LocalPlayer, gameObject);
        if (!Position || !TipAnimator || !isOwner) return;

        rig.useGravity = false;
        rig.isKinematic = true;

        //ResetPosition
        if (!isInside) return;

        TipAnimator.SetInteger(TipParameter, 0);
        transform.position = Position.transform.position;
        transform.rotation = Position.transform.rotation;
    }

    private void Update()
    {
        float distance = Vector3.Distance(Position.transform.position, transform.position);
        if (distance > Distance)
        {
            isInside = false;
            return;
        }
        isInside = true;
    }
}
