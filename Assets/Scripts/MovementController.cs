using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    private PhotonView _photonview;

    public float speed = 5f;
    
    private Vector2 direction = Vector2.down;
    private Rigidbody2D _rigidbody;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _photonview = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (_photonview.IsMine)
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");
            SetDirection(new Vector2(hor,ver));
        }
        
    }
    private void FixedUpdate()
    {
        if (_photonview.IsMine)
        {
            Vector2 position = _rigidbody.position;
            Vector2 translation = direction * (speed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(position + translation);
        }
    }
    private void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion")) {
            Die();
        }
    }

    //PUUUUUN
    [PunRPC]
    private void DeathSequence()
    {
        enabled = false;
        GetComponent<BombController>().enabled = false;
        Invoke(nameof(DeathSequenceEnd), 1.25f);
    }

    [PunRPC]
    private void OnDeathSequenceEnded(bool isLocalPlayer)
    {
        if (isLocalPlayer)
        {
            screenManager.Instance.ShowDefeatScreen();
        }
        else
        {
            screenManager.Instance.ShowVictoryScreen();
        }
    
        gameObject.SetActive(false);
    }


    void DeathSequenceEnd()
    {
        _photonview.RPC("OnDeathSequenceEnded", RpcTarget.All, _photonview.IsMine);
    }

    void Die()
    {
        _photonview.RPC("DeathSequence", RpcTarget.All);       
    }


    

}
