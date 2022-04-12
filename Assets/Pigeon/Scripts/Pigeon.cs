using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pigeon : MonoBehaviour
{
    [Header("---- Origin Used ----")]
    private Animator pigeon;
    public float gravity = 1.0f;
    // public Vector3 moveDirection = Vector3.zero;
    public Vector3 moveDirection;
    CharacterController characterController;
    [Header("---- Looping Used ----")]
    private float StartTime;
    private string Stage; // idle -> walk -> eat -> fly -> landing
    public float TimeScale = 2.0f;
    [Header("---- Path Control Used ----")]
    public int pigeonId;
    public short nPath;
    
	public Action[] path;
    public float moveSpeed;
    public float circleSpeed;
    public Transform mark;
    private Vector3 nextTarget;
    private string nextAction;
    private int actionIndex;
    private float switchActionMoment;
    private bool actionInit;
    private string status;
    private Vector3 target;
    public bool printStatus;
    public float flyHeight;
    public float flyHeightAdjust;
    public float SimulatonTimeScale;
    // flyto
    private Vector3 flytoStart;
    private float flytoStartTime;
    private float flytoInterval;
    private float flytoBegin;

    // arrive

    void Start()
    {
        // Fetch Component
        pigeon = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // Init pigeon Data
        moveDirection = new Vector3(0,0,1);
        characterController.enabled = false;
        
        // Init Looping Data
        Stage = "idle";

        // Init PathControl Data
        StartTime = Time.time;
        actionIndex = 0;
        target = new Vector3(MapToGameX(path[0].x), 0.0f, MapToGameY(path[0].y));
        switchActionMoment = path[0].second;
        actionInit = false;
        status = "start";
    }
    void Update()
    {
    }
    void FixedUpdate()
    {
        // moveDirection.y = gravity * Time.deltaTime;
        // Looping();
        // KeyControl();
        PathControl();
    }
    void Looping()
    {
        if (Stage == "idle" && Time.time - StartTime >= 1.5){
            pigeon.SetBool("walk", true);
            pigeon.SetBool("idle", false);
            Stage = "walkStart";

        } else if (Stage == "walkStart" && Time.time - StartTime >= 3 * TimeScale){
            pigeon.SetBool("idle", true);
            pigeon.SetBool("walk", false);
            Stage = "walkEnd";

        } else if (Stage == "walkEnd" && Time.time - StartTime >= 3.5 * TimeScale){
            pigeon.SetBool("idle", false);
            pigeon.SetBool("eat", true);
            Stage = "eatStart";

        } else if (Stage == "eatStart" && Time.time - StartTime >= 5 * TimeScale){
            pigeon.SetBool("idle", true);
            pigeon.SetBool("eat", false);
            Stage = "eatEnd";

        } else if (Stage == "eatEnd" && Time.time - StartTime >= 5.5 * TimeScale){
            pigeon.SetBool("takeoff", true);
            pigeon.SetBool("idle", false);
            Stage = "flyStart";

        } else if (Stage == "flyStart" && Time.time - StartTime >= 9 * TimeScale){
            pigeon.SetBool("takeoff", false);
            pigeon.SetBool("landing", true);
            Stage = "flyEnd";

        } else if (Stage == "flyEnd" && Time.time - StartTime >= 10 * TimeScale){
            pigeon.SetBool("idle", true);
            pigeon.SetBool("landing", false);
            Stage = "idle";
            StartTime = Time.time;

        }else{

        }
    }
    void KeyControl()
    {
        if (pigeon.GetCurrentAnimatorStateInfo(0).IsName("fly"))
        {
            pigeon.SetBool("takeoff", false);
            pigeon.SetBool("landing", false);
            pigeon.SetBool("glide", false);
        }
        if (pigeon.GetCurrentAnimatorStateInfo(0).IsName("flyleft"))
        {
            pigeon.SetBool("takeoff", false);
            pigeon.SetBool("landing", false);
            pigeon.SetBool("glide", false);
        }
        if (pigeon.GetCurrentAnimatorStateInfo(0).IsName("flyright"))
        {
            pigeon.SetBool("takeoff", false);
            pigeon.SetBool("landing", false);
            pigeon.SetBool("glide", false);
        }
        if (pigeon.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            pigeon.SetBool("eat", false);
            pigeon.SetBool("preen", false);
            pigeon.SetBool("scratching", false);
            pigeon.SetBool("landing", false);
            pigeon.SetBool("walk", false);
            pigeon.SetBool("walkleft", false);
            pigeon.SetBool("walkright", false);
            pigeon.SetBool("glide", false);
            pigeon.SetBool("takeoff", false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pigeon.SetBool("takeoff", true);
            pigeon.SetBool("landing", true);
            pigeon.SetBool("idle", false);
            pigeon.SetBool("fly", false);
            pigeon.SetBool("flyleft", false);
            pigeon.SetBool("flyright", false);
            pigeon.SetBool("glide", false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            pigeon.SetBool("walk", true);
            pigeon.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            pigeon.SetBool("walk", false);
            pigeon.SetBool("idle", true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            pigeon.SetBool("flyleft", true);
            pigeon.SetBool("fly", false);
            pigeon.SetBool("walkleft", true);
            pigeon.SetBool("walk", false);
            pigeon.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            pigeon.SetBool("fly", true);
            pigeon.SetBool("flyleft", false);
            pigeon.SetBool("walk", true);
            pigeon.SetBool("walkleft", false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            pigeon.SetBool("flyright", true);
            pigeon.SetBool("fly", false);
            pigeon.SetBool("walkright", true);
            pigeon.SetBool("walk", false);
            pigeon.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            pigeon.SetBool("fly", true);
            pigeon.SetBool("flyright", false);
            pigeon.SetBool("walk", true);
            pigeon.SetBool("walkright", false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            pigeon.SetBool("eat", true);
            pigeon.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            pigeon.SetBool("preen", true);
            pigeon.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            pigeon.SetBool("scratching", true);
            pigeon.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            pigeon.SetBool("glide", true);
            pigeon.SetBool("fly", false);
            pigeon.SetBool("flyleft", false);
            pigeon.SetBool("flyright", false);
        }
    }
    void PathControl()
    {
        // status action
        if (printStatus) Debug.Log("status: " + status);
        if (printStatus) Debug.Log("target: " + target);
        
        if (status == "start"){
            PigeonStart(target);
            if (Time.time - StartTime >= switchActionMoment){
                status = nextAction;
                target = nextTarget;
                actionInit = false;
            }
        } else if (status == "flyto"){
            PigeonFlyTo(target);
            if (Time.time - StartTime >= switchActionMoment){
                status = nextAction;
                target = nextTarget;
                actionInit = false;
            }
        } else if (status == "circle"){
            PigeonCircle(target);
            if (Time.time - StartTime >= switchActionMoment){
                status = nextAction;
                target = nextTarget;
                actionInit = false;
            }
        } else if (status == "arrive"){
            PigeonArrive();
            status = "success";
        } else if (status == "fail"){
            PigeonFail();
            status = "failure";
        } 
    }
    void loadNextAction()
    {
        actionIndex += 1;
        nextAction = path[actionIndex].action;
        nextTarget = new Vector3(MapToGameX(path[actionIndex].x), flyHeight, MapToGameY(path[actionIndex].y));
        switchActionMoment = path[actionIndex].second / SimulatonTimeScale;
    }
    void PigeonStart(Vector3 startingPoint)
    {
        // if (pigeon.GetCurrentAnimatoStateInfo(0).IsName("idle"))
        if (!actionInit)
        {
            // if (pigeonId == 1) Debug.Log("start from: " + startingPoint);
            pigeon.SetBool("takeoff", true);
            pigeon.SetBool("idle", false);
            
            transform.position = startingPoint;
            characterController.enabled = true;
            actionInit = true; 
            loadNextAction();
            transform.rotation = Quaternion.LookRotation(new Vector3(nextTarget.x,0,nextTarget.z) - transform.position, transform.up);
            moveDirection = transform.forward + new Vector3(0,10,0);
            // Debug.Log("nextTarget: " + nextTarget);
        }
        if (transform.position.y >= flyHeight && moveDirection.y != 0){
            transform.position = new Vector3(transform.position.x,flyHeight,transform.position.z);
            moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        } 
        characterController.Move(moveDirection * Time.deltaTime * moveSpeed);
    }
    void PigeonCircle(Vector3 target)
    {
        if (!actionInit)
        {
            // if (pigeonId == 1) Debug.Log("circle around: " + target);
            // Instantiate (mark, target , Quaternion.identity);
            

            characterController.enabled = false;
            transform.rotation = Quaternion.LookRotation(target - transform.position, transform.forward);
            transform.rotation = Quaternion.LookRotation(Vector3.up, transform.up);
            transform.Rotate(0, 90, 0, Space.Self);
            
            

            actionInit = true;
            loadNextAction();
        }
        // Debug.Log("circling");
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        if (eulerRotation.z != 0)
            transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
        transform.RotateAround(target, Vector3.up, Time.deltaTime * circleSpeed);
    }
    void PigeonFlyTo(Vector3 target)
    {
        if (!actionInit)
        {
            // if (pigeonId == 1) Debug.Log("fly to: " + target);
            // Instantiate (mark, target , Quaternion.identity);

            flytoStart = transform.position = new Vector3(transform.position.x,flyHeight,transform.position.z);
            flytoStartTime = switchActionMoment;
            flytoBegin = Time.time;

            transform.rotation = Quaternion.LookRotation(target - transform.position, transform.up);
            moveDirection = transform.forward;
            // characterController.enabled = true;
            actionInit = true;
            loadNextAction();
            flytoInterval = switchActionMoment - flytoStartTime;


        }
        // characterController.Move(moveDirection * Time.deltaTime * moveSpeed);
        Vector3 posi = Vector3.Lerp(flytoStart, target, (Time.time - flytoBegin) / flytoInterval);
        if (posi.y != flyHeight){
            return;
        }
        transform.position = posi;
        // Debug.Log("flytoStart: " + flytoStart + "  target : " + target + "  (Time.time - flytoBegin) / flytoInterval: " + (Time.time - flytoBegin) / flytoInterval);
    }
    void PigeonArrive()
    {
        if (!actionInit)
        {
            // if (pigeonId == 1) Debug.Log("arrive");
            characterController.enabled = true;

            pigeon.SetBool("landing", true);
            pigeon.SetBool("takeoff", false);
            pigeon.SetBool("fly", false);
            
            actionInit = true; 
        }
    }
    void PigeonFail()
    {
        if (!actionInit)
        {
            // if (pigeonId == 1) Debug.Log("fail");
            characterController.enabled = true;

            pigeon.SetBool("landing", true);
            pigeon.SetBool("takeoff", false);
            pigeon.SetBool("fly", false);
            

            actionInit = true; 
        }
    }
    float MapToGameX(float E)
    {
        return (E - 120.75f) * 200;
    }
    float MapToGameY(float N)
    {
        return (N - 24.0f) * 200;
    }
}    