using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

// 각 스테이지마다 다른 몬스터를 생성
//1단계 :
//2단계 :
//3단계 :
//각 단계마다 몬스터의 수가 다르게 지정한다. 

public class MonsterCreate : MonoBehaviour
{

    private bool IsStart = false; //게임이 시작됐는지
    public static int Stage = 1; //단계

    [SerializeField]
    private Text StageText;
    [SerializeField]
    private Text GameText; //게임 카운터 텍스트

    public List<GameObject> MonsterList = new List<GameObject>();
    // 신에서 존재하는 몬스터를 담는 리스트
    //리스트 -> 편리한 배열
    //배열 -> 담을 수 있는 수 미리지정
    //리스트 -> 자동으로 담는공간을 확장해줌. +찾는 기능 +지우는 기능

    private int MonsterCount = 15;
    //15 30 45 점점 증가하게 만들자

    private bool IsCreate = false; //몬스터를 생성

    [SerializeField]
    private float GameStartTime = 3f;

    [SerializeField]
    private GameObject StartUI;
    [SerializeField]
    private GameObject NextUI;

    [SerializeField]
    private GameObject ClearUI;

    [SerializeField]
    private GameObject[] StageMonsters; //단계마다 생성할 몬스터 

    private const int MaxStage = 3; // 게임의 최대 스테이지 수

    void Start()
    {

    }

    public void NextBtn()
    {
        Debug.Log($"NextBtn 호출됨. Stage: {Stage}, IsStart: {IsStart}, IsCreate: {IsCreate}, MonsterList Count: {MonsterList.Count}");

        // 초기화 작업
        StartUI.SetActive(true);
        IsStart = false;
        IsCreate = false;
        GameStartTime = 3f;

        // 현재 스테이지가 마지막 스테이지라면 게임 클리어 UI를 표시
        if (Stage >= MaxStage)
        {
            ClearUI.SetActive(true);
            Debug.Log("모든 스테이지 클리어! ClearUI 활성화");
        }
        else
        {
            Stage++;
            MonsterList.Clear(); // 몬스터 리스트를 초기화
            Debug.Log("몬스터 리스트 초기화 완료. 다음 스테이지로 이동합니다.");

            NextUI.SetActive(false);
            GameText.text = "READY?:" + GameStartTime.ToString("F1");

            // 다음 스테이지 시작
            GameStartBTN();
        }
    }

    void Update()
    {
        if (IsStart)
        {
            GameStartTime -= Time.deltaTime;
            GameText.text = "READY?:" + GameStartTime.ToString("F1");

            if (GameStartTime <= 0)
            {
                StartUI.SetActive(false);
                if (!IsCreate)
                {
                    StageCreateM();
                }
                else
                {
                    // 몬스터가 모두 소멸했을 경우
                    if (MonsterList.Count <= 0)
                    {
                        if (Stage >= MaxStage)
                        {
                            // 마지막 스테이지 클리어 시 ClearUI를 표시
                            ClearUI.SetActive(true);
                            Debug.Log("모든 스테이지 클리어 완료. ClearUI 활성화");
                        }
                        else
                        {
                            // 다음 스테이지를 진행하기 위해 NextUI를 표시
                            NextUI.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    // <<--스테이지에 따라서 몬스터를 생성해주는 함수-->
    private void StageCreateM()
    {
        IsCreate = true;

        for (int i = 0; i < MonsterCount * Stage; i++)
        {
            float Create_X = 0f;
            float Create_Z = 0f;

            int RandomRate = Random.Range(0, 4);

            if (RandomRate == 0)
            {
                Create_X = Random.Range(-20, -60);
                Create_Z = Random.Range(-20, -60);
            }
            else if (RandomRate == 1)
            {
                Create_X = Random.Range(20, 60);
                Create_Z = Random.Range(20, 60);
            }
            else if (RandomRate == 2)
            {
                Create_X = Random.Range(20, 60);
                Create_Z = Random.Range(-20, -60);
            }
            else if (RandomRate == 3)
            {
                Create_X = Random.Range(-20, -60);
                Create_Z = Random.Range(20, 60);
            }

            Monster CreateMonster = Instantiate(StageMonsters[Stage - 1].GetComponent<Monster>(),
                new Vector3(Create_X, -1f, Create_Z), Quaternion.identity);

            // 생성한 몬스터를 몬스터 리스트에 추가
            MonsterList.Add(CreateMonster.gameObject);

            // 몬스터가 죽을 때 리스트에서 제거
            CreateMonster.Death += () => MonsterList.Remove(CreateMonster.gameObject);
        }
    }
    public void GameStartBTN()
    {
        if (!IsStart)
        {
            IsStart = true;
        }
    }
}
