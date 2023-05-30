using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombController : MonoBehaviour
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
    [HideInInspector]
    public Tilemap destructibleTiles;
    [HideInInspector]
    public Destructible destructiblePrefab;


    public PhotonView photonView;
    private void OnEnable()
    {
        bombsRemaining = bombAmount;
    }
    private void Start()
    {
        destructibleTiles = GameObject.FindGameObjectWithTag("DES").GetComponent<Tilemap>();
    }
    private void Update()
    {
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey))
        {
            PlaceBombCoroutine();
        }
    }

    [PunRPC]
    private IEnumerator PlaceBomb()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        GameObject bomb = Instantiate(bombPrefab, position, Quaternion.identity);
        bombsRemaining--;

        yield return new WaitForSeconds(bombFuseTime);

        position = bomb.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);

        
        Destroy(bomb.gameObject);
        bombsRemaining++;
    }

    [PunRPC]
    void PlaceBombCoroutine()
    {
        StartCoroutine(PlaceBomb());

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

        if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask))
        {
            ClearDestructible(position);
            return;
        }

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, direction, length - 1);
    }

    void ExplodeNetwork(Vector2 position, Vector2 direction, int length)
    {
        photonView.RPC("Explode", RpcTarget.All, position, direction, length);
    }

    
    private void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if (tile != null)
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleTiles.SetTile(cell, null);
        }
    }
    void ClearDestruibleNetwork(Vector2 position)
    {
        ClearDestructible(position);

        photonView.RPC("ClearDestructible", RpcTarget.All, position);
    }


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
