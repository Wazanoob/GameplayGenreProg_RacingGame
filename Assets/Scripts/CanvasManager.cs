using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    private CarController m_carController;
    [SerializeField] private Image m_nitroFull;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_carController = player.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        m_nitroFull.fillAmount = m_carController.nitroAmount;
    }
}
