using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    private CarController m_carController;
    [SerializeField] private Image m_nitroFull;


    [SerializeField] private Image m_One;
    [SerializeField] private Image m_Two;
    [SerializeField] private Image m_Three;
    [SerializeField] private Image m_Go;

    private float m_downSizeWidthOne = 300f;
    private float m_downSizeWidthTwo = 300f;
    private float m_downSizeWidthThree = 450f;
    private float m_downSizeWidthGo = 455f;
    private float m_downSizeHeightOne = 300f;
    private float m_downSizeHeightTwo = 300f;
    private float m_downSizeHeightThree = 450f;
    private float m_downSizeHeightGo = 455;

    private float downSizeSpeed;

    private bool isRaceStarted = false;
    private float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_carController = player.GetComponent<CarController>();

        deltaTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += Time.deltaTime;

        if (!isRaceStarted)
        {
            StartRace();
        }

        m_nitroFull.fillAmount = m_carController.nitroAmount;
    }

    void StartRace()
    {
        if (deltaTime <= 2.0f) 
        {
            downSizeSpeed = 450 / 2f;
            //Just HardCoding this function cause dont have enough time to do it clean soorrrryyy
            m_Three.gameObject.SetActive(true);

            m_downSizeWidthThree -= downSizeSpeed * Time.deltaTime;
            m_downSizeHeightThree -= downSizeSpeed * Time.deltaTime;

            m_downSizeWidthThree = Mathf.Clamp(m_downSizeWidthThree, 1, m_downSizeWidthThree);
            m_downSizeHeightThree = Mathf.Clamp(m_downSizeHeightThree, 1, m_downSizeHeightThree);

            m_Three.rectTransform.sizeDelta = new Vector2(m_downSizeWidthThree, m_downSizeHeightThree);
        }else if (deltaTime <= 4.0f)
        {
            downSizeSpeed = 300 / 2f;

            m_Three.gameObject.SetActive(false);
            m_Two.gameObject.SetActive(true);

            m_downSizeWidthTwo -= downSizeSpeed * Time.deltaTime;
            m_downSizeHeightTwo -= downSizeSpeed * Time.deltaTime;

            m_downSizeWidthTwo = Mathf.Clamp(m_downSizeWidthTwo, 1, m_downSizeWidthTwo);
            m_downSizeHeightTwo = Mathf.Clamp(m_downSizeHeightTwo, 1, m_downSizeHeightTwo);

            m_Two.rectTransform.sizeDelta = new Vector2(m_downSizeWidthTwo, m_downSizeHeightTwo);
        }else if (deltaTime <= 6.0f)
        {
            m_Three.gameObject.SetActive(false);
            m_Two.gameObject.SetActive(false);
            m_One.gameObject.SetActive(true);

            m_downSizeWidthOne -= downSizeSpeed * Time.deltaTime;
            m_downSizeHeightOne -= downSizeSpeed * Time.deltaTime;

            m_downSizeWidthOne = Mathf.Clamp(m_downSizeWidthOne, 1, m_downSizeWidthOne);
            m_downSizeHeightOne = Mathf.Clamp(m_downSizeHeightOne, 1, m_downSizeHeightOne);

            m_One.rectTransform.sizeDelta = new Vector2(m_downSizeWidthOne, m_downSizeHeightOne);
        }else if(deltaTime <= 8.0f)
        {
            m_carController.gamedStarted = true;
            m_One.gameObject.SetActive(false);
            m_Go.gameObject.SetActive(true);

            m_downSizeWidthGo -= downSizeSpeed * Time.deltaTime;
            m_downSizeHeightGo -= downSizeSpeed * Time.deltaTime;

            m_downSizeWidthGo = Mathf.Clamp(m_downSizeWidthGo, 1, m_downSizeWidthGo);
            m_downSizeHeightGo = Mathf.Clamp(m_downSizeHeightGo, 1, m_downSizeHeightGo);

            m_Go.CrossFadeAlpha(0, 0.50f, false);
        }
        else
        {
            m_Go.gameObject.SetActive(false);
            isRaceStarted = true;
        }
    }
}
