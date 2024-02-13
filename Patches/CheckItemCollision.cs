using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;
namespace MaskedAIRevamp.Patches
{
    public class CheckItemCollision : MonoBehaviour
    {
        //public bool isCollisionWithCloset;
        //private GameObject closetObject;
        public bool hidedByMasked;
        //void Update()
        //{
            //closetObject = GameObject.Find("PlacementBounds");
            //Collider closetCollider = closetObject.GetComponent<Collider>();
            //if (closetObject.name == "PlacementBounds" && CheckCollision(closetCollider, this.GetComponent<Collider>()))
            //{
            //    isCollisionWithCloset = true;
            //}else
            //{
            //    isCollisionWithCloset = false;
            //}
        //}

        //private bool CheckCollision(Collider closetCollider, Collider collider2)
        //{

        //    return Physics.CheckBox(closetCollider.bounds.center, closetCollider.bounds.extents, Quaternion.identity, LayerMask.GetMask("Triggers")) &&
        //           Physics.CheckBox(collider2.bounds.center, collider2.bounds.extents, Quaternion.identity, LayerMask.GetMask("Props"));
        //}
    }
}