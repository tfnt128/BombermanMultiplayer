using Photon.Pun;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AnimatedSpriteRenderer start;
    public AnimatedSpriteRenderer middle;
    public AnimatedSpriteRenderer end;

    public PhotonView photonView;

    public void reference(PhotonView go)
    {
        photonView = go;
    }

    [PunRPC]
    public void SetActiveRenderer(AnimatedSpriteRenderer renderer)
    {
        start.enabled = renderer == start;
        middle.enabled = renderer == middle;
        end.enabled = renderer == end;        
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
    public void SetActiveRendererNetwork(AnimatedSpriteRenderer renderer)
    {
        photonView.RPC("SetActiveRenderer", RpcTarget.All, renderer);
    }

    public void destroyAfterNetwork(float seconds)
    {
        photonView.RPC("DestroyAfter", RpcTarget.All, seconds);
    }
    public void SetDirectionNetwork(Vector2 direction)
    {
        photonView.RPC("SetDirection", RpcTarget.All, direction);
    }
}
