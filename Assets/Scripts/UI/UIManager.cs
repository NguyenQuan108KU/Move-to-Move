using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public TextMeshProUGUI alive;
    public TextMeshProUGUI pointOfPlayer;
    public TextMeshProUGUI namePlayer;
    public int enemyAliveTotal;

    [Header("Load")]
    [SerializeField] public GameObject loadCircle;
    [SerializeField] public float speedRotation;
    [SerializeField] TextMeshProUGUI number;
    private int countNumber;

    [Header("Dead2")]
    [SerializeField] private GameObject Canvas_Dead_1;
    [SerializeField] private GameObject Canvas_Dead_2;
    [SerializeField] public bool isDead = false;

    private void Start()
    {
        enemyAliveTotal = 30;
        countNumber = 5;
    }
    private void Update()
    {
        alive.text = enemyAliveTotal.ToString();
        setVitriScorePlayer();
        pointOfPlayer.text = GameManager.instance.playerController.point.ToString();
        if (isDead)
        {
            Load();
        }
        //Load();
    }
    private void LateUpdate()
    {
        setVitriScorePlayer();
    }
    public void UpdateAlive()
    {
        enemyAliveTotal -= 1;
    }
    void setVitriScorePlayer()
    {
        Vector3 enemyScreenPosition = Camera.main.WorldToScreenPoint(GameManager.instance.transform.position + new Vector3(0, 4, 0));
        namePlayer.transform.position = enemyScreenPosition;
    }

    //Load khi player chết
    public void Load()
    {
        loadCircle.transform.rotation = Quaternion.Euler(0, 0, Time.time * -speedRotation);
        int countdownStartTime = Mathf.RoundToInt(Time.time);
        int countdownDuration = 5;
        int count = countdownDuration - countdownStartTime;
        number.text = (countdownDuration - countdownStartTime).ToString();
        Debug.Log(count);
        if(count <= 0)
        {
            Canvas_Dead_1.SetActive(false);
            Canvas_Dead_2.SetActive(true);
        }
    }
}
