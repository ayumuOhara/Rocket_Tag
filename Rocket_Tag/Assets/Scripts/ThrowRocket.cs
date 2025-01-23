using Photon.Realtime;
using UnityEngine;

public class ThrowRocket : MonoBehaviour
{
    float throwSpeed = 1;
    float throwedTime = 10f;
    float retrieveTime = 1.5f;
    bool isThrowed = false;
    bool isReturning = false;
    bool isHoldRocket = true;

    GameObject player;
    GameObject rocket;
    Collider[] hitCollider;
    CapsuleCollider capsuleCollider;
    Rocket rocketCS;
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isThrowed = true;
        }
        if(isThrowed && !isReturning)
        {
            ThrowToPos();
        }
        if (isThrowed)
        {
            throwedTime += Time.deltaTime;
        }
        if (WhatIsHit(hitCollider) == "Player")
        {
            
        }
    }
    void Initialize()
    {
        rocket = GameObject.Find("Rocket");
        player = GameObject.Find("Player");
        hitCollider = new Collider[100];
        capsuleCollider = rocket.GetComponent<CapsuleCollider>();
        rocketCS = GetComponent<Rocket>();
    }
    void ThrowToPos()
    {
        isThrowed = true;
        isReturning = false;
        isHoldRocket = false;
        StraightMoveToPos(rocket.transform, rocket.transform.position, GetLineDir(GetScreenCenterPos(), rocket.transform.position), throwSpeed);
        hitCollider = Physics.OverlapCapsule(rocket.transform.position - Vector3.down * capsuleCollider.height * 0.5f,
                                       rocket.transform.position + Vector3.up * capsuleCollider.height * 0.5f,
                                       capsuleCollider.radius);
    }
    public Vector3 GetLineDir(Vector3 target, Vector3 current)
    {
        return target - current;
    }
    Vector3 GetScreenCenterPos()    //  カメラのワールドでの中心座標を求める
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
        //Vector3 direction = (worldCenter - transform.position).normalized;
        return Camera.main.ScreenToWorldPoint(screenCenter);
    }
    bool IsNeedRetrieve()
    {
        return throwedTime > retrieveTime;
    }
    public void RetriveByStraightLine()
    {
        isReturning = true;
        GetLineDir(rocket.transform.position, player.transform.position);
    }
    void ApproachPos(GameObject axis, GameObject approcher, Vector3 offset)    //  axisを中心にApprocherのPosをoffset分加えて変更する
    {
        approcher.transform.position = axis.transform.position + offset;
    }
    string WhatIsHit(Collider[] hits)
    {
        return hits[0].tag;
    }
    void StraightMoveToPos(Transform moved, Vector3 current, Vector3 target, float moveSpeed)    //  座標に向かって直線移動
    {
         moved.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);
    }
}
