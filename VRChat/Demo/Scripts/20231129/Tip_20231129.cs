
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Tip_20231129 : UdonSharpBehaviour
{
    public PartPickup_20231129 Target;
    // public Animator TipAnimator;
    // public string TipParameterName = "index";

    [SerializeField]
    private Image image;
    [SerializeField]
    private Color unclick = new Color(1f, 1f, 1f, 1f);
    [SerializeField]
    private Color clicked = new Color(0f, 1f, 0f, 1f);

    [HideInInspector]
    public bool isShowLabel = false;

    private VRCPlayerApi LocalPlayer;

    private void Start()
    {
        //checking
        if (!Target || !image)
            return;

        //setting
        LocalPlayer = Networking.LocalPlayer;
    }

    public override void Interact()
    {
        //Getting Owner
        Networking.SetOwner(LocalPlayer, gameObject);

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(tip));
    }

    public void tip()
    {
        // if (Target.isInside) return;

        // bool isResetPosition = Target.transform.position == Target.Position.transform.position && Target.transform.rotation == Target.Position.transform.rotation;
        isShowLabel = !isShowLabel;
        // TipAnimator.SetBool(TipParameterName, isShowLabel);

        if (isShowLabel)
            image.color = clicked;
        else
            image.color = unclick;
    }
}
