using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI bestTimeText;
    [SerializeField] GameObject gameOverText;
    public GameObject winText;
    public GameObject reTryBtn;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Joystick leftJoystick;
    [SerializeField] Joystick rightJoystick;
    [SerializeField] Button jumpButton;

    private float surviveTime;
    public bool isGameOver;
    private GameObject currentPlayer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            float bestTime = PlayerPrefs.GetFloat("BestTime", 0);
            bestTimeText.text = "Best Time: " + bestTime.ToString("F2");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Photon 서버 연결
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        // 게임 시작 시 타이머 텍스트 활성화
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinOrCreateRoom("DefaultRoom", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        // 게임 시작 시 타이머 텍스트 활성화
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        // 플레이어를 항상 생성
        SpawnPlayer();
    }

    void Update()
    {
        if (!isGameOver)
        {
            surviveTime += Time.deltaTime;
            timerText.text = "Time: " + surviveTime.ToString("F2");
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                OnRestart();
            }
        }
    }

    public void EndGame()
    {
        isGameOver = true;

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        if (reTryBtn != null)
        {
            reTryBtn.SetActive(true);
        }

        float bestTime = PlayerPrefs.GetFloat("BestTime", 0);

        if (surviveTime > bestTime)
        {
            bestTime = surviveTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }

        if (bestTimeText != null)
        {
            bestTimeText.text = "Best Time: " + bestTime.ToString("F2");
        }
    }

    public void OnRestart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 현재 플레이어 객체 초기화
            currentPlayer = null;

            // 마스터 클라이언트가 모든 클라이언트에게 씬 로드를 요청
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
        }
    }

    private void SpawnPlayer()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // 네트워크를 통해 플레이어 생성
            GameObject player = PhotonNetwork.Instantiate("Player", playerSpawnPoint.position, playerSpawnPoint.rotation);
            currentPlayer = player; // 생성된 플레이어를 저장

            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.leftJoystick = leftJoystick;
                playerController.rightJoystick = rightJoystick;
                playerController.jumpButton = jumpButton;
                playerController.vCam = virtualCamera;

                UpdateVirtualCameraTarget(player);
            }
        }
    }

    private void UpdateVirtualCameraTarget(GameObject player)
    {
        if (virtualCamera != null && player != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }
}
