using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;  // 씬 관리를 위해 추가

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public Joystick joystick;
    public CinemachineVirtualCamera virtualCamera;
    public float cameraSpeed = 5f;
    public float rotationSpeed = 100f; // 카메라 회전 속도
    public float minVerticalAngle = -30f; // 최소 수직 각도
    public float maxVerticalAngle = 60f; // 최대 수직 각도
    public GameObject retryButton; // Retry 버튼 UI 오브젝트
    public GameObject endButton; // End 버튼 UI 오브젝트로 이름 변경
    public GameObject startButton; // Start 버튼 UI 오브젝트

    private Transform cameraFollow;
    private GameObject player;
    private float currentVerticalAngle = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (virtualCamera != null && player != null)
        {
            // 빈 게임오브젝트를 카메라 피벗으로 생성
            GameObject cameraPivot = new GameObject("CameraPivot");
            cameraPivot.transform.position = player.transform.position;
            cameraPivot.transform.parent = player.transform;
            
            virtualCamera.Follow = cameraPivot.transform;
            virtualCamera.LookAt = player.transform;
            cameraFollow = cameraPivot.transform;
        }

        // 씬 시작 시 항상 Retry 버튼 숨기기
        HideRetryButton();

        // 씬 시작 시 Start 버튼 보이기 (인트로씬에서)
        ShowStartButton();

        // AdRequest가 씬에 있으면 초기화
        var adRequest = FindObjectOfType<AdRequest>();
        if (adRequest != null)
        {
            AdManager.Instance.adRequest = adRequest;
        }
    }

    void Update()
    {
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        if (joystick == null || cameraFollow == null) return;

        Vector2 input = joystick.InputVector;

        // 수평 회전
        cameraFollow.RotateAround(player.transform.position, Vector3.up, input.x * rotationSpeed * Time.deltaTime);

        // 수직 회전 (제한된 각도 범위 내에서)
        currentVerticalAngle -= input.y * rotationSpeed * Time.deltaTime;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
        
        // 카메라 피벗의 로컬 회전 설정
        Vector3 targetRotation = cameraFollow.localEulerAngles;
        targetRotation.x = currentVerticalAngle;
        cameraFollow.localEulerAngles = targetRotation;
    }

    public void ShowRetryButton()
    {
        if (retryButton != null)
        {
            retryButton.SetActive(true);
        }
    }

    private void HideRetryButton()
    {
        if (retryButton != null)
        {
            retryButton.SetActive(false);
        }
    }

    public void OnRetryButtonClick()
    {
        if (retryButton != null)
        {
            retryButton.SetActive(false);
        }
        ResetGame();
    }

    private void ResetGame()
    {
        HideRetryButton();
        // 현재 활성화된 씬을 다시 로드
        SceneManager.LoadScene(0);
    }

    public void ShowEndButton()
    {
        if (endButton != null)
        {
            endButton.SetActive(true);
        }
    }

    private void HideEndButton()
    {
        if (endButton != null)
        {
            endButton.SetActive(false);
        }
    }

    public void OnEndButtonClick()
    {
        if (endButton != null)
        {
            endButton.SetActive(false);
        }
        QuitGame();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowStartButton()
    {
        if (startButton != null)
        {
            startButton.SetActive(true);
        }
    }

    private void HideStartButton()
    {
        if (startButton != null)
        {
            startButton.SetActive(false);
        }
    }

    public void OnStartButtonClick()
    {
        if (startButton != null)
        {
            startButton.SetActive(false);
        }
        LoadMainScene();
    }

    private void LoadMainScene()
    {
        // "Main" 씬 이름이 정확히 일치해야 합니다.
        SceneManager.LoadScene("Main");
    }
}
