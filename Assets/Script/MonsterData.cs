using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터의 정보를 담을 수 있는 스크립터블 오브젝트
[CreateAssetMenu(menuName = "ScriptableObject/MonsterData", fileName = "MonsterData")]
public class MonsterData : ScriptableObject
{
    public float MonsterHP; //체력
    public float MonsterDamage; //공격력
    public float MonsterSpeed; //속도
    public float AttackTime; // 공격 속도 
    public AudioClip MonsterDieClip;
    public AudioClip MonsterAttackClip;
    public AudioClip MonsterHitClip;
}
