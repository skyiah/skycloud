using SkyCloud.Web;

namespace SkyCloud.Chain
{
    [Ui("General")]
    public class AdmlyWork : WebWork
    {
        protected internal override void OnCreate()
        {
            CreateWork<PeerWork>("peer");

            CreateWork<LoginWork>("login");

            CreateWork<TypWork>("typ");

            CreateWork<BlockWork>("block");
        }

        public void @default(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            if (inner)
            {
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.FORM_("uk-card uk-card-primary");
                    h._UL();
                    h._FORM();
                });
            }
            else
            {
                wc.GiveFrame(200, false, 3, "Node Management");
            }
        }
    }
}