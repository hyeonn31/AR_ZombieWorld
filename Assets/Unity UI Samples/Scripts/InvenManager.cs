using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 상점에서 구매한 아이템
// InvenUI 담는다.
// 담았던 아이템을 사용한다.
public class InvenManager : MonoBehaviour
{
    [SerializeField]
    private Image[] ItemImage; // 구매한 아이템을 표기할 이미지 배열

    public Text[] ItemCountText = new Text[3]; // 아이템 수 표기할 텍스트

    [SerializeField]
    private GameObject ChangeUI; // 교체 UI

    [SerializeField]
    private GameObject InvenUI; // 인벤 창

    [SerializeField]
    private Image ChangeUI_Image; // 교체 UI 교체할 아이템 이미지

    private string UseItemName; // 사용할 아이템 이름
    private GameObject ChangeGun; // 교체할 총을 담아줄 변수

    private GunPlayer gunPlayer;
    private ShopManager shopManager;

    public static bool IsStop = false; // 멈출 아이템 변수
    private float StopTime = 5f; // 멈추는 시간

    public int[] ItemCount = new int[3]; // 아이템 수를 담는 배열, 크기 설정이 중요함

    [SerializeField]
    private Transform gunParentTransform; // 총을 고정시킬 빈 게임 오브젝트

    void Start()
    {
        gunPlayer = GameObject.FindObjectOfType<GunPlayer>();

        if (gunPlayer != null)
        {
            Debug.Log("gunPlayer 오브젝트를 Start()에서 찾았습니다: " + gunPlayer.name);
        }
        else
        {
            Debug.LogError("Start()에서 gunPlayer 오브젝트를 찾을 수 없습니다. gunPlayer가 씬에 존재하는지 확인하세요.");
        }
    }

    // 상점에서 구매한 아이템을
    // 인벤 빈칸에 구매한 아이템으로 이미지 변경
    public void GetItem(string ItemName, int ItemNumber)
    {
        Sprite LoadImage = Resources.Load<Sprite>(ItemName);

        ItemImage[ItemNumber].sprite = LoadImage;
        // Resources 폴더 -> 폴더안에 오브젝트를 불러올 수 있다.
    }

    // 클릭했던 아이템을 사용한다.
    public void ItemUse()
    {
        // 인벤 안에 아이템이 들어있을 경우
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite != null)
        {
            ChangeUI.SetActive(true);
            ChangeUI_Image.sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;

            UseItemName = ChangeUI_Image.sprite.name;
        }
    }

    public void ChangeItem()
    {
        Debug.Log("ChangeItem 메서드 호출됨");
        if (UseItemName.Contains("Gun"))
        {
            if (gunPlayer == null)
            {
                gunPlayer = GameObject.FindObjectOfType<GunPlayer>();

                if (gunPlayer != null)
                {
                    Debug.Log("ChangeItem 메서드에서 gunPlayer를 찾았습니다: " + gunPlayer.name);
                }
                else
                {
                    Debug.LogError("ChangeItem 메서드에서 gunPlayer를 찾을 수 없습니다. gunPlayer 오브젝트를 확인하세요.");
                    return; // gunPlayer가 없으면 더 이상 진행하지 않음
                }
            }

            // "Gun" 태그를 가진 모든 오브젝트를 배열로 찾습니다.
            GameObject[] hasGuns = GameObject.FindGameObjectsWithTag("Gun");

            // 각 오브젝트를 순회하며 삭제합니다.
            foreach (GameObject gun in hasGuns)
            {
                Destroy(gun);
            }

            // 새로운 총을 생성합니다.
            ChangeGun = Resources.Load<GameObject>(UseItemName + "Obj");
            if (ChangeGun != null)
            {
                ChangeGun = Instantiate(ChangeGun);
                // 씬에서 존재하는 메인카메라의 자식으로 설정해줍니다.
                ChangeGun.transform.SetParent(Camera.main.transform);
                ChangeGun.transform.localPosition = new Vector3(); // 카메라의 상대 위치로 고정
                ChangeGun.transform.localRotation = Quaternion.identity; // 회전 고정
                                                                         // 씬에서 존재하는 메인카메라로 설정해준다.
                ChangeGun.transform.parent = Camera.main.transform;

                // 총의 종류에 따라서 생성되는 위치가 다르다.
                if (UseItemName == "SciFiGunLightBlueGun")
                {
                    ChangeGun.transform.localPosition = new Vector3(0.116f, -0.737f, 0.029912f);
                }
                else if (UseItemName == "G4Gun")
                {
                    ChangeGun.transform.localPosition = new Vector3(0.42f, -1.35f, 2.253f);
                }
                else if (UseItemName == "shotGun2")
                {
                    ChangeGun.transform.localPosition = new Vector3(0.1f, -1.07f, 2.054271f);
                }
                else if (UseItemName == "M16Gun")
                {
                    ChangeGun.transform.localPosition = new Vector3(0.003f, -1.335f, 1.638f);
                }
            }


        }
        else
        {
            if (UseItemName == "Ammo")
            {
                if (ItemCount[0] >= 1)
                {
                    ItemCount[0]--;
                    ItemCountText[0].text = "X" + ItemCount[0].ToString();
                    // 탄창의 총알 증가
                    gunPlayer.gun.AmmoRemain += 20;
                    if (ItemCount[0] <= 0)
                    {
                        ItemCountText[0].text = "X";
                        ItemImage[4].sprite = null;
                    }
                }
            }
            else if (UseItemName == "HP")
            {
                Debug.Log("ChangeItem - HP 아이템 사용 가능, 현재 수량: " + ItemCount[1]);

                if (ItemCount[1] >= 1)
                {
                    ItemCount[1]--;
                    ItemCountText[1].text = "X" + ItemCount[1].ToString();
                    Debug.Log("ChangeItem - HP 아이템 사용 후 남은 수량: " + ItemCount[1]);

                    if (gunPlayer != null)
                    {
                        gunPlayer.HP += 20;
                        Debug.Log("gunPlayer의 HP가 20 증가되었습니다. 현재 HP: " + gunPlayer.HP);
                    }
                    else
                    {
                        Debug.LogError("gunPlayer가 null입니다. gunPlayer 오브젝트를 찾을 수 없습니다.");
                    }

                    if (ItemCount[1] <= 0)
                    {
                        ItemCountText[1].text = "X";
                        ItemImage[5].sprite = null;  // 이미지 삭제
                        Debug.Log("ChangeItem - HP 아이템 이미지가 삭제되었습니다.");
                    }
                }
                else
                {
                    Debug.Log("ChangeItem - HP 아이템을 사용할 수 없습니다. 수량 부족: " + ItemCount[1]);
                }
            }
            else if (UseItemName == "STOP")
            {
                if (ItemCount[2] >= 1)
                {
                    ItemCount[2]--;
                    ItemCountText[2].text = "X" + ItemCount[2].ToString();
                    IsStop = true;

                    if (ItemCount[2] <= 0)
                    {
                        ItemCountText[2].text = "X";
                        ItemImage[6].sprite = null;
                    }
                }
            }
        }
        ChangeUI.SetActive(false);
    }

    void Update()
    {
        if (gunPlayer == null)
        {
            gunPlayer = GameObject.FindObjectOfType<GunPlayer>();
            if (gunPlayer != null)
            {
                Debug.Log("gunPlayer 오브젝트를 찾았습니다: " + gunPlayer.name);
            }
            else
            {
                Debug.LogError("gunPlayer 오브젝트를 여전히 찾을 수 없습니다. gunPlayer가 존재하는지 확인하세요.");
            }
        }

        if (IsStop)
        {
            StopTime -= Time.deltaTime;
            if (StopTime <= 0)
            {
                IsStop = false;
                StopTime = 5f;
            }
        }
    }
}
