using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombController : MonoBehaviourPunCallbacks
{
    [Header("Bomb")]
    public KeyCode inputKey = KeyCode.LeftShift;
    public GameObject bombPrefab;
    public float bombFuseTime = 3f;
    public int bombAmount = 1;
    private int bombsRemaining;

    [Header("Explosion")]
    public Explosion explosionPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;

    [Header("Destructible")]
    public Tilemap destructibleTiles;
    public Destructible destructiblePrefab;

    private PhotonView photonView;

    private void OnEnable()
    {
        bombsRemaining = bombAmount;
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        destructibleTiles = GameObject.FindGameObjectWithTag("DES").GetComponent<Tilemap>();
    }

    private void Update()
    {
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey) && photonView.IsMine)
        {
            PlaceBombCoroutine();
        }
    }

    private IEnumerator PlaceBomb()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        // Use PhotonNetwork.Instantiate to instantiate the bombPrefab
        GameObject bomb = PhotonNetwork.Instantiate(bombPrefab.name, position, Quaternion.identity);
        bombsRemaining--;

        yield return new WaitForSeconds(bombFuseTime);

        position = bomb.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        Explosion explosion = PhotonNetwork.Instantiate(explosionPrefab.name, position, Quaternion.identity).GetComponent<Explosion>();
        explosion.DestroyAfterNetwork(explosionDuration);

        ExplodeNetwork(position, Vector2.up, explosionRadius);
        ExplodeNetwork(position, Vector2.down, explosionRadius);
        ExplodeNetwork(position, Vector2.left, explosionRadius);
        ExplodeNetwork(position, Vector2.right, explosionRadius);

        PhotonNetwork.Destroy(bomb.gameObject);
        bombsRemaining++;
    }

    [PunRPC]
    void PlaceBombCoroutine()
    {
        photonView.RPC("PlaceBombNetwork", RpcTarget.All);
    }

    [PunRPC]
    void PlaceBombNetwork()
    {
        StartCoroutine(PlaceBomb());
    }

    [PunRPC]
    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0)
        {
            return;
        }

        position += direction;

        if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f))
        {
            ClearDestructibleNetwork(position);
            return;
        }

        GameObject explosionObj = PhotonNetwork.Instantiate(explosionPrefab.name, position, Quaternion.identity);
        Explosion explosion = explosionObj.GetComponent<Explosion>();

        explosion.SetDirectionNetwork(direction);
        explosion.DestroyAfterNetwork(explosionDuration);

        ExplodeNetwork(position, direction, length - 1);
    }

    void ExplodeNetwork(Vector2 position, Vector2 direction, int length)
    {
        photonView.RPC("Explode", RpcTarget.All, position, direction, length);
    }

    [PunRPC]
    private void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if (tile != null)
        {
            destructibleTiles.SetTile(cell, null);
        }
    }

    void ClearDestructibleNetwork(Vector2 position)
    {
        Debug.Log("ClearDestructibleNetwork called at position: " + position);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ClearDestructible", RpcTarget.All, position);
        }
    }

    [PunRPC]
    public void AddBomb()
    {
        bombAmount++;
        bombsRemaining++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            other.isTrigger = false;
        }
    }
}
