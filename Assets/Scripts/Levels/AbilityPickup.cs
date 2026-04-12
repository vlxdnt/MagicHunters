using UnityEngine;
using Unity.Netcode;

public class AbilityPickup : NetworkBehaviour
{
    public enum TipAbilitate { Heal, Invizibilitate, DoubleJump }
    
    [Header("Setari")]
    public TipAbilitate abilitate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            WitchAbilities witch = other.GetComponent<WitchAbilities>();
            PlayerInput input = other.GetComponent<PlayerInput>();

            bool succes = false;

            if (abilitate == TipAbilitate.DoubleJump)
            {
                if (witch == null && input != null)
                {
                    input.DeblocheazaDoubleJumpClientRpc();
                    succes = true;
                }
            }
            else
            {
                if (witch != null)
                {
                    witch.DeblocheazaAbilitateClientRpc(abilitate.ToString());
                    succes = true;
                }
            }

            if (succes)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}