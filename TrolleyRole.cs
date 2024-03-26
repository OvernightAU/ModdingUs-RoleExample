using System.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

public class TrolleyRole : RoleBehaviour
{
    public override string roleDisplayName => "Trolley";
    public override string roleDescription => "Kill someone";

    public override void ConfigureRole()
    {
        RoleTeamType = RoleTeamTypes.Neutral;
        enemyTeams = new RoleTeamTypes[] { RoleTeamTypes.Impostor, RoleTeamTypes.Crewmate, RoleTeamTypes.Neutral };
        CanUseKillButton = true;
        CanVent = false;
    }

    public IEnumerator DespawnCoroutine()
    {
        HudManager.Instance.Notifier.AddItem("<color=white>Pietro: Hey");
        yield return new WaitForSeconds(2);
        HudManager.Instance.Notifier.AddItem("<color=white>Pietro: Would be funny if i destroyed the game.");
        yield return new WaitForSeconds(3);
        HudManager.Instance.Notifier.AddItem("<color=white>Pietro: You know what?!</color>");
        yield return new WaitForSeconds(5);
        HudManager.Instance.Notifier.AddItem("5");
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("4");
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("3");
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("2");
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("1");
        yield return new WaitForSeconds(1);
        HudManager.Instance.Notifier.AddItem("0");
        yield return new WaitForSeconds(1);
        yield return HudManager.Instance.CoFadeFullScreen(Color.black, Color.red, 0.5f);
        Application.Quit();
    }

    public override bool CheckMurder(PlayerControl target)
    {
        StopAllCoroutines();
        StartCoroutine(DespawnCoroutine());
        return false;
    }
}