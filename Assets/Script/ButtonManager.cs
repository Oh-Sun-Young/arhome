using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;
/*
    참고 사이트
    - [Unity/C#] 방금 클릭한 UI 이름, 정보 가져오기 https://yoonstone-games.tistory.com/70
    - [Unity] 유니티 게임오브젝트의 여러 자식오브젝트의 인덱스 구하기 https://ssscool.tistory.com/348
*/

public class ButtonManager : MonoBehaviour
{
    // 팝업 활성화
    public void PopupActive(bool active)
    {
        UIManager.instance.PopupActive(active);
        if (active)
        {
            GoHome();
            BannerManager.instance.BnrEnable();
        }
    }

    // 홈 팝업 활성화
    public void GoHome()
    {
        UIManager.instance.PopupContentActive("home");
    }

    // URL 이동
    public void GoURL(string url)
    {
        Application.OpenURL(url);
    }

    // 가이드 팝업 활성화
    public void GoGuide(string content)
    {
        UIManager.instance.PopupContentActive(content);
    }

    // 카테고리 팝업 활성화
    public void GoCategory(String category)
    {
        int index = CategoryIndex(category);
        UIManager.instance.PopupContentActive("category");
        UIManager.instance.CategoryActive(index);
    }

    // 선택한 카테고리 활성화
    public void OnCategory()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        int index = clickObject.transform.GetSiblingIndex();
        UIManager.instance.CategoryActive(index);
    }

    // 카테고리 인텍스 확인
    private int CategoryIndex(String category)
    {
        int index = 0;

        if (category == "Bed") index = 0;
        else if (category == "Sofa") index = 1;
        else if (category == "Table") index = 2;
        else if (category == "Chair") index = 3;
        else if (category == "Lamp") index = 4;
        else if (category == "Plant") index = 5;

        return index;
    }

    // 아이템 배치
    public void ItemPlace()
    {
        UIManager.instance.PopupActive(false);
        UIManager.instance.EditActive(true);
        GameManager.instance.ItemPlace();
    }

    // 아이템 적용
    public void ItemCheck()
    {
        UIManager.instance.EditActive(false);
        GameManager.instance.ItemCheck();
    }

    // 게임 종료
    public void GameExit()
    {
        Application.Quit();
    }
}
