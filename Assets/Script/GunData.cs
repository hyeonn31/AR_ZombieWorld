using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//총에 관한 정보를 담는 스크립트
[CreateAssetMenu(menuName = "ScriptableObject/GunData", fileName = "GunData")]
public class GunData : ScriptableObject //ScriptableObject -> 간단한 데이터를 오브젝트 형식으로 담는다. 
{
    public AudioClip ShotClip; //발사소리
    public AudioClip ReloadClip; //장전소리
    public float GunDamage = 10f; //총 공격력
    public int AmmoRemain = 100; //남아있는 총알 수
    public int MagAmmo = 30; //탄창안에 들어가는 총알 수 
    public float GunAttackTime = 0.12f; //총알 발사 간격 시간
    public float ReloadTime=1.8f; //장전 시간
}
