using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Training : MonoBehaviour
{
    public TextMeshProUGUI hp;
    public Slider bar;

    public Animator anim;

    private int hpValue = 0;
    private bool isTraining = false;

    private bool isLBPressed = false;
    private bool isRBPressed = false;
    private bool isBlendActive = false;  // 블렌드 상태를 추적하는 변수 추가

    public void OnLBButtonDown()
    {
        isLBPressed = true;
        IncreaseHP();
    }

    public void OnLBButtonUp()
    {
        isLBPressed = false;
    }

    public void OnRBButtonDown()
    {
        isRBPressed = true;
        isBlendActive = !isBlendActive;  // 블렌드 상태 토글
        IncreaseHP();
    }

    public void OnRBButtonUp()
    {
        isRBPressed = false;
    }

    void Update()
    {
        // PC 입력 처리
        if (Input.GetAxisRaw("Horizontal") != 0 || 
            Input.GetAxisRaw("Vertical") != 0 || 
            Input.GetKey(KeyCode.Space))
        {
            IncreaseHP();
        }

        HpView();
        Anim();
        Move();
    }

    void HpView()
    {
        hp.text = "FEVER: " + hpValue.ToString();
        // HP 값을 0-1 사이의 값으로 정규화하여 슬라이더에 적용
        bar.value = hpValue / 100f;
    }

    private void IncreaseHP()
    {
        if (hpValue < 100)
        {
            hpValue++;
            if (hpValue >= 100)
            {
                // HP가 100이 되면 End 버튼 표시
                GameManager.Instance.ShowEndButton();
            }
        }
    }

    public void ResetHP()
    {
        hpValue = 0;
        bar.value = 0;
        hp.text = "FEVER: 0";
    }

    void Anim()
    {
        if (hpValue >= 50 || bar.value > 0.5f) // HP가 50이상이면 서있는 상태가 됨
        {
            anim.SetLayerWeight(1,0);
        }
        else
        {
            anim.SetLayerWeight(1,1);
        }
    }

    void Move()
    {
        // HP가 100이면 모든 애니메이션을 idle 상태로 리셋
        if (hpValue >= 100)
        {
            anim.SetBool("SitUp", false);
            anim.SetFloat("Blend", 0);
            isBlendActive = false;
            return;
        }

        // LB 버튼이나 키보드 입력으로 SitUp 애니메이션 제어
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || isLBPressed)
        {
            anim.SetBool("SitUp", true);
        }
        else
        {
            anim.SetBool("SitUp", false);
        }
        
        // RB 버튼이나 스페이스바로 Blend 애니메이션 제어
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isBlendActive = !isBlendActive;
        }

        anim.SetFloat("Blend", isBlendActive ? 1 : 0);
    }
}
