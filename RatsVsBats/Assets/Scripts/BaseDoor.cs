using UnityEngine;

public abstract class BaseDoor : MonoBehaviour
{
    public Animator animator;
    public bool isOpened;
    public GameObject collision;

    private void Start()
    {
        collision = gameObject.transform.parent.Find("Collision").gameObject;
    }

    protected virtual bool CanOpenDoor(PlayerController player)
    {
        return false;
    }

    protected virtual void OnInteract(PlayerController player)
    {
        // Implement the interaction behavior for specific doors
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if (isOpened)
            {
                CanvasManager.Instance.NonDoorMSG();
                return;
            }

            if(CanOpenDoor(player))
            {
                CanvasManager.Instance.DoorMSG("Press [E] to open this door");
            }
            else
            {
                CanvasManager.Instance.DoorMSG("You can't open this door");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if (CanOpenDoor(player))
            {
                if (player.isInteracting)
                {
                    OnInteract(player);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CanvasManager.Instance.NonDoorMSG();
    }
}