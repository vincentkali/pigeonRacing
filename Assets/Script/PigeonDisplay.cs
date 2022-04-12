using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public long pigeonId;
    private Animator pigeon;
    private int currentStageIdx;
    private int StageLen;
    private string[] actionOrder;
    void Start()
    {
        
    }
    void OnEnable()
    {
        pigeonInit();
    }
    public void pigeonInit()
    {
        // Fetch Component
        pigeon = GetComponent<Animator>();

        // Init Looping Data
        actionOrder = new string[]{"eat","walk","scratching","walkleft","walkright"};
        StageLen = actionOrder.Length;

        // shuffle
        for (int i = 0; i < StageLen; i++) {
            int rnd = Random.Range(0, StageLen);
            string tempGO = actionOrder[rnd];
            actionOrder[rnd] = actionOrder[i];
            actionOrder[i] = tempGO;
        }
        currentStageIdx = 0;

        StartCoroutine(Looping());
    }
    IEnumerator Looping()
    {
        while (true){
            string action = actionOrder[currentStageIdx];
            // Debug.Log(action);
            pigeon.SetBool("idle", false);
            pigeon.SetBool(action, true);
            yield return new WaitForSeconds(2);
            if (action == "walkleft" || action == "walkright"){
                pigeon.SetBool(action, false);
                pigeon.SetBool("walk", true);
                yield return new WaitForSeconds(0.5f);
                pigeon.SetBool("walk", false);
                pigeon.SetBool("idle", true);
            }else{
                pigeon.SetBool("idle", true);
                pigeon.SetBool(action, false);
            }
            yield return new WaitForSeconds(0.5f);
            currentStageIdx = (currentStageIdx + 1) % StageLen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        
    }
}
