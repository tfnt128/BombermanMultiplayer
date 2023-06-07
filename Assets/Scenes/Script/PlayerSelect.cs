using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSelect : MonoBehaviourPunCallbacks
{
    public int playerSelected = 0;
    public GameObject playerList;
    public Image playerIconCanvas;

    PhotonView photonView;
    public GameObject myPlayerCanvas;
    private NetworkController net;

    public bool isPlayer1On;

    private void Start()
    {
        isPlayer1On = true;
        photonView = GetComponent<PhotonView>();
        net = FindObjectOfType<NetworkController>();

        if (!photonView.IsMine)
        {
            myPlayerCanvas.gameObject.SetActive(false);
        }
        SwitchPlayer();
    }

    public void SwitchPlayer()
    {
        photonView.RPC("SwitchPlayerRPC", RpcTarget.AllBuffered);
    }

    public void ButtonRight()
    {
        photonView.RPC("ButtonRightRPC", RpcTarget.AllBuffered);
    }

    public void ButtonLeft()
    {
        photonView.RPC("ButtonLeftRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ButtonRightRPC()
    {
        playerSelected++;
        
        if (playerSelected > (playerList.transform.childCount - 1))
        {
            
            playerSelected = 0;
            if (net.IsPlayer1())
            {
                transform.position = net.player1Pos.position;
            }
            else
            {
                transform.position = net.player2Pos.position;
            }
        }
        isPlayer1On = true;
        SwitchPlayer();
    }

    [PunRPC]
    public void ButtonLeftRPC()
    {
        playerSelected--;

        if (playerSelected < 0)
        {
            
            playerSelected = playerList.transform.childCount - 1;
            if (net.IsPlayer1())
            {
                
            }
            else
            {
                
            }
        }
        isPlayer1On = false;
        SwitchPlayer();
    }

    [PunRPC]
    public void SwitchPlayerRPC()
    {
        int i = 0;
        foreach (Transform item in playerList.transform)
        {
            if (i == playerSelected)
            {
                isPlayer1On = true;
                transform.position = net.player1Pos.position;
                item.gameObject.SetActive(true);

                if (item.gameObject.GetComponent<PlayerConfig>())
                {
                    playerIconCanvas.sprite = item.gameObject.GetComponent<PlayerConfig>().playerIcon;
                }
            }
            else
            {
                isPlayer1On = false;
                transform.position = net.player2Pos.position;
                item.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
