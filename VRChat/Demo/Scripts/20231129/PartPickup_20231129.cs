
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[RequireComponent(typeof(VRC_Pickup))]
public class PartPickup_20231129 : UdonSharpBehaviour
{
    public Tip_20231129 tip;
    public GameObject Position;

    [Tooltip("Distance of Resetting position"), Range(0f, .2f)]
    public float Distance = .1f;

    [SerializeField]
    private GameObject label;
    [SerializeField, Range(-180f, 180f)]
    private float offsetAngle = -90f;
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform[] linePosition;

    [HideInInspector]
    public bool isInside = false;

    [UdonSynced]
    private Vector3 _position2;
    private Rigidbody rig;
    private VRCPlayerApi LocalPlayer;

    private void Start()
    {
        //setting
        rig = this.GetComponent<Rigidbody>();
        LocalPlayer = Networking.LocalPlayer;

        if (Networking.IsOwner(LocalPlayer, gameObject))
            _position2 = linePosition[1].position - linePosition[0].position;
    }

    public override void OnDrop()
    {
        bool isOwner = Networking.IsOwner(LocalPlayer, gameObject);
        if (!Position || !isOwner) return;

        rig.useGravity = false;
        rig.isKinematic = true;

        //ResetPosition
        if (!isInside) return;

        transform.position = Position.transform.position;
        transform.rotation = Position.transform.rotation;
    }

    private void Update()
    {
        if (!tip || linePosition.Length == 0 || !label) return;

        //show Label
        label.SetActive(tip.isShowLabel);
        lineRenderer.SetPosition(0, linePosition[0].position);
        lineRenderer.SetPosition(1, linePosition[0].position);
        if (tip.isShowLabel)
            lineRenderer.SetPosition(1, linePosition[0].position + _position2);

        label.transform.position = linePosition[0].position + _position2;

        //look at player
        label.transform.LookAt(LocalPlayer.GetBonePosition(HumanBodyBones.Head));
        if (offsetAngle != 0f)
            label.transform.rotation *= Quaternion.AngleAxis(offsetAngle, Vector3.up);

        //check distance
        float distance = Vector3.Distance(Position.transform.position, transform.position);
        if (distance > Distance)
            isInside = false;
        else
            isInside = true;
    }
}
