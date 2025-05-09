using UnityEngine;

public class DragRagdoll : MonoBehaviour
{
    private Rigidbody selectedRigidbody;
    private Transform rootTransform;
    private Camera cam;
    private Vector3 offset;
    private Vector3 partOffset;
    [SerializeField] private float dragForce = 10f;
    [SerializeField] private float followSpeed = 5f;

    [SerializeField] private KeyCode getUpKey = KeyCode.Space;
    [SerializeField] private float getUpForce = 5f;
    [SerializeField] private float uprightTorque = 30f;
    private Rigidbody[] ragdollRigidbodies;
    private Transform hips;
    private bool isGettingUp;

    private class BodyPart
    {
        public Transform transform;
        public Vector3 targetPosition;
        public Quaternion targetRotation;
        public Rigidbody rb;
    }
    private BodyPart[] bodyParts;
    private bool isInTPose = false;
    [SerializeField] private float poseTransitionSpeed = 5f;

    void Start()
    {
        cam = Camera.main;
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        hips = transform.Find("Armature/Hips");

        InitializeTPoseData();
    }

    private void InitializeTPoseData()
    {
        bodyParts = new BodyPart[ragdollRigidbodies.Length];
        for (int i = 0; i < ragdollRigidbodies.Length; i++)
        {
            bodyParts[i] = new BodyPart
            {
                transform = ragdollRigidbodies[i].transform,
                targetPosition = ragdollRigidbodies[i].transform.localPosition,
                targetRotation = Quaternion.Euler(0, 0, 0),
                rb = ragdollRigidbodies[i]
            };
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                {
                    selectedRigidbody = hit.rigidbody;
                    rootTransform = selectedRigidbody.transform.root;
                    offset = rootTransform.position - hit.point;
                    partOffset = selectedRigidbody.transform.position - hit.point;
                }
            }
        }

        if (Input.GetMouseButton(0) && selectedRigidbody != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = cam.WorldToScreenPoint(selectedRigidbody.transform.position).z;

            Vector3 partTargetPos = cam.ScreenToWorldPoint(mousePos) + partOffset;
            selectedRigidbody.MovePosition(Vector3.Lerp(selectedRigidbody.position, partTargetPos, Time.deltaTime * dragForce));

            Vector3 rootTargetPos = cam.ScreenToWorldPoint(mousePos) + offset;
            rootTransform.position = Vector3.Lerp(rootTransform.position, rootTargetPos, Time.deltaTime * followSpeed);
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedRigidbody = null;
            rootTransform = null;
        }

        if (Input.GetKey(getUpKey) && !isGettingUp)
        {
            isGettingUp = true;
            StartGettingUp();
        }

        if (isGettingUp)
        {
            ApplyGetUpForce();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isInTPose = !isInTPose;
            ToggleTPose();
        }

        if (isInTPose)
        {
            UpdateTPose();
        }
    }

    private void StartGettingUp()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void ApplyGetUpForce()
    {
        if (hips != null)
        {
            Rigidbody hipsRb = hips.GetComponent<Rigidbody>();
            if (hipsRb != null)
            {
                // 힙을 위로 올리는 힘을 증가
                hipsRb.AddForce(Vector3.up * getUpForce * 2f, ForceMode.Force);
                
                // 앞쪽 방향으로도 약간의 힘을 가해서 앞으로 일어나도록 함
                hipsRb.AddForce(hipsRb.transform.forward * getUpForce * 0.5f, ForceMode.Force);
            }
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            // 현재 회전 상태 확인
            Vector3 currentForward = rb.transform.forward;
            Vector3 targetForward = Vector3.forward; // 앞쪽 방향
            
            // 위쪽 방향과 앞쪽 방향을 모두 고려하여 회전력 계산
            Vector3 upTorque = Vector3.Cross(rb.transform.up, Vector3.up);
            Vector3 forwardTorque = Vector3.Cross(currentForward, targetForward);
            
            // 회전력 적용
            rb.AddTorque(upTorque * uprightTorque);
            rb.AddTorque(forwardTorque * uprightTorque * 0.5f);

            // 각속도 제한
            rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, 5f);
            
            // 수직 속도 제한
            Vector3 velocity = rb.velocity;
            velocity.y = Mathf.Clamp(velocity.y, -5f, 5f);
            rb.velocity = velocity;
        }

        // 캐릭터가 거의 똑바로 섰을 때 일어나기 종료
        if (hips != null)
        {
            float upDot = Vector3.Dot(hips.up, Vector3.up);
            float forwardDot = Vector3.Dot(hips.forward, Vector3.forward);
            
            if (upDot > 0.9f && Mathf.Abs(forwardDot) > 0.7f)
            {
                isGettingUp = false;
            }
        }
    }

    private void ToggleTPose()
    {
        foreach (var bodyPart in bodyParts)
        {
            // 다리 부분은 isKinematic을 false로 유지
            if (IsLegPart(bodyPart.transform.name))
            {
                bodyPart.rb.isKinematic = false;
            }
            else
            {
                bodyPart.rb.isKinematic = isInTPose;
            }
            
            bodyPart.rb.velocity = Vector3.zero;
            bodyPart.rb.angularVelocity = Vector3.zero;
        }
    }

    // 다리 부분인지 확인하는 헬퍼 메서드
    private bool IsLegPart(string partName)
    {
        return partName.Contains("Leg") || 
               partName.Contains("Foot") || 
               partName.Contains("Thigh") ||
               partName.Contains("Knee");
    }

    private void UpdateTPose()
    {
        foreach (var bodyPart in bodyParts)
        {
            // 다리 부분은 포즈 업데이트 제외
            if (!IsLegPart(bodyPart.transform.name))
            {
                bodyPart.transform.localRotation = Quaternion.Lerp(
                    bodyPart.transform.localRotation,
                    bodyPart.targetRotation,
                    Time.deltaTime * poseTransitionSpeed
                );
                bodyPart.transform.localPosition = Vector3.Lerp(
                    bodyPart.transform.localPosition,
                    bodyPart.targetPosition,
                    Time.deltaTime * poseTransitionSpeed
                );
            }
        }
    }
}