
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Tip : UdonSharpBehaviour
{
    public PartPick_up Target;
    public Animator TipAnimator;
    public string TipParameter = "index";

    [Header("Setting value of TipParameter in TipAnimator.")]
    public int index = 0;

    private VRCPlayerApi LocalPlayer;

    private bool isSetted = false;

    private void Start()
    {
        //checking
        if (!Target || !TipAnimator)
            return;

        bool isExist = false;
        foreach (var _parameter in TipAnimator.parameters)
        {
            if (_parameter.name == TipParameter)
                isExist = true;
        }
        if (!isExist)
            return;

        //setting
        isSetted = true;
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

        if (!isSetted) return;

        // if (Target.isInside) return;

        // bool isResetPosition = Target.transform.position == Target.Position.transform.position && Target.transform.rotation == Target.Position.transform.rotation;
        if (TipAnimator.GetInteger(TipParameter) != index)
            TipAnimator.SetInteger(TipParameter, index);
        else
            TipAnimator.SetInteger(TipParameter, 0);
    }
}
