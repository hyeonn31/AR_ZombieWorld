using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GunPlayer : MonoBehaviour
{
    private AudioSource PlayerAudio; // 플레이어 오디오

    [SerializeField]
    private GameObject GunShotBtn; // 공격 버튼

    [SerializeField]
    private GameObject ReloadBtn; // 장전 버튼

    public Gun gun;

    [SerializeField]
    private Text GunText; //총알 텍스트

    [SerializeField]
    private Text HP_Text; //체력 텍스트
    public float HP; //체력


    [SerializeField]
    private GameObject DMG_Pannel; //데미지 패널

    [SerializeField]
    private AudioClip DMG_Clip; //데미지 소리

    [SerializeField]
    private AudioClip Die_Clip; //죽는 소리

    [SerializeField]
    private GameObject DieUI;

    public Image TargetImage; //타겟 이미지

    private bool IsDie = false; //플레이어가 죽었는지 확인하는 변수

    private GameObject Target; //조준한 몬스터를 담는 변수
    private Vector3 GunHitPos; //총으로 맞춘 지점




    void Start()
    {
        gun = FindObjectOfType<Gun>();

        if (gun == null)
        {
            Debug.LogError("Start()에서 gun 객체를 찾을 수 없습니다. gun이 씬에 존재하는지 확인하세요.");
        }
        else
        {
            Debug.Log("gun 객체를 성공적으로 찾았습니다: " + gun.name);
        }

        HP = 100;
        // 컴포넌트 참조
        PlayerAudio = this.GetComponent<AudioSource>();

        if (PlayerAudio == null)
        {
            Debug.LogWarning("AudioSource가 없습니다. 자동으로 추가합니다.");
            PlayerAudio = gameObject.AddComponent<AudioSource>();
        }

        // 총을 여러개 교체
        // 버튼을 함수로써 등록한다.
        GunShotBtn.GetComponent<Button>().onClick.AddListener(GunShooter);
        ReloadBtn.GetComponent<Button>().onClick.AddListener(GunReload);
    }

    // 장전 함수
    public void GunReload()
    {
        //Debug.Log("GunReload입니다.");
        gun.GunReload();
    }

    //코루틴
    //다음 코드를 지정한 시간이 지난후에
    //호출할 수 있는 기능이 있다. 
    public IEnumerator PlayerDMG(float MonsterDMG)
    {
        if (!IsDie)
        {
            HP -= MonsterDMG;
            Debug.Log("HP감소됨 :" + HP);

            if (HP <= 0)
            {
                IsDie = true;
                Debug.Log("플레이어가 사망했습니다.");
            }

            DMG_Pannel.SetActive(true);
            //데미지를 입었을 때의 소리 재생
            yield return new WaitForSeconds(0.1f);
            DMG_Pannel.SetActive(false);
        }
    }

    private void PlayUI()
    {
        if (HP_Text != null && GunText != null && gun != null)
        {
            HP_Text.text = "HP: " + HP.ToString();
            GunText.text = gun.MagAmmo + "/" + gun.AmmoRemain;
        }
        else
        {
            if (HP_Text == null)
                Debug.LogError("HP_Text가 null입니다. Inspector에서 올바르게 연결되었는지 확인하세요.");

            if (GunText == null)
                Debug.LogError("GunText가 null입니다. Inspector에서 올바르게 연결되었는지 확인하세요.");

            if (gun == null)
                Debug.LogError("gun 객체가 null입니다. Start()에서 gun 객체를 초기화했는지 확인하세요.");
        }
    }


    // 총 발사 함수 호출
    public void GunShooter()
    {
        if (!IsDie)
        {
            gun.GunShot();
            Handheld.Vibrate(); //휴대폰 진동 효과

            if (Target != null)
            {
                Monster TargetMonster = Target.GetComponent<Monster>();
                TargetMonster.Damage(GunHitPos, gun.GunDamage); //
            }
        }


        // if (gun != null)
        // {
        //     gun.GunShot();
        //     //Debug.Log("GunShooter입니다.");
        // }
        // else
        // {
        //     Debug.LogWarning("gun이 초기화되지 않았습니다.");
        // }
    }

    //카메라 앞방향 레이를 쏘고, 타겟(좀비)이 존재한다면 조준점 색깔 변경 코드
    private void RayTarget()
    {
        //레이 : 보이지 않는 가상의 레이저
        //상대방이 콜라이더를 가지고 있으면 상대방의 정보를 확인 할 수 있음(상호작용)

        //Camera.main => AR Cameara => 휴대폰 카메라
        //
        Ray GunRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward); //카메라의 위치(Camera.main.transform.position)에서 시작해서, 카메라가 바라보고 있는 방향(Camera.main.transform.forward)으로 쏨 => 즉, 플레이어가 보고 있는 앞족으로 레이저가 발사됨
        RaycastHit Gunhit; //레이저가 무엇에 닿았는지의 결과를 저장할 공간


        //* Physics.Raycast : 레이저를 쏘고, 그 레이저가 어떤 객체에 닿았는지 확인, 
        //* GunRay : 내가 만든 레이저 
        //* Gunhit : 레이저가 닿았을 때 정보 담는 변수
        //* Mathf.Infinity : 레이저가 닿을 수 있는 거리 무한대로 설정
        if (Physics.Raycast(GunRay, out Gunhit, Mathf.Infinity)) //카메라에서 발사한 레이저가 어떤 물체에 닿았는지 확인하는 조건문
        {
            if (Gunhit.collider.gameObject.tag == "Monster") //레이저가 닿은 객체가 "Monster"라는 태그를 가지고 있으면
            {
                TargetImage.color = Color.red; //색 변경
                GunHitPos = Gunhit.point; //레이에 부딪힌 지점
                Target = Gunhit.collider.gameObject; //레이에 부딪힌 오브젝트
            }
        }
        else
        {
            TargetImage.color = Color.white;
            Target = null;
            GunHitPos = Vector3.zero;
        }
    }

    // 위 코드는 카메라가 바라보는 방향으로 가상의 레이저를 쏴서, 그 레이저가 어떤 객체에 닿으면 그 객체의 정보를 확인하고, 만약 그 객체가 "Monster"태그를 가진다면 
    // 조준점(TargetImage)의 색깔을 빨간색으로 바꾸는 기능을 수행함.
    // 그렇지 않으면 조준점 색깔을 흰색으로 되돌리는 코드임.

    public void ReStartBtn()
    {
        //현재 활성화중인 씬을 다시 불러온다. 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitBtn()
    {
        Application.Quit();
    }



    void Update()
    {


        if (gun == null)
        {
            gun = FindObjectOfType<Gun>();

            if (gun != null)
            {
                Debug.Log("gun 객체를 Update에서 찾았습니다: " + gun.name);
            }
        }


        RayTarget();

        if (HP >= 0)
        {
            PlayUI();
        }
        else
        {
            IsDie = true;
        }
        if (IsDie)
        {
            DieUI.SetActive(true);
        }


    }
}