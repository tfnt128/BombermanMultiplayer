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
        Debug.Log("AAAAAAAAAAA");
        if (PhotonNetwork.IsMasterClient)
        {
            DestroyAfterNetwork();
        }
    }

    [PunRPC]
    private void DestroyAfterNetwork()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}