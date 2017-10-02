﻿using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public abstract class KickWork<V> : Work where V : KickVarWork
    {
        protected KickWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, int>(obj => ((Kick) obj).id);
        }
    }

    [Ui("我的投诉")]
    [User]
    public class MyKickWork : KickWork<MyKickVarWork>
    {
        public MyKickWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM kicks WHERE wx = @1 ORDER BY id DESC", p => p.Set(wx)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Kick>(0xffff), (h, o) =>
                    {
                        h.COL("姓名", o.name);
                        h.COL("内容", o.content);
                    });
                }
                else
                {
                    ac.GiveGridPage(200, (Kick[]) null, null);
                }
            }
        }
    }

    [Ui("举报管理")]
    [User(adm: true)]
    public class AdmKickWork : KickWork<AdmKickVarWork>
    {
        public AdmKickWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                const int proj = 0x00ff;
                dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM kicks ORDER BY id DESC LIMIT 20 OFFSET @1");
                if (dc.Query(p => p.Set(page * 20)))
                {
                    ac.GiveTablePage(200, dc.ToArray<Item>(), h => { h.TH(""); }, (h, o) => { });
                }
                else
                {
                    ac.GiveTablePage(200, (Item[]) null, null, null);
                }
            }
        }
    }
}