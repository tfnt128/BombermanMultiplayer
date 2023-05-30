using Photon.Pun;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public PhotonView photonView;
    

    private void Start()
    {
        //photonView = GameObject.FindGameObjectWithTag("PHOTON").GetComponent<PhotonView>();
    }
    public enum ItemType
    {
        ExtraBomb,
        BlastRadius,
        SpeedIncrease,
    }

    public ItemType type;

    [PunRPC]
    private void OnItemPickup(int playerId, ItemType itemType)
    {
        GameObject player = PhotonView.Find(playerId).gameObject;

        switch (itemType)
        {
            case ItemType.ExtraBomb:
                player.GetComponent<BombController>().AddBomb();
                break;

            case ItemType.BlastRadius:
                player.GetComponent<BombController>().explosionRadius++;
                break;

            case ItemType.SpeedIncrease:
                player.GetComponent<MovementController>().speed++;
                break;
        }

        Destroy(gameObject);
    }

    void OnItemPickupNetwork(int playerId, ItemType itemType)
    {
        photonView.RPC("OnItemPickup", RpcTarget.All, playerId, itemType);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            photonView = other.GetComponentInParent<PhotonView>();

            if (photonView != null)
            {
                int playerId = photonView.ViewID;
                OnItemPickupNetwork(playerId, type);
            }
        }
    }

}
