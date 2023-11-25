
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Button : UdonSharpBehaviour
{
    public Game game;
    public string funcName;

    private VRCPlayerApi LocalPlayer;

    private void Start()
    {
        LocalPlayer = Networking.LocalPlayer;
    }
    public override void Interact()
    {
        game.SendCustomEvent(funcName);
    }
}
