// using UnityEngine;
// using Unity.Netcode;

// public class TargetHitConnector : NetworkBehaviour
// {
//     [ServerRpc(RequireOwnership = false)] 
//     public void LovesteTargetServerRpc(string numeTarget)
//     {
//         LovesteTargetClientRpc(numeTarget);
//     }

//     [ClientRpc]
//     void LovesteTargetClientRpc(string numeTarget)
//     {
//         GameObject target = GameObject.Find(numeTarget);
//         if (target != null)
//         {
//             TargetDoor td = target.GetComponent<TargetDoor>();
//             if (td != null) td.DeschideUsa();
//         }
//     }
// }