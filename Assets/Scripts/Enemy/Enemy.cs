using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float EnemySpeed;
    private Animator anim;

    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private Transform firingTransform;
    public Transform target;

    [Header("Detection & Attack Range")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 3f;

    [Header("Target Tags")]
    [SerializeField] private List<string> targetTags = new List<string> { "Player", "Enemy" };

    public SkinnedMeshRenderer[] render;

    public GameObject BloodParticle;
    public bool isDead = false;
    public GameObject targetEnemy;

    public float timeStartBullet;

    private Vector3 randomDirection;
    private float changeDirectionTime = 3f;
    private float timer = 0f;
    public TextMeshProUGUI textEnemy;

    public TextMeshProUGUI nameEnemy;
    public List<NameData> listName = new List<NameData>();
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        nameEnemy.text = listName[Random.Range(0, listName.Count)].name.ToString();
        
        foreach (var item in render)
        {
            item.material.color = Random.ColorHSV();
        }
    }

    private void Update()
    {
        EnemyMovement();
        EnemyAttack();
        setVitriScoreEnemy();
    }

    public void EnemyMovement()
    {

        if (isDead || target != null) return; // Không di chuyển nếu đã chết hoặc đang tấn công

        timer += Time.deltaTime;
        if (timer >= changeDirectionTime)
        {
            anim.SetBool("Move", true);
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            timer = 0f;
        }

        Vector3 move = randomDirection * EnemySpeed * Time.deltaTime;
        rb.MovePosition(transform.position + move);

        // Quay mặt theo hướng di chuyển
        if (randomDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(randomDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 5f * Time.deltaTime);
        }
    }

    public void EnemyAttack()
    {

        if (isDead) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        bool foundTarget = false;

        foreach (var hit in colliders)
        {
            if (hit.transform == gameObject.transform) continue;
            if (hit.gameObject.CompareTag("Player") || hit.gameObject.CompareTag("Enemy"))
            {
                target = hit.transform;
                anim.SetBool("Attack", true);
                // Quay mặt về hướng Player
                Vector3 directionEnemy = hit.transform.position - transform.position;
                directionEnemy.y = 0;
                Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);

                foundTarget = true;
                break; // Dừng kiểm tra nếu đã tìm thấy Player
            }
        }

        if (!foundTarget)
        {
            // Player đã rời khỏi phạm vi → quay lại trạng thái di chuyển ngẫu nhiên
            target = null;
            anim.SetBool("Attack", false);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet1") || collision.gameObject.CompareTag("Bullet2"))
        {
            Bullet bulletScript = collision.gameObject.GetComponent<Bullet>();
            if (bulletScript == null) return;
            if (bulletScript.owner == this.gameObject)
            {
                return;
            }
            GameManager.instance.playerController.point += 5;
            UIManager.instance.UpdateAlive();
            isDead = true;
            //Praticle System
            BloodParticle.SetActive(true);
            anim.SetBool("Death", true);
            gameObject.tag = "Untagged";
        }
    }


    public void Shooting()
    {
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        bulletObj.tag = "Bullet2"; // tag cho bullet của enemy
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetOwner(gameObject);
        bulletScript.SetTarget(target);
    }
    void setVitriScoreEnemy()
    {
        Vector3 enemyScreenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 4, 0));
        textEnemy.transform.position = enemyScreenPosition;
    }
    public void RestartTimerBullet() => timeStartBullet = 0.8f;
    public void DestroyEnemy() => Destroy(gameObject);
}