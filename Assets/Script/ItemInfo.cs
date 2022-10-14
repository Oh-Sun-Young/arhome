using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [Header("Item Info")]
    [Tooltip("아이디")]
    public int itemId;
    [Tooltip("이름")]
    public string itemName;
    [Tooltip("태그")]
    public string itemTag;
    [Tooltip("이미지")]
    public Texture itemImg;
    [Tooltip("3D 모델링")]
    public GameObject itemPrefab;
    [Tooltip("좋아요")]
    public bool itemLike;

    [Header("UI")]
    public Text text;
    public RawImage img;

    private void Start()
    {
        text.text = itemName;
        img.texture = (Texture)itemImg;
    }
}
