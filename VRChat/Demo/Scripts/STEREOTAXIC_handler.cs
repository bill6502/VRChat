
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class STEREOTAXIC_handler : UdonSharpBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private string ParameterName;
    [SerializeField]
    private Slider slider;

    [UdonSynced, FieldChangeCallback(nameof(SetParameter))]
    private float ParameterValue;
    public float SetParameter
    {
        set
        {
            slider.value = value;
            ParameterValue = value;

            animator.SetFloat(ParameterName, ParameterValue);
        }
        get => ParameterValue;
    }

    private VRCPlayerApi LocalPlayer;

    private void Start()
    {
        LocalPlayer = Networking.LocalPlayer;
    }

    public void OnValueChanged()
    {
        Networking.SetOwner(LocalPlayer, gameObject);

        ParameterValue = slider.value;
        animator.SetFloat(ParameterName, ParameterValue);
        RequestSerialization();
    }


}
