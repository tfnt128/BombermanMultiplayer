using UnityEngine;
using Photon.Pun;

public class Destructible : MonoBehaviourPunCallbacks
{
    public float destructionTime = 1f;

    [Range(0f, 1f)]
    public float itemSpawnChance = 0.2f;
    public GameObject[] spawnableItems;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            DestroyAfterNetwork();
        }
    }

    [PunRPC]
    private void DestroyAfterNetwork()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}