using Kino;
using System.Collections;
using TrolleyTest;
using UnityEngine;
using Hazel;
using UnityObject = UnityEngine.Object;
using System;

public class TrolleyRole : RoleBehaviour
{
    public override string roleDisplayName => "Trolley";
    public override string roleDescription => "Kill someone";
    public override string KillAbilityName => "TROLL";

    public AudioClip goofyAhdio;

    public bool hacking = false;

    public enum RpcCalls
    {
        DoTheTrolling = 0,
    }

    public override void ConfigureRole()
    {
        //The team of the role
        RoleTeamType = RoleTeamTypes.Neutral;
        //The teams that the role can kill
        enemyTeams = new RoleTeamTypes[] { RoleTeamTypes.Impostor, RoleTeamTypes.Crewmate, RoleTeamTypes.Neutral };
        //if the kill button will appear
        CanUseKillButton = true;
        //if he can use vent
        CanVent = false;
        try
        {
            goofyAhdio = Utils.LoadAudioClipFromResource("TrolleyTest.Resources.ohio-goofy-ahh.wav");
        }
        catch { }
    }

    public void RpcTroll()
    {
        if (AmongUsClient.Instance.AmHost) //only host should run that actually
        {
            Troll();
        }
        MessageWriter writer = PlayerControl.LocalPlayer.StartRoleRpc((byte)RpcCalls.DoTheTrolling);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public void Troll()
    {
        if (Player.AmOwner)
        {
            Player.Data.myRole.SetKillTimer(500f);
        }
        StopAllCoroutines();
        if (DestroyableSingleton<TutorialManager>.InstanceExists) StartCoroutine(DespawnFreeplay());
        else StartCoroutine(DespawnCoroutine());
    }

    public IEnumerator DespawnCoroutine()
    {
        AnalogGlitch analogGlitch = Camera.main.GetComponent<AnalogGlitch>();
        analogGlitch.enabled = true;
        analogGlitch.scanLineJitter = 0.05f;
        hacking = true;
        HudManager.Instance.Notifier.AddItem("<color=white>Pietro: Hey");
        yield return new WaitForSeconds(2);
        HudManager.Instance.Notifier.AddItem("<color=white>Pietro: Would be funny if i destroyed the game.");
        yield return new WaitForSeconds(3);
        HudManager.Instance.Notifier.AddItem("<color=white>Pietro: You know what?!</color>");
        yield return new WaitForSeconds(5);
        HudManager.Instance.Notifier.AddItem("5");
        analogGlitch.colorDrift = 0.05f;
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("4");
        analogGlitch.colorDrift = 0.1f;
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("3");
        analogGlitch.colorDrift = 0.15f;
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("2");
        analogGlitch.colorDrift = 0.2f;
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("1");
        analogGlitch.colorDrift = 0.25f;
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("0");
        analogGlitch.colorDrift = 0.3f;
        yield return new WaitForSeconds(1);
        yield return HudManager.Instance.CoFadeFullScreen(Color.black, Color.red, 0.5f);
        Application.Quit();
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        HudManager.Instance.FullScreen.color = GetRandomColor();
    }

    public static Color GetRandomColor()
    {
        float randomR = UnityEngine.Random.value;
        float randomG = UnityEngine.Random.value;
        float randomB = UnityEngine.Random.value;

        return new Color(randomR, randomG, randomB);
    }

    public IEnumerator DespawnFreeplay()
    {
        SoundManager.Instance.PlaySound(goofyAhdio, false);
        AnalogGlitch analogGlitch = Camera.main.GetComponent<AnalogGlitch>();
        hacking = true;
        analogGlitch.enabled = true;
        analogGlitch.scanLineJitter = 0.05f;
        yield return new WaitForSeconds(5);
        analogGlitch.colorDrift = 0.05f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 0.1f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 0.15f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 0.2f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 0.25f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 0.3f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 0.4f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 0.5f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 0.6f;
        yield return new WaitForSeconds(1);
        analogGlitch.colorDrift = 1f;
        yield return HudManager.Instance.CoFadeFullScreen(Color.black, Color.red, 1f);
        yield return new WaitForSeconds(2);
        Application.Quit();
    }

    public override bool CheckMurder(PlayerControl target)
    {
        if (DateTime.UtcNow.Subtract(Player.Data.LastMurder.UtcDateTime).TotalSeconds < Player.Data.myRole.KillCooldown - 0.5f)
        {
            return false;
        }
        RpcTroll();
        return false;    
    }

    public override void HandleRpc(MessageReader reader, int rpc)
    {
        switch ((RpcCalls)rpc)
        {
            case RpcCalls.DoTheTrolling:
            Troll();
            break;
        }
    }
}