using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{
    public float speed = 5f;
    private Transform player;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player GameObject with tag 'Player' not found!");
            }
        }
    }

    private void FixedUpdate()
    {
        // 적의 움직임은 마스터 클라이언트에서만 처리
        if (!PhotonNetwork.IsMasterClient) return;

        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);

        // 위치 동기화
        photonView.RPC("SyncPosition", RpcTarget.Others, transform.position);
    }

    [PunRPC]
    private void SyncPosition(Vector3 position)
    {
        // 다른 클라이언트에서 적의 위치를 업데이트
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 처리는 마스터 클라이언트에서만 처리
        if (!PhotonNetwork.IsMasterClient) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.EndGame();
            PhotonNetwork.Destroy(gameObject); // 네트워크에서 적 제거
        }
    }
}
