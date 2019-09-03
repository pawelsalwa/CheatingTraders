using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotController : MonoBehaviour
{
    public MovementComponent movement;
    public AttackComponent attack;
    public CharacterRotationComponent rotatation;

    private Transform player { get { return FindObjectsOfType<BasicUnit>().FirstOrDefault(x => x.GetComponent<UserInputHandler>().enabled).transform; }}

    private void Update()
    {

        Vector3 thisPos = transform.position + Vector3.up;
        Vector3 targetPos = player.position + Vector3.up;
        Vector3 targetDir = targetPos - thisPos;
        
        Debug.DrawRay(thisPos, targetDir, Color.white, Time.deltaTime, true);
        
        RaycastHit asd;
//        Ray asd2 = new Ray(transform.position + Vector3.up, player.position - transform.position + Vector3.up);
//        Physics.Raycast( asd2, out asd, 100);
        
        bool asd3 = Physics.Raycast(thisPos, targetDir, out asd);
        
        Debug.Log(asd3);

//        if (asd.transform?.GetComponent<BasicUnit>() != null) {
        if(asd.transform.tag == "kurwamac") {
            Debug.Log(asd.transform.name);
            rotatation.LookAt(  player.position - transform.position);
        }
        
    }


    public enum BotState
    {
    }
}