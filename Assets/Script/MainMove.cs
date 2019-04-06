using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMove : MonoBehaviour {

    GameObject Stick,Player,Eyes,Ground,BGL;
    ScoreUI scoreUI;
    
    //
    public Texture[] PlayerAnimation = new Texture[6];
    //
    public int Velocity = 3;
    public int state = 0;
    float angle = 0;
    Vector3 Pos1;
    //
    bool StickRotateSwitch = false;
    //棍子触地标志位
    bool GroundSwitch = false;
    bool PlayDeadMusic=false;
    bool HitRed = false;
    //
    int StickLock = 0,PicItem=0;
    Rigidbody PlayerRigidbody;

    Vector3 OriPos;
    Vector3 StickTopPoint;
    public GameObject prefab_Ground;
    GameObject ColliderGround;

    MusicControll MusicManager;
	void Start () {
        Stick = GameObject.Find("StickParent").gameObject;
        Player = GameObject.Find("Player").gameObject;
        Eyes = GameObject.Find("Eyes").gameObject;
        Ground= GameObject.Find("FirstGround").gameObject;
        BGL = GameObject.Find("BGL").gameObject;
        PlayerRigidbody = Player.GetComponent<Rigidbody>();
        scoreUI = gameObject.GetComponent<ScoreUI>();
        MusicManager = Eyes.GetComponent<MusicControll>();
    }
	void Update () {
        if(state==0)
        {
            //          
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                StickLock ++;
            }
            if (Input.GetKey(KeyCode.Mouse0)&&StickLock==1)
            {
                Stick.transform.localScale += new Vector3(0, Velocity * Time.deltaTime, 0);               
            }
            //
            if (Input.GetKeyUp(KeyCode.Mouse0)&&StickLock==1)
            {
                Invoke("StickWait", 0.4f);
            }

            if(StickRotateSwitch==true)
            {
                Stick.transform.Rotate(0, 0, -4.5f);
                angle += 4.5f;
                if (angle == 90)
                {
                    Invoke("CheckStickHitGround", 0.4f);
                    //如果已经旋转90度，则停止旋转
                    StickRotateSwitch = false;                   
                    if(GroundSwitch==false)
                    {
                        MusicManager.PlayStickFail();
                    }
                }

            }
        }
        if (state == 1)
        {
            PlayAnimation();
            //Move
            Player.transform.position = Vector3.MoveTowards(Player.transform.position, Pos1 + new Vector3(0, 1.167f, 0), 2*Time.deltaTime);
            if (Player.transform.position == Pos1+new Vector3(0, 1.167f, 0))
            {
                MusicManager.PlayAddScore();
                MoveStick();
            }
        }
        if (state == -1)
        {
            //float StickLong = Stick.transform.localScale.y + GroundX/2;
            PlayAnimation();
            //Stop moving
            //Player.transform.position = Vector3.MoveTowards(Player.transform.position, OriPos + new Vector3(StickLong, 0, 0), 2 * Time.deltaTime);
            //float DesX = OriPos.x + StickLong;
            Player.transform.position = Vector3.MoveTowards(Player.transform.position,StickTopPoint+new Vector3(0.1f, 0.2f, 0), 2 * Time.deltaTime);
            if (Player.transform.position.x >= StickTopPoint.x)
            {
                PlayerRigidbody.useGravity = true;
                Eyes.transform.parent = null;
                //BGL.transform.parent = null;
            }
           
            if(Player.transform.position.y < -2)
            {
                PlayerDead();
            }
        }
	}
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")&&HitRed ==false)
        {
            if (scoreUI.getScore()>0)
            {
                Destroy(ColliderGround.transform.parent.gameObject, 2);
            }
            if (scoreUI.getScore() == 0)
            {
                Destroy(Ground.gameObject,2);
            }
            Pos1 = other.transform.position;
            ColliderGround = other.gameObject;
            CreateGround();
            GroundSwitch = true;
            transform.GetComponent<ScoreUI>().AddScore();
            MusicManager.PlayStickHitPlatform();
        }
        else if(other.gameObject.CompareTag("Ground") && HitRed == true)
        {
            if (scoreUI.getScore() > 1)
            {
                Destroy(ColliderGround.transform.parent.gameObject, 2);
            }
            if (scoreUI.getScore() == 0)
            {
                Destroy(Ground.gameObject,2);
            }
            Pos1 = other.transform.position;
            ColliderGround = other.gameObject;
            CreateGround();
            GroundSwitch = true;
            transform.GetComponent<ScoreUI>().AddScore();
        }
        if (other.gameObject.CompareTag("Red"))
        {
            MusicManager.PlayPrefect();
            transform.GetComponent<ScoreUI>().AddScore();
            HitRed = true;
        }
       
    }
    void CheckStickHitGround()
    {

        if (GroundSwitch)
        {
            state = 1;
        }
        else
        {
            //OriPos = Player.transform.position;
            //float StickLong = Stick.transform.localScale.y + GroundX / 2;
            //print("OriPos" + OriPos);
            //print("StickLong" + StickLong);
            StickTopPoint = Stick.transform.Find("Stick").Find("TopPoint").transform.position;
            state = -1;
        }
    }
    void StickWait()
    {
        StickRotateSwitch = true;
    }

    void MoveStick()
    {
        Stick.transform.position = ColliderGround.transform.Find("StickPos").transform.position;
        Stick.transform.rotation = Quaternion.identity;
        Stick.transform.localScale = new Vector3(0.05f, 0.01f, 0.05f);
        state = 0;
        angle = 0;
        GroundSwitch = false;
        StickLock = 0;
        HitRed = false;
    }

    void CreateGround()
    {       
        Ground = Instantiate(prefab_Ground);
        Ground.transform.position = ColliderGround.transform.position + new Vector3(Random.Range(1, 2.5f), 0, 0);
        Ground.transform.Find("Ground").transform.localScale = new Vector3(Random.Range(0.2f, 1), 1, 1);
    }
  
    void PlayAnimation()
    {
        Player.transform.GetComponent<Renderer>().material.mainTexture = PlayerAnimation[PicItem];
        PicItem++;
       if(PicItem==5)
       {
            PicItem = 0;
       }
    }

    void PlayerDead()
    {
        if (PlayDeadMusic == false)
        {
            MusicManager.PlayPlayerFall();
            PlayDeadMusic = true;
        }
        transform.GetComponent<ScoreUI>().CalculateScore();
    }

}
