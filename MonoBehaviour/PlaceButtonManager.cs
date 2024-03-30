using TMPro;
using UnityEngine;

namespace TrolleyTest
{
    public class PlaceButtonManager : AbilityButtonManager
    {
        public TextMeshPro UseText;

        public override string abilityName
        {
            get
            {
                string langName = TranslationController.Instance.CurrentLanguage.langName;
                if (langName == "Português") return "BOTAR";
                return "PLACE";
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            spriteRender.sprite = RoleManager.Instance.allSprites["explodeSprite"];
        }

        public override void DoClick()
        {
            if (base.isActiveAndEnabled)
            {
                BomberRole bomber = PlayerControl.LocalPlayer.Data.myRole as BomberRole;
                bomber.CmdCheckExplode();
            }
        }

        internal void FixedUpdate()
        {
            if (GameFast.Local == null && GameFast.Local.Data.myRole == null) return;
            if (GameFast.Local.Data.myRole.roleCodeName != "BomberRole") Destroy(gameObject);
        }
    }
}
