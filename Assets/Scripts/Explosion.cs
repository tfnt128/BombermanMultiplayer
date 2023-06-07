using Photon.Pun;
using UnityEngine;

public class Explosion : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    [PunRPC]
    public void DestroyAfter(float seconds)
    {
        Destroy(gameObject, seconds);
    }

    [PunRPC]
    public void SetDirectionNetwork(Vector2 direction)
    {
        photonView.RPC("SetDirection", RpcTarget.All, direction);
    }
    
    [PunRPC]
    public void DestroyAfterNetwork(float seconds)
    {
        photonView.RPC("DestroyAfter", RpcTarget.All, seconds);
    }
}