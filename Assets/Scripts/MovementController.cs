using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;

    public float speed = 5f;

    private Vector2 direction = Vector2.down;
    private Rigidbody2D _rigidbody;

    private bool isDead = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (_photonView.IsMine && !isDead)
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");
            SetDirection(new Vector2(hor, ver));
        }
    }

    private void FixedUpdate()
    {
        if (_photonView.IsMine && !isDead)
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (_photonView.IsMine)
        {
            _photonView.RPC("OnDeathSequenceEnded", RpcTarget.All, !_photonView.IsMine);
        }

        gameObject.SetActive(false);
    }

    [PunRPC]
    private void OnDeathSequenceEnded(bool isDefeated)
    {
        if (_photonView.IsMine)
        {
            if (isDefeated)
            {
                screenManager.Instance.ShowDefeatScreen();
            }
            else
            {
                screenManager.Instance.ShowVictoryScreen();
            }
        }
    }
}