using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    참고 사이트
    - [Unity] 인스펙터를 정리해보자 : https://m.blog.naver.com/2983934/221428284978
    - Inspector 뷰에서 공개된 enum의 값을 여러 개 선택 가능하게 하기 : https://wergia.tistory.com/104
    - Unity Inspector 창에 public class 객체 나타나도록 하기 : https://dana3711.tistory.com/26
*/
[System.Serializable] // 타클래스에서 []배열로 선언되었을 경우 Inspector에서 값 입력 가능
public class ItemInfoAll
{
    public enum type
    {
        Bed,
        Sofa,
        Table,
        Chair,
        Lamp,
        Plant
    }

    [Tooltip("분류")]
    public type category;
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
}
