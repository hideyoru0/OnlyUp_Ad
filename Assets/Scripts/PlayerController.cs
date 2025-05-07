using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public Joystick leftJoystick;
    public Joystick rightJoystick;
    public Button jumpButton;
    public CinemachineVirtualCamera vCam;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 100f; // 카메라 회전 속도

    private Rigidbody rb;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private bool isGrounded = false; // Ground 상태를 저장

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 로컬 플레이어만 입력을 처리
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>()); // 다른 플레이어의 카메라 제거
        }
        else
        {
            // JumpButton 클릭 이벤트 등록
            jumpButton.onClick.AddListener(OnJumpButtonClicked);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Joystick 및 키보드 입력 처리
        HandleMovement();
        HandleCameraRotation();

        // 스페이스바 입력으로 점프 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpButtonClicked();
        }
    }

    private void HandleMovement()
    {
        // Joystick 입력
        Vector2 moveInput = leftJoystick.InputVector;

        // 키보드 입력 추가
        float horizontalInput = Input.GetAxis("Horizontal"); // A, D 또는 화살표 좌우
        float verticalInput = Input.GetAxis("Vertical");     // W, S 또는 화살표 상하

        // Joystick과 키보드 입력을 합산
        Vector3 movement = new Vector3(moveInput.x + horizontalInput, 0, moveInput.y + verticalInput) * moveSpeed;

        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    private void HandleCameraRotation()
    {
        float rotateInput = rightJoystick.InputVector.x;

        // 카메라 회전 처리
        Vector3 rotation = new Vector3(0, rotateInput, 0) * rotationSpeed * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
    }

    private void OnJumpButtonClicked()
    {
        if (isGrounded) // Ground 상태 확인
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // 점프 후 Ground 상태 해제
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ground 태그가 있는 오브젝트 또는 부모 오브젝트와 충돌했을 때
        if (collision.gameObject.CompareTag("Ground") || collision.transform.root.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Ground 태그가 있는 오브젝트 또는 부모 오브젝트에서 벗어났을 때
        if (collision.gameObject.CompareTag("Ground") || collision.transform.root.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Photon Network를 통해 위치와 회전을 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어의 위치와 회전을 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 네트워크 플레이어의 위치와 회전을 수신
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // 네트워크 플레이어의 위치와 회전을 보간
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * 10);
        }
    }
}

