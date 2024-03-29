using TrolleyTest;
using UnityEngine;
using Hazel;
using UnityObject = UnityEngine.Object;
using System;
using TMPro;
using System.Collections;

public class BomberRole : RoleBehaviour
{
    public override string roleDisplayName => "Bomber";
    public override string roleDescription => "You can explode up to 2 bombs";
    public PlaceButtonManager placeButton;
    public AudioClip explosion;

    public enum RpcCalls
    {
        Explode = 0,
        CheckExplode = 1,
    }

    public override void ConfigureRole()
    {
        //The team of the role
        RoleTeamType = RoleTeamTypes.Impostor;

        //The teams that the role can kill
        enemyTeams = new RoleTeamTypes[] { RoleTeamTypes.Crewmate, RoleTeamTypes.Neutral };

        //If the role can use kill button in the moment
        CanUseKillButton = true;

        //If the role can vent
        CanVent = false;

        //Add optional sounds (WARNING: Only wav files are supported for now)
        //If your audio is too quick or too slow, try changing the second argument between the numbers:
        //2, 4, 6, 8
        explosion = Utils.LoadAudioClipFromResources("TrolleyTest.Resources.cola-explosion.wav", 2);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (Player != null && Player.AmOwner)
        {
            if (placeButton is null && HudManager.InstanceExists)
            {
                HudManager hud = HudManager.Instance;
                Transform parent = hud.transform.Find("Buttons/BottomRight").transform;
                placeButton = CreatePlaceButton(parent);
            }
        }
    }

    public PlaceButtonManager CreatePlaceButton(Transform parent)
    {
        PlaceButtonManager placeButtonM = UnityObject.FindAnyObjectByType<PlaceButtonManager>();
        if (placeButtonM != null)
        {
            return placeButtonM;
        }
        GameObject obj = UnityObject.Instantiate(DestroyableSingleton<CachedMaterials>.Instance.abilityButton, parent).gameObject;
        AbilityButtonManager component = obj.GetComponent<AbilityButtonManager>();
        TextMeshPro abilityText = component.AbilityText;
        TextMeshPro cooldownText = component.CooldownText;
        SpriteRenderer spriteRender = component.spriteRender;
        UnityObject.Destroy(component);
        PlaceButtonManager placeButtonManager1 = obj.AddComponent<PlaceButtonManager>();
        placeButtonManager1.spriteRender = spriteRender;
        placeButtonManager1.CooldownText = cooldownText;
        placeButtonManager1.AbilityText = abilityText;
        placeButtonManager1.Refresh();
        placeButtonManager1.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
        placeButtonManager1.GetComponent<PassiveButton>().OnClick.AddListener(placeButtonManager1.DoClick);
        return placeButtonManager1;
    }

    public void CmdCheckExplode()
    {
        if (AmongUsClient.Instance.AmHost)
        {
            CheckExplode();
        }
        MessageWriter writer = Player.StartRoleRpc(1);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public void RpcExplode()
    {
        if (AmongUsClient.Instance.AmHost)
        {
            StartCoroutine(Explode());
        }
        MessageWriter writer = Player.StartRoleRpc(0); //Host starts the rpc, pretending to be the player
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public void CheckExplode() //This entire method should only be run by host.
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (Player.Data.IsDead) return;
        if (DateTime.UtcNow.Subtract(Player.Data.LastMurder.UtcDateTime).TotalSeconds < Player.Data.myRole.KillCooldown - 0.5f)
        {
            return;
        }
        RpcExplode();
    }

    public IEnumerator Explode() //Some parts of this code should only be run by host. (e.g murderplayer)
    {
        Vector2 explosionSource = Player.GetTruePosition();
        SoundManager.Instance.PlaySound(explosion, false);
        yield return new WaitForSeconds(0.23f);
        yield return HudManager.Instance.CoFadeFullScreen(Palette.DisabledColor, new Color(255, 0, 0, 0.4f), 0.3f);

        yield return new WaitForSeconds(2f);
        yield return HudManager.Instance.CoFadeFullScreen(Palette.DisabledColor, new Color(0, 0, 0, 0), 0.1f);

        if (AmongUsClient.Instance.AmHost)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionSource, 3f, Constants.PlayersOnlyMask);

            foreach (Collider2D collider in colliders)
            {
                PlayerControl player = collider.GetComponent<PlayerControl>();
                if (player != null && !player.Data.IsDead && player != Player)
                {
                    player.RpcMurderPlayer(player, MurderResultFlags.Succeeded);
                }
            }
        }

        yield break;
    }

    public override void HandleRpc(MessageReader reader, int rpc)
    {
        switch ((RpcCalls)rpc)
        {
            case RpcCalls.CheckExplode:
            CheckExplode();
            break;
            case RpcCalls.Explode:
            StartCoroutine(Explode());
            break;
        }
    }
}