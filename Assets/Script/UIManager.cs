using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.Controls.AxisControl;
using System.Linq;

/*
 * 참고 사이트
 * - Implemented a function to share screen shots to SNS [Unity / Native Share for Android & iOS] : https://youtu.be/6YePDa3rEbU
 * - 유니티에서 게임 오브젝트 폭, 높이 구하는 방법 : https://dreamaz.tistory.com/1118
 * - 두 개 이상의 카메라 사용 : https://docs.unity3d.com/kr/2020.3/Manual/MultipleCameras.html
 * - AR plane manager : https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/plane-manager.html
 * - Use touch to scale, rotate and drag AR objects https://www.youtube.com/watch?v=jgQVUttENTI
*/

public class UIManager : MonoBehaviour
{
    public static UIManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }
            return m_instance;
        }
    }
    private static UIManager m_instance;

    [Header ("HUD Name")]
    public GameObject blur;
    public GameObject notice;
    private CanvasGroup noticeCanvasGroup;
    public GameObject intro;
    private CanvasGroup introCanvasGroup;
    public GameObject basicButton;
    public GameObject editButton;
    public GameObject popupAllMenu;
    public GameObject home;
    public GameObject category;
    public GameObject detail;
    public GameObject detailCanvas;
    private CanvasGroup popupCanvasGroup;

    [Header("UI : Home")]
    public GameObject warning;
    public GameObject howToUse;
    public GameObject howToShare;

    [Header("UI : Category")]
    public GameObject[] categoryTab;
    public GameObject categoryView;
    public Transform categoryLine;

    [Header("UI : Detail")]
    public Text detailTitle;
    public Text detailTag;
    public Transform detailPool;
    private GameObject detailObject;

    [Header("AR")]
    public GameObject arSessionOrigin;
    private ARPlaneManager arPlaneManager;
    private ARPointCloudManager arPointCloudManager;
    public GameObject arObjectToSpawn;
    private GameObject spawnedObject;
    private Pose PlacementPose;
    private bool placementPoseIsValid = false;

    [Header("Camera")]
    public Camera arCamera;
    public Camera derailCamera;

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }

        arPlaneManager = arSessionOrigin.GetComponent<ARPlaneManager>();
        arPointCloudManager = arSessionOrigin.GetComponent<ARPointCloudManager>();

        noticeCanvasGroup = notice.GetComponent<CanvasGroup>();
        introCanvasGroup = intro.GetComponent<CanvasGroup>();
        popupCanvasGroup = popupAllMenu.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        noticeCanvasGroup.alpha = 0;
        introCanvasGroup.alpha = 0;

        blur.SetActive(true);
        notice.SetActive(true);
        intro.SetActive(false);
        basicButton.SetActive(false);
        editButton.SetActive(false);
        detail.SetActive(false);
        warning.SetActive(false);
        howToUse.SetActive(false);
        howToShare.SetActive(false);

        popupCanvasGroup.alpha = 0;
        popupAllMenu.transform.localPosition = new Vector2(0, -Screen.height);
        ArPlaneActive(false);

        detailCanvas.SetActive(false);
        detailObject = null;

        ChangeCamera(true);

        noticeCanvasGroup.LeanAlpha(1, 0.75f);
        Invoke("IntroActive", 3f);
    }
    void Update()
    {
        if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
        }
    }
    // 인트로&타이틀 화면
    private void IntroActive()
    {
        if(noticeCanvasGroup.alpha == 1) noticeCanvasGroup.LeanAlpha(0, 0.5f);
        if(introCanvasGroup.alpha == 0) introCanvasGroup.LeanAlpha(1, 1f);

        blur.SetActive(true);
        notice.SetActive(false);
        intro.SetActive(true);
        basicButton.SetActive(true);
        ArPlaneActive(false);
    }

    // 바닥 이미지 활성화 여부
    private void ArPlaneActive(bool active)
    {
        arPlaneManager.enabled = active;
        arPointCloudManager.enabled = active;
    }

    // 팝업 활성화 여부
    public void PopupActive(bool active)
    {
        float alpha;
        float position;
        bool place;

        if (active == true)
        {
            alpha = 1;
            position = 0;
        }
        else
        {
            alpha = 0;
            position = -Screen.height;
            detailCanvas.SetActive(false);

            if (GameManager.instance.placeList.Count > 0) place = false;
            else place = true;

            blur.SetActive(place);
            intro.SetActive(place);
            basicButton.SetActive(true);
        }
        popupCanvasGroup.LeanAlpha(alpha, 0.5f);
        popupAllMenu.transform.LeanMoveLocalY(position, 0.5f).setEaseOutExpo().delay = 0.1f;
        ChangeCamera(true);
    }

    // 선택된 팝업 내용
    public void PopupContentActive(string content)
    {
        home.SetActive(false);
        category.SetActive(false);
        detail.SetActive(false);
        detailCanvas.SetActive(false);
        warning.SetActive(false);
        howToUse.SetActive(false);
        howToShare.SetActive(false);

        if (content == "home")
        {
            home.SetActive(true);
        }
        if (content == "category")
        {
            category.SetActive(true);
        }
        if (content == "detail")
        {
            detail.SetActive(true);
            detailCanvas.SetActive(true);

            blur.SetActive(false);
            intro.SetActive(false);
            basicButton.SetActive(false);
        }
        if (content == "warning")
        {
            warning.SetActive(true);
        }
        if (content == "howToUse")
        {
            howToUse.SetActive(true);
        }
        if (content == "howToShare")
        {
            howToShare.SetActive(true);
        }
    }

    // 선택된 카테고리 활성화
    public void CategoryActive(int index)
    {
        // 변수 값 설정
        float alpha;
        float position = -Screen.width * index;
        float menuWidth = categoryTab[index].GetComponent<RectTransform>().rect.width;
        float menuPosition = categoryTab[index].transform.position.x - menuWidth / 2;

        // 페이지 위치 이동
        categoryView.transform.LeanMoveX(position, 0.5f);

        // 카테고리 상태 변경
        for (int i = 0; i < categoryTab.Length; i++)
        {
            if (i == index) alpha = 1f;
            else alpha = 0.25f;

            categoryTab[i].GetComponent<CanvasGroup>().LeanAlpha(alpha, 0.5f);
        }
        categoryLine.LeanScaleX(menuWidth, 0.5f);
        categoryLine.LeanMoveX(menuPosition, 0.5f);
    }

    // 상세 화면
    public void DetailActive(int id)
    {
        float objectSize = 250;
        float objectPositionY = 0;
        int category = (int)GameManager.instance.itemInfoAll[id].category;

        if (category == 0) // Bed
        {
            objectPositionY = -70;
        }
        else if (category == 1) // Sofa
        {
        }
        else if (category == 2) // Table
        {
            objectPositionY = -120;
        }
        else if (category == 3) // Chair
        {
            objectSize = 500;
            objectPositionY = -320;
        }
        else if (category == 4) // Lamp
        {
        }
        else if (category == 5) // Plant
        {
        }

        // 카메라 변경
        ChangeCamera(false);

        // 정보 입력
        detailTitle.text = GameManager.instance.itemInfoAll[id].itemName;
        detailTag.text = GameManager.instance.itemInfoAll[id].itemTag;

        // 3D 모델링 배치
        if (detailObject != null)
        {
            Destroy(detailObject);
        }
        detailObject = Instantiate(GameManager.instance.itemInfoAll[id].itemPrefab, detailPool);
        detailObject.gameObject.layer = 5;
        if(detailObject.transform.childCount > 0)
        {
            for(int i=0;i<detailObject.transform.childCount; i++)
            {
                detailObject.transform.GetChild(i).gameObject.layer = 5;
            }
        }
        detailObject.transform.localPosition = new Vector3(0, objectPositionY, 0);
        detailObject.transform.localRotation = new Quaternion(0, 180, 0, 0);
        detailObject.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
    }

    // 카메라 변경
    public void ChangeCamera(bool ar)
    {
        arCamera.enabled = ar;
        derailCamera.enabled = !ar;

    }

    // 가구 배치 화면
    public void EditActive(bool select)
    {
        blur.SetActive(false);
        intro.SetActive(false);
        basicButton.SetActive(!select);
        editButton.SetActive(select);
        ArPlaneActive(select);
        PlaneActive(select);
    }

    // 화면 공유 

    public void GameShare()
    {
        basicButton.SetActive(false);
        ArPlaneActive(false);
        StartCoroutine(TakeScreenShotAndShare());
    }

    // 회전 오브젝트 생성
    void ARPlaceObject()
    {
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation);
    }

    // AR plane 비활성화
    public void PlaneActive(bool active)
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(active);
        }
        foreach (var plane in arPointCloudManager.trackables)
        {
            plane.gameObject.SetActive(active);
        }
    }

    // 화면을 캡처하고 공유하는 기능
    private IEnumerator TakeScreenShotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        string fileRath = System.IO.Path.Combine(Application.temporaryCachePath, "share.png");
        System.IO.File.WriteAllBytes(fileRath, texture.EncodeToPNG());

        Destroy(texture);

        new NativeShare().AddFile(fileRath).SetSubject("").SetText("").SetUrl("").SetCallback((res, target) => Debug.Log($"result {res}, target app: {target}")).Share();
        
        yield return null;

        basicButton.SetActive(true);
        ArPlaneActive(true);
    }
}
