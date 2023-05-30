using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    float frente;
    float girar;
    public PhotonView photonview;
    public Camera myCamera;

    [Header("VIDA")]
    public Image playerHealthFill;
    public Text playerName;
    public float playerHealthMax = 100f;
    public float playerHealthCurrent = 0f;

    [Header("BALA")]
    public GameObject bullet;
    public GameObject bulletPhotonView;
    public GameObject spawnBullet;

    #region Metodos da Unity
    void Start()
    {
        photonview = GetComponent<PhotonView>();

        if(!photonView.IsMine)
        {
            myCamera.gameObject.SetActive(false);
        }

        Debug.LogWarning("Name: " + PhotonNetwork.NickName + " PhotonView: " + photonview.IsMine);
        frente = 10;
        girar = 60;
        playerName.text = PhotonNetwork.NickName;
        HealthManager(playerHealthMax);
    }
    void Update()
    {        
        if (photonview.IsMine)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(0, 0, (frente * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(0, 0, (-frente * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(0, (-girar * Time.deltaTime), 0);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(0, (girar * Time.deltaTime), 0);
            }

            Shooting();
        }
        
    }
    #endregion

    #region Meus Metodos

    void HealthManager(float value)
    {
        Debug.LogWarning("HealthManager");
        Debug.LogWarning("value= "+ value);
            playerHealthCurrent += value;
            playerHealthFill.fillAmount = playerHealthCurrent / 100;
        
    }

    public void TakeDamage( float value)
    {
        Debug.LogWarning("TakeDamage");
        Debug.LogWarning("value= " + value);
        photonView.RPC("TakeDamageNetwork", RpcTarget.AllBuffered, value);
    }

    [PunRPC]
    void TakeDamageNetwork(float value)
    {
        Debug.LogWarning("TakeDamageNetwork");
        HealthManager(value);
    }

    void Shooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC("Shoot", RpcTarget.All);
        }
     
    }

    [PunRPC]
    void Shoot()
    {
        Instantiate(bullet, spawnBullet.transform.position, spawnBullet.transform.rotation);
    }

    #endregion
}
