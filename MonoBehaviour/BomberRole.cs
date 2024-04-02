using TrolleyTest;
using UnityEngine;
using Hazel;
using UnityObject = UnityEngine.Object;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kino;

public class BomberRole : RoleBehaviour
{
    public override string roleDisplayName => "Bomber";
    public override string roleDescription => "You can explode up to 2 bombs";
    public PlaceButtonManager placeButton;
    public AudioClip explosion;
    public AudioClip earRing;
    public Sprite bomb;
    public bool bombEnabled = false;

    public int MaxUses = 2;
    public int uses = 0;
    public bool bombCoolingDown => explodeTimer > 0;
    public float explodeTimer;
    public float explodeCooldown = 45f;

    public DateTimeOffset LastExplosion;

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
        explosion = Utils.LoadAudioClipFromResources("RoleExample.Resources.cola-explosion.wav", 2);
        earRing = Utils.LoadAudioClipFromResources("RoleExample.Resources.ear-ringing.wav", 2);

        //load your sprite
        //change second argument to change image size
        bomb = Utils.LoadSprite("RoleExample.Resources.Dinamite.png", 300f);
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
            SetExplodeTimer(explodeTimer - Time.fixedDeltaTime);
            if (explodeTimer > 0f)
            {
            placeButton.CooldownText.text = Mathf.CeilToInt(explodeTimer).ToString();
            }
            else
            {
            placeButton.CooldownText.text = "";
            }
            placeButton.UseText.text = $"{MaxUses - uses}";
        }
    }

    public void SetExplodeTimer(float time)
    {
        GameData.PlayerInfo data = Player.Data;
        if (data.myRole != null && data.myRole.CanUseKillButton && Player.CanMove && !data.IsDead && !bombEnabled)
        {
            explodeTimer = time;
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
        placeButtonManager1.UseText = Instantiate(cooldownText);
        placeButtonManager1.UseText.transform.SetParent(placeButtonManager1.transform, false);
        placeButtonManager1.Refresh();
        placeButtonManager1.CooldownText.gameObject.SetActive(true);
        placeButtonManager1.UseText.gameObject.SetActive(true);
        placeButtonManager1.UseText.color = Palette.LightBlue;
        placeButtonManager1.UseText.transform.SetX(placeButtonManager1.UseText.transform.localPosition.x - 0.4f);
        placeButtonManager1.CooldownText.transform.SetY(placeButtonManager1.CooldownText.transform.localPosition.y - 0.2f);
        placeButtonManager1.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
        placeButtonManager1.GetComponent<PassiveButton>().OnClick.AddListener(placeButtonManager1.DoClick);
        return placeButtonManager1;
    }

    public void CmdCheckExplode()
    {
        if (bombCoolingDown) return;
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
        if (!Player.CanMove) return;
        if (bombEnabled) return;
        if (uses >= MaxUses) return;
        if (DateTime.UtcNow.Subtract(LastExplosion.UtcDateTime).TotalSeconds < explodeCooldown - 0.5f)
        {
            return;
        }
        RpcExplode();
    }

    public GameObject CreateBomb(Vector2 pos)
    {
        GameObject bombObj = new GameObject("Bomb");
        bombObj.layer = LayerMask.NameToLayer("Objects");
        bombObj.transform.position = pos;
        bombObj.transform.SetZ(pos.y / 1000f);

        //Add Sprite
        SpriteRenderer renderer = bombObj.AddComponent<SpriteRenderer>();
        renderer.material = CachedMaterials.Instance.SpriteDefault;
        renderer.sprite = bomb;

        return bombObj;
    }

    public List<PlayerControl> GetPlayersInArea(Vector2 area, float radius)
    {
        List<PlayerControl> players = new List<PlayerControl>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(area, radius, Constants.PlayersOnlyMask);
        foreach (Collider2D collider in colliders)
        {
            PlayerControl player = collider.GetComponent<PlayerControl>();
            if (player != null && !player.Data.Disconnected && !player.Data.IsDead && player != Player)
            {
                players.Add(player);
            }
        }
        return players;
    }

    public IEnumerator Explode() //Some parts of this code should only be run by host. (e.g murderplayer)
    {
        uses++;
        Vector2 explosionSource = Player.GetTruePosition();
        GameObject bomb = CreateBomb(explosionSource);
        bombEnabled = true;
        LastExplosion = DateTime.UtcNow;
        explodeTimer = explodeCooldown;


        List<PlayerControl> alertedPlayers = GetPlayersInArea(explosionSource, 2.5f);

        if (alertedPlayers.Any(p => p.IsMe)) 
        {
            SoundManager.Instance.PlaySound(explosion, false);
        }
        else
        {
            SoundManager.Instance.PlaySound(explosion, false, 0.25f);
        }

        yield return new WaitForSeconds(0.23f);
        yield return HudManager.Instance.CoFadeFullScreen(Palette.DisabledColor, new Color(255, 0, 0, 0.4f), 0.3f);

        yield return new WaitForSeconds(2f);
        yield return HudManager.Instance.CoFadeFullScreen(Palette.DisabledColor, new Color(0, 0, 0, 0), 0.2f);

        //New logic coming later logic:
        //Return a list of all player controls in that area
        //If you are one of them, the effects will start
        //The effects will do a flash in every second, because it will be like a 3 2 1
        //If you are host, you will kill them

        bomb.transform.SetAllScale(3f);
        bomb.GetComponent<SpriteRenderer>().color = Color.black;

        List<PlayerControl> affectedPlayers = GetPlayersInArea(explosionSource, 3f);
        AnalogGlitch glitch = Camera.main.GetComponent<AnalogGlitch>();
        glitch.enabled = true;
        glitch.verticalJump = -0.3f;
        glitch.scanLineJitter = 0.2f;
        glitch.colorDrift = 0.1f;

        DigitalGlitch digitalGlitch = Camera.main.gameObject.GetComponent<DigitalGlitch>();
        digitalGlitch.enabled = true;
        digitalGlitch.intensity = 0.8f;

        if (AmongUsClient.Instance.AmHost)
        {
            foreach (var player in affectedPlayers)
            {
                if (player != null && !player.Data.Disconnected && !player.Data.IsDead && player != Player)
                {
                    player.RpcMurderPlayer(player, MurderResultFlags.Succeeded);
                }
            }
        }

        yield return new WaitForSeconds(0.4f);
        glitch.verticalJump = 0f;
        glitch.scanLineJitter = 0f;
        glitch.colorDrift = 0f;
        glitch.enabled = false;

        digitalGlitch.intensity = 0f;
        digitalGlitch.enabled = false;

        if (!affectedPlayers.Any(p => p.IsMe))
        {
            SoundManager.Instance.PlaySound(earRing, false, 0.3f);
        }

        yield return new WaitForSeconds(2f);
        SoundManager.Instance.StopSound(earRing);
        UnityObject.Destroy(bomb);
        bombEnabled = false;

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

    public override void OnAssign(PlayerControl player)
    {
        base.OnAssign(player);
        explodeTimer = explodeCooldown;
    }
}