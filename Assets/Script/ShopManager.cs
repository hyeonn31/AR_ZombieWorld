using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Xml.Serialization;
using Unity.VisualScripting;


//무기를 구매
//아이템 구매(체력, 총알, 공격력)
public class ShopManager : MonoBehaviour
{



    [SerializeField]
    private GameObject WeaponPannel;

    [SerializeField]
    private GameObject ItemPannel;

    [SerializeField]
    private Text CoinText; //코인 표기 텍스트
    private AudioSource ShopAudio; //상점소리
    [SerializeField]
    private AudioClip ShopClip;
    [SerializeField]
    private InvenManager invenManager;

    public static int Coin = 10000; //코인


    void Start()
    {
        CoinText.text = "Coin :" + Coin.ToString();

        if (invenManager == null)
        {
            invenManager = FindObjectOfType<InvenManager>();
        }

        if (invenManager == null)
        {
            Debug.LogError("InvenManager를 찾을 수 없습니다. Inspector에서 할당되었는지 확인하세요.");
        }
        else
        {
            // ItemCount 배열 크기 확인 및 강제 초기화
            if (invenManager.ItemCount == null || invenManager.ItemCount.Length < 3)
            {
                Debug.LogWarning("ItemCount 배열 크기가 잘못되었습니다. 크기를 3으로 초기화합니다.");
                invenManager.ItemCount = new int[3]; // 크기 강제 초기화
            }

            for (int i = 0; i < invenManager.ItemCountText.Length; i++)
            {
                if (invenManager.ItemCountText[i] == null)
                {
                    Debug.LogError($"ItemCountText[{i}]가 null입니다. Inspector에서 올바르게 연결되어 있는지 확인하세요.");
                }
                else
                {
                    Debug.Log($"ItemCountText[{i}]가 올바르게 설정되었습니다.");
                }
            }
        }

        ShopAudio = GetComponent<AudioSource>();
    }





    //코인
    //구매할 아이템 가격 높다면 구매
    //그렇지 않다면 구매하지 못한다.
    public void PurChase(string ItemName)
    {
        Debug.Log("ItemName: " + ItemName); // 전달된 ItemName 확인

        if (ItemName == "SciFiGunLightBlueGun")
        {
            if (Coin >= 2000)
            {
                Coin -= 2000;
                CoinText.text = "Coin :" + Coin.ToString(); // 코인 UI 갱신
                ShopAudio.PlayOneShot(ShopClip);
                invenManager.GetItem(ItemName, 1);
                EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                //코인이 부족한 경우

            }
        }
        else if (ItemName == "G4Gun")
        {
            if (Coin >= 1000)
            {
                Coin -= 1000;
                CoinText.text = "Coin :" + Coin.ToString(); // 코인 UI 갱신
                ShopAudio.PlayOneShot(ShopClip);
                invenManager.GetItem(ItemName, 2);
                EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                //코인이 부족한 경우

            }
        }
        else if (ItemName == "shotGun2")
        {
            if (Coin >= 2500)
            {
                Coin -= 2500;
                CoinText.text = "Coin :" + Coin.ToString(); // 코인 UI 갱신
                ShopAudio.PlayOneShot(ShopClip);
                invenManager.GetItem(ItemName, 3);
                EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                //코인이 부족한 경우

            }
        }
        else if (ItemName == "Ammo")
        {
            if (Coin >= 1500)
            {
                Coin -= 1500;
                CoinText.text = "Coin :" + Coin.ToString(); // 코인 UI 갱신

                // 인벤에 전달
                invenManager.GetItem(ItemName, 4);
                ShopAudio.PlayOneShot(ShopClip);

                // 배열 인덱스 0 사용
                if (invenManager.ItemCount.Length > 0)
                {
                    invenManager.ItemCount[0]++;
                    Debug.Log("Ammo 구매 후 수량: " + invenManager.ItemCount[0]);

                    if (invenManager.ItemCountText.Length > 0 && invenManager.ItemCountText[0] != null)
                    {
                        invenManager.ItemCountText[0].text = "X" + invenManager.ItemCount[0];
                        Debug.Log("Ammo Count Text 업데이트: " + invenManager.ItemCountText[0].text);
                    }
                    else
                    {
                        Debug.LogError("ItemCountText[0]이 null이거나 배열 크기보다 큽니다. Inspector에서 Text 컴포넌트가 올바르게 연결되어 있는지 확인하세요.");
                    }
                }
                else
                {
                    Debug.LogError("ItemCount 배열의 크기가 충분하지 않습니다. Inspector에서 배열 크기를 확인하세요.");
                }
            }
        }
        else if (ItemName == "HP")
        {
            if (Coin >= 2000)
            {
                Coin -= 2000;
                CoinText.text = "Coin :" + Coin.ToString(); // 코인 UI 갱신

                // 인벤에 전달
                invenManager.GetItem(ItemName, 5);
                ShopAudio.PlayOneShot(ShopClip);

                // 배열 인덱스 2 사용
                if (invenManager.ItemCount.Length > 1)
                {
                    invenManager.ItemCount[1]++;
                    Debug.Log("HP 구매 후 수량: " + invenManager.ItemCount[1]);

                    if (invenManager.ItemCountText.Length > 1 && invenManager.ItemCountText[1] != null)
                    {
                        invenManager.ItemCountText[1].text = "X" + invenManager.ItemCount[1];
                        Debug.Log("HP Count Text 업데이트: " + invenManager.ItemCountText[1].text);
                    }
                    else
                    {
                        Debug.LogError("ItemCountText[1]이 null이거나 배열 크기보다 큽니다. Inspector에서 Text 컴포넌트가 올바르게 연결되어 있는지 확인하세요.");
                    }
                }
                else
                {
                    Debug.LogError("ItemCount 배열의 크기가 충분하지 않습니다. Inspector에서 배열 크기를 확인하세요.");
                }
            }
        }

        else if (ItemName == "STOP")
        {
            if (Coin >= 2500)
            {
                Coin -= 2500;
                CoinText.text = "Coin :" + Coin.ToString(); // 코인 UI 갱신

                // 인벤에 전달
                invenManager.GetItem(ItemName, 6);
                ShopAudio.PlayOneShot(ShopClip);

                // 배열 인덱스 2 사용
                if (invenManager.ItemCount.Length > 2)
                {
                    invenManager.ItemCount[2]++;
                    Debug.Log("STOP 구매 후 수량: " + invenManager.ItemCount[2]);

                    if (invenManager.ItemCountText.Length > 2 && invenManager.ItemCountText[2] != null)
                    {
                        invenManager.ItemCountText[2].text = "X" + invenManager.ItemCount[2];
                        Debug.Log("STOP Count Text 업데이트: " + invenManager.ItemCountText[2].text);
                    }
                    else
                    {
                        Debug.LogError("ItemCountText[2]이 null이거나 배열 크기보다 큽니다. Inspector에서 Text 컴포넌트가 올바르게 연결되어 있는지 확인하세요.");
                    }
                }
                else
                {
                    Debug.LogError("ItemCount 배열의 크기가 충분하지 않습니다. Inspector에서 배열 크기를 확인하세요.");
                }
            }
        }


    }
    void Update()
    {
        CoinText.text = "Coin: " + Coin.ToString();
    }
}
