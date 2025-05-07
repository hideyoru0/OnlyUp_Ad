using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Finish : MonoBehaviourPun
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 마스터 클라이언트에서만 승리 처리
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("HandleWin", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void HandleWin()
    {
        // 모든 클라이언트에서 승리 UI 활성화
        GameManager.instance.winText.SetActive(true);
        GameManager.instance.reTryBtn.SetActive(true);
    }
}
