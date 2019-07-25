using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public float EyeViewDistance; //視野距離
    public float ViewAngle = 120f; //視野角度
    private Collider[] SpottedPlayer; //附近的玩家

    EnemyAI AI;
    public Transform PlayerLastPosition;


    void Start()
    {
        AI = GetComponentInParent<EnemyAI>();

    }
    private void FixedUpdate()
    {
        DetectPlayer();
    }

    void DetectPlayer()  //探測玩家
    {
        //OverlapSphere内的敵人
        SpottedPlayer = Physics.OverlapSphere(transform.position, EyeViewDistance, LayerMask.GetMask("Player"));
        for (int i = 0; i < SpottedPlayer.Length; i++) //檢測玩家(Layer)是否在視野區中
        {
            Vector3 PlayerPosition = SpottedPlayer[i].transform.position; //玩家的位置

            //Debug.Log(transform.forward + " 面對的方向");
            //Debug.Log("夾角為:" + Vector3.Angle(transform.forward, EnemyPosition - transform.position));

            Debug.DrawRay(transform.position, PlayerPosition - transform.position, Color.yellow); //敵人位置到玩家位置的向量
            if (Vector3.Angle(transform.forward, PlayerPosition - transform.position) <= ViewAngle / 2)  //這個玩家是否在視野內
            {
                //如果在視野內
                RaycastHit info = new RaycastHit();
                int layermask = LayerMask.GetMask("Player", "Obstacles"); //指定射線碰撞的對象為 玩家和障礙物(牆壁物件)
                Physics.Raycast(transform.position, PlayerPosition - transform.position, out info, EyeViewDistance, layermask); //向玩家位置發射射線
                //Raycast(自己的位置,方向,打到物體,距離,圖層)
                //Debug.Log(info.collider.gameObject.name);//log撞到的collider名稱
                if (info.collider == SpottedPlayer[i]) //如果途中無其他障礙物，那麼射線就會碰撞到玩家
                {
                    DiscoveredPlayer(SpottedPlayer[i]);
                    PlayerLastPosition.position  = PlayerPosition;
                    print("看到了");
                }
                else
                {
                    //玩家離開視線範圍 放棄追逐
                    AI.Player = null;
                    print("跨謀");
                }
            }
        }
    }

    //DiscoveredPlayer和GiveUpChasing可以統整為一個function 一個進入一個離開

    void DiscoveredPlayer(Collider Player) //發現玩家
    {
        if (AI.Player == null)
        {
            AI.Player = Player.transform;
            AI.EnemyStatus = EnemyAI.Enemy.Alert;
        }
    }
}
