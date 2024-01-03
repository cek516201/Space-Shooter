using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class MonsterCtrl : MonoBehaviour
{
    public enum MonsterState {idle, trace, attack, die};
    public MonsterState monsterState = MonsterState.idle;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    private bool isDie = false;

    private int hp = 100;

    private GameUI gameUI;

    public GameObject bloodEffect;
    public GameObject bloodDecal;

    void Start()
    {
        monsterTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();

        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MonsterAction());
    }

    void Update()
    {
        
    }

    IEnumerator CheckMonsterState()
    {
        while(isDie == false) // 살아있으면
        {
            yield return new WaitForSeconds(0.2f); // 0.2초 기다렸다가 코드진행

            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if(dist <= attackDist) // 공격범위 안쪽이면
            {
                monsterState = MonsterState.attack;
            }
            else if(dist <= traceDist) // 공격범위 바깥쪽, 추적범위 안쪽이면
            {
                monsterState = MonsterState.trace;
            }
            else
            {
                monsterState = MonsterState.idle;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (isDie == false) // 살아있으면
        {
            switch(monsterState)
            {
                case MonsterState.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);
                    break;

                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                    break;

                case MonsterState.attack:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsAttack", true);
                    break;
            }

            yield return null; // 1프레임 기다렸다가 코드진행
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            Destroy(coll.gameObject);
            
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;

            createBloodEffect(coll.transform.position);

            if(hp <= 0)
            {
                MonsterDie();
            }
            else
            {
                animator.SetTrigger("IsHit");
            }
        }
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        nvAgent.isStopped = true;
        animator.SetTrigger("IsPlayerDie");
    }

    void MonsterDie()
    {
        StopAllCoroutines();
        isDie = true;

        monsterState = MonsterState.die;
        nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

        foreach (Collider coll in this.gameObject.GetComponentsInChildren<SphereCollider>()) // 양손 콜라이더 비활성화
        {
            coll.enabled = false;
        }

        gameUI.DispScore(50);
    }

    void createBloodEffect(Vector3 pos)
    {
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 2.0f);

        Vector3 decalPos = monsterTr.position + Vector3.up * 0.05f;
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;
        Destroy(blood2, 5.0f);
    }    
}
