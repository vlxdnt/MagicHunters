using UnityEngine;
using Unity.Netcode;

public class AbilityPickup : NetworkBehaviour
{
    public enum TipAbilitate { Heal, Invizibilitate, DoubleJump }
    
    [Header("Setari")]
    public TipAbilitate abilitate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Doar Serverul are voie sa aprobe coliziunea si sa distruga obiectul
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            WitchAbilities witch = other.GetComponent<WitchAbilities>();
            PlayerInput input = other.GetComponent<PlayerInput>();

            bool succes = false;

            if (abilitate == TipAbilitate.DoubleJump)
            {
                // Daca e pisica, APELAM CLIENT RPC ca sa se schimbe pe toate ecranele
                if (witch == null && input != null)
                {
                    input.DeblocheazaDoubleJumpClientRpc();
                    succes = true;
                }
            }
            else
            {
                // Daca e vrajitoare, APELAM CLIENT RPC
                if (witch != null)
                {
                    witch.DeblocheazaAbilitateClientRpc(abilitate.ToString());
                    succes = true;
                }
            }

            // Daca a fost luat cu succes, il scoatem de pe harta
            if (succes)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}