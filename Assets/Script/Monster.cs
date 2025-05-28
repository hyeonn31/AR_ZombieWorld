using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private MonsterData monsterData; // 몬스터 데이터

    [SerializeField]
    private float M_HP; // 체력
    private float M_Damage; // 공격력
    private float M_Speed; // 속도
    private float M_attackTime; // 공격 시간

    public event Action Death; // 대리자 이벤트

    private bool IsStop = false;
    private float IsStopTime = 2f;

    private float Distance; // 플레이어와의 거리
    private bool IsAttack = false; // 공격을 구분해주는 변수
    private float AttackTime = 2f; // 다시 공격하는 시간

    [SerializeField]
    private ParticleSystem HitEffect; // 피격 효과
    private bool IsDie = false;

    private GameObject PlayerObj; // 씬에서 존재하는 플레이어를 담는 변수
    private Animator monsterAnimator; // 몬스터 애니메이터
    private AudioSource monsterAudio; // 몬스터 오디오

    void Start()
    {
        monsterAnimator = this.GetComponent<Animator>();
        monsterAudio = this.GetComponent<AudioSource>();
        MonsterSetting(monsterData);
    }

    // <<--능력 설정-->>
    private void MonsterSetting(MonsterData Data)
    {
        M_attackTime = Data.AttackTime;
        M_Damage = Data.MonsterDamage;
        M_Speed = Data.MonsterSpeed;
        M_HP = Data.MonsterHP;
    }

    // <<--이동 함수 : 플레이어를 향해 이동-->>
    private void MonsterMove()
    {
        if (PlayerObj == null)
        {
            PlayerObj = GameObject.FindGameObjectWithTag("Player");

            if (PlayerObj == null)
            {
                Debug.LogError("PlayerObj를 찾을 수 없습니다. 'Player' 태그가 할당된 객체가 있는지 확인하세요.");
                return;
            }
        }

        if (!IsStop)
        {
            monsterAnimator.SetBool("Move", true);
            Distance = Vector3.Distance(this.transform.position, PlayerObj.transform.position);

            if (Distance > 0.5f) // 플레이어와의 거리가 0.5 이상일 때만 이동
            {
                Vector3 direction = (PlayerObj.transform.position - this.transform.position).normalized;
                this.transform.Translate(direction * M_Speed * Time.deltaTime, Space.World);
                this.transform.LookAt(new Vector3(PlayerObj.transform.position.x, this.transform.position.y, PlayerObj.transform.position.z));
            }
        }
    }

    // 2초 후 공격 가능 상태로 복구하는 코루틴
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(2f);
        IsAttack = false;
    }

    // <<--콜라이더끼리 부딪히고 있는 동안 호출되는 함수-->>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GunPlayer hitPlayer = other.GetComponent<GunPlayer>();

            if (hitPlayer != null && !IsAttack)
            {
                IsAttack = true;
                monsterAnimator.SetTrigger("Attack");
                StartCoroutine(hitPlayer.PlayerDMG(M_Damage));
                StartCoroutine(ResetAttack());
            }
        }
    }

    // <<--몬스터 정지 함수-->>
    private void MonsterStop()
    {
        if (IsStop)
        {
            IsStopTime -= Time.deltaTime;

            if (IsStopTime <= 0)
            {
                IsStopTime = 2f;
                IsStop = false;
            }
        }
    }

    // 맞은 시점 처리 함수
    public void Damage(Vector3 hitpos, float GunDMG)
    {
        if (!IsDie)
        {
            monsterAnimator.SetBool("Move", false);
            IsStop = true;

            HitEffect.transform.position = hitpos;
            HitEffect.Play();

            M_HP -= GunDMG;

            if (M_HP <= 0)
            {
                ShopManager.Coin += 200;
                Death?.Invoke();
                IsDie = true;
                monsterAnimator.SetTrigger("Die");
                Destroy(this.gameObject, 2f);
            }
        }
    }

    void Update()
    {
        // InvenManager의 IsStop 변수를 확인하여 몬스터의 멈춤 상태를 제어합니다.
        if (InvenManager.IsStop)
        {
            IsStop = true;
        }

        if (!IsStop)
        {
            MonsterMove();
        }
        else
        {
            MonsterStop();
        }

        if (IsAttack)
        {
            AttackTime -= Time.deltaTime;
            if (AttackTime <= 0)
            {
                IsAttack = false;
                AttackTime = 2f;
            }
        }
    }
}