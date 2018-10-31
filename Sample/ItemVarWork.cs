using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class ItemVarWork : Work
    {
        const int PicAge = 60 * 60;

        protected ItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(WebContext wc)
        {
            short id = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE id = @1", p => p.Set(id)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, PicAge);
                }
                else wc.Give(404, @public: true, maxage: PicAge); // not found
            }
        }

        public void img(WebContext wc)
        {
            short id = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT img FROM items WHERE id = @1", p => p.Set(id)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, PicAge);
                }
                else wc.Give(404, @public: true, maxage: PicAge); // not found
            }
        }
    }


    public class SampItemVarWork : ItemVarWork
    {
        public SampItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            var prin = (User) wc.Principal;
            string hubid = wc[0];
            short itemid = wc[this];
            int uid = wc.Query[nameof(uid)];
            if (uid == 0)
            {
                uid = prin.id;
            }
            bool door = false;
            short qty;
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE hubid = @1 AND id = @2");
                var o = dc.Query1<Item>(p => p.Set(hubid).Set(itemid));
                dc.Query1("SELECT sum(qty) FROM orders WHERE hubid = @1 AND status BETWEEN 0 AND 5 AND itemid = @2 GROUP BY itemid", p => p.Set(hubid).Set(itemid));
                dc.Let(out o.ongoing);
                wc.GivePage(200, h =>
                {
                    h.DIV_(css: "uk-inline");
                    h.PIC_(circle: false).T("img")._PIC();
                    h.DIV_(css: "uk-overlay uk-overlay-primary uk-position-bottom").H4(o.name)._DIV();
                    h._DIV();

                    h.T(o.remark);

                    h.FORM_(post: true, css: "uk-bottom-bar uk-flex-between");
                    h.T('¥').T(o.price).T("/").T(o.unit);
                    h.NUMBER(null, nameof(qty), o.min, max: o.Avail, min: o.min, step: o.step == 0 ? (short) 1 : o.step, onchange: "this.form.total.value=this.value;");

                    h.T("<output name=\"total\">").T(o.min * o.price).T("</output>");

                    h.HIDDEN(nameof(uid), uid);
                    h.HIDDEN(nameof(itemid), itemid);
                    h.TOOL(nameof(prepay));

                    h._FORM();
                });
            }
        }


        [Ui("付款"), Tool(ButtonScript, "uk-button-danger")]
        public async Task prepay(WebContext wc)
        {
            var prin = (User) wc.Principal;
            string hubid = wc[0];
            short itemid = wc[this];
            var f = await wc.ReadAsync<Form>();
            int uid = f[nameof(uid)];
            short qty = f[nameof(qty)];
            bool door = f[nameof(door)];
            Order o;
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE id = @1");
                var m = dc.Query1<Item>(p => p.Set(itemid));
                dc.Sql("SELECT ").collst(User.Empty).T(" FROM users WHERE id = @1");
                var u = dc.Query1<User>(p => p.Set(uid));
                // create and insert a new order
                o = new Order()
                {
                    hubid = hubid,
                    custid = uid,
                    cust = u.name,
                    custtel = u.tel,
                    custaddr = u.addr,
                    teamid = u.teamid,
                    itemid = itemid,
                    item = m.name,
                    price = m.price,
                    qty = qty,
                    unit = m.unit,
                    total = m.price * qty,
                    creatorid = prin.id,
                    creator = prin.name,
                    custwx = prin.wx,
                };
                dc.Sql("INSERT INTO orders ")._(Order.Empty, 0)._VALUES_(Order.Empty, 0).T(" RETURNING id");
                dc.Query1(p => o.Write(p, 0));
                dc.Let(out o.id);
            }
            // call WeChatPay to prepare order there
            var hub = Obtain<Map<string, Hub>>()[hubid];
            var (prepay_id, _) = await hub.PostUnifiedOrderAsync(
                o.id.ToString(),
                o.total,
                o.custwx, // the payer is always the current user
                wc.RemoteAddr.ToString(),
                SampUtility.NetAddr + "/" + hub.id + "/" + nameof(SampVarWork.onpay),
                hub.name
            );
            if (prepay_id != null)
            {
                wc.Give(200, hub.BuildPrepayContent(prepay_id));
            }
            else
            {
                wc.Give(500);
            }
        }
    }

    public class HublyItemVarWork : ItemVarWork
    {
        public HublyItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [UserAuthorize(hubly: 7)]
        [Ui("属性"), Tool(ButtonShow, size: 2)]
        public async Task upd(WebContext wc)
        {
            string hubid = wc[0];
            short id = wc[this];
            if (wc.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE id = @1");
                    var o = dc.Query1<Item>(p => p.Set(id));
                    wc.GivePane(200, h =>
                    {
                        h.FORM_().FIELDUL_("填写商品属性");
                        h.LI_().TEXT("品　名", nameof(o.name), o.name, max: 10, required: true)._LI();
                        h.LI_().TEXTAREA("简　介", nameof(o.descr), o.descr, max: 100, min: 20, required: true)._LI();
                        h.LI_().TEXTAREA("说　明", nameof(o.remark), o.descr, max: 500, min: 100, required: true)._LI();
                        h.LI_().TEXT("单　位", nameof(o.unit), o.unit, required: true).LABEL("冷　藏").CHECKBOX(nameof(o.refrig), o.refrig)._LI();
                        h.LI_().NUMBER("价　格", nameof(o.price), o.price, required: true).NUMBER("工坊占", nameof(o.shopp), o.shopp, required: true)._LI();
                        h.LI_().NUMBER("派运占", nameof(o.senderp), o.senderp, required: true).NUMBER("团组占", nameof(o.teamp), o.teamp, required: true)._LI();
                        h.LI_().NUMBER("起　订", nameof(o.min), o.min, min: (short) 1).NUMBER("递　增", nameof(o.step), o.step, min: (short) 1)._LI();
                        h._FIELDUL()._FORM();
                    });
                }
            }
            else // POST
            {
                const byte proj = 0;
                var o = await wc.ReadObjectAsync<Item>(proj);
                o.hubid = hubid;
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj).T(" WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(id);
                    });
                }
                wc.GivePane(200); // close
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("图标"), Tool(ButtonCrop, size: 1)]
        public new async Task icon(WebContext wc)
        {
            if (wc.IsGet)
            {
                base.icon(wc);
            }
            else // POST
            {
                short id = wc[this];
                var f = await wc.ReadAsync<Form>();
                ArraySegment<byte> img = f[nameof(img)];
                using (var dc = NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE id = @2", p => p.Set(img).Set(id)) > 0)
                    {
                        wc.Give(200); // ok
                    }
                    else wc.Give(500); // internal server error
                }
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("照片"), Tool(ButtonCrop, size: 2)]
        public new async Task img(WebContext wc)
        {
            if (wc.IsGet)
            {
                base.img(wc);
            }
            else // POST
            {
                short id = wc[this];
                var f = await wc.ReadAsync<Form>();
                ArraySegment<byte> img = f[nameof(img)];
                using (var dc = NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET img = @1 WHERE id = @2", p => p.Set(img).Set(id)) > 0)
                    {
                        wc.Give(200); // ok
                    }
                    else wc.Give(500); // internal server error
                }
            }
        }

        [Ui("供应"), Tool(ButtonShow, size: 2)]
        public async Task plan(WebContext wc)
        {
            string hubid = wc[0];
            short id = wc[this];
            short shopid;
            short cap7;
            short status;
            if (wc.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    dc.Query1("SELECT shopid, cap7, status FROM items WHERE id = @1", p => p.Set(id));
                    dc.Let(out shopid).Let(out cap7).Let(out status);
                    wc.GivePane(200, h =>
                    {
                        var shops = Obtain<Map<short, Shop>>();
                        h.FORM_();
                        h.FIELDUL_("填写供应信息");
                        h.LI_().NUMBER("周产能", nameof(cap7), cap7, required: true)._LI();
                        h.LI_().SELECT("工　坊", nameof(shopid), shopid, shops)._LI();
                        h.LI_().SELECT("状　态", nameof(status), status, Item.Statuses)._LI();
                        h._FIELDUL();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                var f = await wc.ReadAsync<Form>();
                cap7 = f[nameof(cap7)];
                shopid = f[nameof(shopid)];
                status = f[nameof(status)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE items SET cap7 = @1, shopid = @2, status = @3 WHERE id = @4");
                    dc.Execute(p => p.Set(cap7).Set(shopid).Set(status).Set(id));
                }
                wc.GivePane(200); // close
            }
        }
    }
}