using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerManager : MonoBehaviour
{
    public static BannerManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<BannerManager>();
            }
            return m_instance;
        }
    }
    private static BannerManager m_instance;

    public GameObject Bnr;

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void BnrEnable()
    {
        int cnt = Bnr.transform.childCount;
        if(cnt < 0)
        {
            Destroy(Bnr);
        }
        else
        {
            int num = Random.RandomRange(0, cnt);
            for(int i = 0; i < cnt; i++)
            {
                Bnr.transform.GetChild(i).gameObject.SetActive(i == num ? true : false);
            }
        }
    }
}
