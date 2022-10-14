using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }
    private static GameManager m_instance;

    public ItemInfoAll[] itemInfoAll; // 생성할 아이템의 원본 정보
    public GameObject[] itemPrefab; // 아이템의 원본 프리팹
    public GameObject itemNone; // 데이터 없는 경우의 원본 프리팹
    public Transform[] itemListHome; // 홈에서의 아이템 위치
    public Transform[] itemListCategory; // 카테고리에서의 아이템 위치
    private int[] itemCnt; // 카테고리별 아이템 갯수

    public int idDetailView; // 현재 디테일 페이지의 아이디

    public ARRaycastManager manager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public Transform placePool; // 배치 아이템 위치
    private Vector2 placeCenter; // 배치 위치
    public List<GameObject> placeList = new List<GameObject>(); // 배치된 아이템
    private GameObject select;
    private GameObject selectItem;


    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        itemCnt = new int[itemListCategory.Length];

        for (int i = 0; i < itemInfoAll.Length; i++)
        {
            int index = (int)itemInfoAll[i].category;
            itemCnt[index]++;

            for (int j = 0; j < itemPrefab.Length; j++)
            {
                if (j == 0 && itemCnt[index] > 3)
                {
                    continue;
                }
                else
                {
                    Transform itemPosition;
                    if (j == 0) itemPosition = itemListHome[index];
                    else itemPosition = itemListCategory[index];

                    GameObject obj = Instantiate(itemPrefab[j], itemPosition);
                    CopyInfo(obj, i);
                }
            }
        }

        for (int i = 0; i < itemCnt.Length; i++)
        {
            if (itemCnt[i] == 0)
            {
                Destroy(itemListHome[i].parent.parent.parent.gameObject);
                Instantiate(itemNone, itemListCategory[i].parent);
                Destroy(itemListCategory[i].gameObject);
            }
            else if (itemCnt[i] < 3)
            {
                Destroy(itemListHome[i].GetChild(0).gameObject);
            }
        }
    }

    // 아이템 배치
    public void ItemPlace()
    {
        select = Instantiate(itemInfoAll[idDetailView].itemPrefab);

        UIManager.instance.arObjectToSpawn = select;

        placeList.Add(select);

        StartCoroutine("ItemSet");
    }

    // 아이템 위치
    IEnumerator ItemSet()
    {
        while (true)
        {
            placeCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            if (manager.Raycast(placeCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                ARPlane plane = hits[0].trackable.GetComponent<ARPlane>();
                if (plane != null)
                {
                    select.transform.position = hits[0].pose.position;
                    select.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    select.transform.position = new Vector3(0, 0, 0);
                }
            }
            yield return null;
        }
    }

    // 아이템 적용
    public void ItemCheck()
    {
        StopCoroutine("ItemSet");

        select.transform.localScale = new Vector3(1, 1, 1);
        select.transform.SetParent(placePool);

        select.GetComponent<LeanTwistRotateAxis>().enabled = false;

        select = null;
    }

    // 아이템 정보 복사
    void CopyInfo(GameObject obj, int num)
    {
        ItemInfo itemInfo = obj.GetComponent<ItemInfo>();
        itemInfo.itemId = num;
        itemInfo.itemName = itemInfoAll[num].itemName;
        itemInfo.itemTag = itemInfoAll[num].itemTag;
        itemInfo.itemImg = itemInfoAll[num].itemImg;
        itemInfo.itemPrefab = itemInfoAll[num].itemPrefab;
        itemInfo.itemLike = itemInfoAll[num].itemLike;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
