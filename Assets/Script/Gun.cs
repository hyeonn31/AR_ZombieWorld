using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

//1. 총에 대한 데이터 정보를 받아온다. 
//2. 총 발사
//3. 총 장전
//4. 남은 총알을 탄창에 최대한 채우는 장전 로직

public class Gun : MonoBehaviour
{

    //함수 밖에서 선언된 변수
    //멤버 변수
    //접근제한지정자(private,public) 자료형 변수 이름 = 그 자료형에 맞게 값을 (초기값)
    //private : 내부 public : 외부

    public GunData gundata; //총 정보를 담는 데이터
    public int AmmoRemain; //가지고 있는 총알 수 
    public int MagAmmo; //탄창 수 
    public float GunDamage; //공격력

    private float LastAttackTime; //마지막으로 총을 쏜 시점
    private float ReloadTime; //장전 시간
    private float GunAttackTime; //총알 발사 간격 시간

    private AudioSource GunAudio; //총 오디오

    [SerializeField] private ParticleSystem FlashEffect; //총 발사 파티클
    [SerializeField] private ParticleSystem ShellEffect; //총알 떨어지는 파티클

    private bool IsReady = true;


    [SerializeField]
    private string W_Name;


    void Start()
    {
        GunAudio = this.GetComponent<AudioSource>();
        GunSetting(gundata);
    }

    //<------------1. 총에 대한 데이터 정보를 받아오는 함수-------------->
    [SerializeField]
    private void GunSetting(GunData gundata)
    {
        GunDamage = gundata.GunDamage;
        AmmoRemain = gundata.AmmoRemain;
        MagAmmo = gundata.MagAmmo;
        ReloadTime = gundata.ReloadTime;
        GunAttackTime = gundata.GunAttackTime;

    }
    //<--------------------2. 총 쏘는 함수------------------------->
    public void GunShot()
    {
        if (IsReady)
        {
            //1. 탄창에 총알이 들어있을 경우
            //2. 마지막으로 총을 발사했던 시점이 총알 다시 발사하는 간격 시점을 넘길경우
            if (MagAmmo > 0 && Time.time > LastAttackTime + GunAttackTime)
            {

                FlashEffect.Play();
                if (ShellEffect != null)
                    ShellEffect.Play();
                MagAmmo--;
                GunAudio.PlayOneShot(gundata.ShotClip);

                if (W_Name == "G4GunObj(Clone)")
                {
                    StartCoroutine(G4_ON());
                }
            }
        }
    }

    private IEnumerator G4_ON()
    {
        this.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(1f);
        this.GetComponent<SphereCollider>().enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Monster" && W_Name == "G4GunObj(Clone)")
        {
            Monster hitMonster = other.GetComponent<Monster>();
            if (hitMonster != null)
            {
                hitMonster.Damage(other.ClosestPoint(this.transform.position), 100);
            }
        }
    }

    //<-------------------3.총알을 장전하는 함수---------------------->
    public void GunReload()
    {
        if (IsReady)
        {
            //1. 남아있는 총알이 있는 경우
            //2. 탄창에 총알이 가득차지 않는 경우
            //3. 장전중일 때 -> 다시 장전
            if (AmmoRemain > 0 || MagAmmo < gundata.MagAmmo)
            {
                //장전을 하겠다.
                StartCoroutine(GunReloadTime());
            }
        }
    }

    //코루틴
    //지정한 시간이 지난후에
    //다음 코드를 호출할 수 있다. 

    //<---------------4. 남은 총알을 탄창에 최대한 채우는 장전 로직--------------->
    //남아있는 총알이 부족하면 남은 만큼 탄창에 넣고, 충분하면 탄창을 가득 채운 뒤 남은 총알을 차감하는 기능을 구현
    private IEnumerator GunReloadTime()
    {
        IsReady = false;
        //장전소리 재생
        GunAudio.PlayOneShot(gundata.ReloadClip);
        //장전하는 시간이 지난 후에
        yield return new WaitForSeconds(ReloadTime);

        // 장전해야 할 총알수
        // 가득찬 탄창에 총알수 - 탄창에 들어있는 총알수
        // ex) 30 - 15 = 15 -> 15발 충전
        int ammoFill = gundata.MagAmmo - MagAmmo;

        //충전해야할 총알수
        //남아있는 총알(AmmoRemain)이 충전해야 할 총알(ammoFill)수 보다 적을 경우 
        if (AmmoRemain < ammoFill)
        {
            //충전할 수 있는 만큼만 탄창에 채움
            MagAmmo += AmmoRemain;
            //남은 총알 모두 사용
            AmmoRemain = 0;
        }
        else
        {
            //탄창에 총알 수 충전
            MagAmmo += ammoFill;

            //남아있는 총알 수 뺀다
            AmmoRemain -= ammoFill;

        }

        IsReady = true;
    }

    //설명:
    // ammoFill은 가득 채울 때 필요한 총알의 개수
    // 남아있는 총알(AmmoRemain)이 ammoFill보다 적을 경우, 남아있는 총알을 모두 탄창에 넣고, AmmoRemain은 0으로 설정
    // 반대로 남아있는 총알이 충분할 경우, 탄창을 가득 채우고 남은 총알을 줄임.
    // 이제, 예를 들어 AmmoRemain이 5이고, MagAmmo가 10 남았을 때 장전을 누르면, MagAmmo는 15가 되고, AmmoRemain은 0이 된다.




    // Update is called once per frame
    void Update()
    {

    }
}
