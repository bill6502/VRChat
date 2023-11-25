
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[RequireComponent(typeof(VRC_Pickup)), UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class fruit : UdonSharpBehaviour
{
    [SerializeField]
    private Game game;
    [SerializeField]
    private FruitBucket Bucket;

    [HideInInspector, UdonSynced]
    public int index = -1;
    [HideInInspector, UdonSynced]
    public int materialIndex = -1;

    private Rigidbody _rigidbody;
    private MeshRenderer _meshRenderer;

    private VRCPlayerApi LocalPlayer;
    private VRC_Pickup pickup;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();

        LocalPlayer = Networking.LocalPlayer;
        pickup = GetComponent<VRC_Pickup>();
    }

    public override void OnPickup()
    {
        bool isOwner = Networking.IsOwner(LocalPlayer, game.gameObject);
        if (!isOwner || !game.gameState) return;

        game.previewFruit();
    }

    public override void OnDrop()
    {
        bool isOwner = Networking.IsOwner(LocalPlayer, game.gameObject);
        if (!isOwner || !game.gameState) return;

        // SendCustomNetworkEvent(NetworkEventTarget.All, nameof(unPickupable));

        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
    }

    public void setFruit()
    {
        //setting this fruit
        index = game.index;
        materialIndex = game.nextFruit;
        RequestSerialization();

        transform.localScale = Bucket.localScale.localScale * game.scales[materialIndex];
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(setMaterial));
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Pickupable));

        // Debug.Log($"{name} : index={index} ,Material={_meshRenderer.material.name}");
    }

    public void nextLevel()
    {
        transform.localScale = Bucket.localScale.localScale * game.scales[materialIndex];
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(setMaterial));
    }

    public void setMaterial()
    {
        _meshRenderer.material = game.materials[materialIndex];
        _rigidbody.mass = game.scales[materialIndex] * game.mass;
    }

    public void Reset()
    {
        index = -1;
        materialIndex = -1;
        RequestSerialization();

        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;

        transform.position = Bucket.startPosition.position;
        transform.localScale = Bucket.localScale.transform.localScale;

        // SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Pickupable));
    }

    public void AddPoints()
    {
        game.score = game.score + game.points[materialIndex - 1];
        game.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(game.updateScore));
    }

    private void OnCollisionEnter(Collision other)
    {
        Collision(other);
    }

    private void OnCollisionStay(Collision other)
    {
        Collision(other);
    }

    public void Collision(Collision other)
    {
        bool isOwner = Networking.IsOwner(LocalPlayer, game.gameObject);
        bool isFruit = other.gameObject.layer == LayerMask.NameToLayer("fruit");
        if (!isOwner || !isFruit) return;

        VRC_Pickup otherPickup = other.gameObject.GetComponent<VRC_Pickup>();
        if (otherPickup && otherPickup.IsHeld)
        {
            game.GameOver();
            return;
        }

        if (pickup.IsHeld || !game.gameState) return;

        Networking.SetOwner(LocalPlayer, gameObject);
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(unPickupable));

        fruit otherFruit = other.gameObject.GetComponent<fruit>();
        VRC_Pickup other_pickup = other.gameObject.GetComponent<VRC_Pickup>();
        bool isPickupable = pickup.pickupable || other_pickup.pickupable;
        if (index <= otherFruit.index || materialIndex != otherFruit.materialIndex || isPickupable)
            return;

        _rigidbody.velocity = Vector3.zero;
        materialIndex++;
        RequestSerialization();

        otherFruit.Reset();

        AddPoints();

        if (materialIndex == game.materials.Length)
        {
            Reset();
            return;
        }
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(nextLevel));
    }

    public void Pickupable()
    {
        pickup.pickupable = true;
    }

    public void unPickupable()
    {
        pickup.pickupable = false;
    }
}
