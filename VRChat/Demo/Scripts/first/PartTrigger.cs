
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PartTrigger : UdonSharpBehaviour
{
    public Animator TipAnimator;
    public string TipParameter = "index";

    private bool isSetted = false;

    private void Start()
    {
        //checking
        if (!TipAnimator)
        {
            Debug.LogErrorFormat(this.name + " : Target or TipAnimator is Null.");
            return;
        }

        bool isExist = false;
        foreach (var _parameter in TipAnimator.parameters)
        {
            if (_parameter.name == TipParameter)
                isExist = true;
        }
        if (!isExist)
        {
            Debug.LogErrorFormat(this.name + " : TipParameter does not exist in TipAnimator.");
            return;
        }

        //setting
        isSetted = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //checking
        // bool isTarget = target.gameObject == other.gameObject;
        if (!isSetted) return;

        var isPartPick_up = other.GetComponent<PartPick_up>();
        if (!isPartPick_up) return;
        //Resetting value of TipParameter
        TipAnimator.SetInteger(TipParameter, 0);
    }
}
