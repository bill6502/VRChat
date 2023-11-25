
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DropOrResetPosition : UdonSharpBehaviour
{
    [Tooltip("[0,infinity]"), Min(0f)]
    public float randomForce = 0f;

    public Animator TipAnimator;
    public string TipParameter = "index";
    public PartPick_up[] Objects;
    public GameObject[] Positions;
    public Slider slider;

    [UdonSynced, HideInInspector]
    public bool isDropped = false;

    private Rigidbody[] rigs;
    private VRCPlayerApi LocalPlayer;
    // private float multiple = 1f;

    private bool isSetted = false;

    private void Start()
    {
        //checking
        if (Objects.Length == 0 || Positions.Length == 0 || Objects.Length != Positions.Length || !TipAnimator)
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


        rigs = new Rigidbody[Objects.Length];
        for (int i = 0; i < Objects.Length; ++i)
            rigs[i] = Objects[i].GetComponent<Rigidbody>();
    }

    public override void Interact()
    {
        if (!isSetted) return;

        //Getting Owner
        Networking.SetOwner(LocalPlayer, gameObject);

        foreach (var Object in Objects)
            Networking.SetOwner(LocalPlayer, Object.gameObject);

        //Dropping or ResetPositions
        for (int i = 0; i < rigs.Length; ++i)
        {
            if (!isDropped)
                RandomForce(i);
            else
                ResetPositions(i);
        }

        isDropped = !isDropped;
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(resetAnimatorParameter));
    }

    public void resetAnimatorParameter()
    {
        TipAnimator.SetInteger(TipParameter, 0);
    }

    public void RandomForce(int i)
    {
        var rig = rigs[i];
        rig.useGravity = true;
        rig.isKinematic = false;

        if (randomForce == 0f) return;

        float x = 0;
        float y = 0;

        while (Mathf.Abs(x) == 0) x = Random.Range(-randomForce, randomForce);
        while (Mathf.Abs(y) == 0) y = Random.Range(-randomForce, randomForce);

        Vector3 Force = new Vector3(x, y, 0);
        rig.AddForce(Force, ForceMode.Force);
    }

    public void ResetPositions(int i)
    {
        var isPosition = Objects[i].transform.position == Positions[i].transform.position;

        if (isPosition) return;

        var rig = rigs[i];
        rig.useGravity = false;
        rig.isKinematic = true;

        Objects[i].transform.position = Positions[i].transform.position;
        Objects[i].transform.rotation = Positions[i].transform.rotation;
    }

    public void shuffle()
    {
        //only doing shuffle after droping
        if (!isDropped) return;

        //get owner
        Networking.SetOwner(LocalPlayer, gameObject);

        foreach (var Object in Objects)
            Networking.SetOwner(LocalPlayer, Object.gameObject);

        //shuffle
        int[] swapped = new int[Objects.Length];

        for (int i = 0; i < Objects.Length; ++i)
            swapped[i] = -1;

        int begin = Objects.Length % 2 == 0 ? 0 : 1;
        for (int i = begin; i < Objects.Length; ++i)
        {
            //skip when the position of Object[i] was swapped
            if (swapped[i] != -1) continue;

            //get other index of object
            int randomIndex = i;
            while (randomIndex == i || swapped[randomIndex] != -1)
                randomIndex = Random.Range(0, Objects.Length);

            //record index
            swapped[i] = randomIndex;
            swapped[randomIndex] = i;

            //swap position
            var swapPosition = Objects[i].transform.position;
            Objects[i].transform.position = Objects[randomIndex].transform.position + Vector3.up;
            Objects[randomIndex].transform.position = swapPosition + Vector3.up;
        }
    }

    public void setRandomForce()
    {
        if (!slider) return;

        Networking.SetOwner(LocalPlayer, gameObject);

        randomForce = slider.value * rigs[0].mass * 100;
    }
}
