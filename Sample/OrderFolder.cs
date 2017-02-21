﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.Order;

namespace Greatbone.Sample
{
    ///
    ///
    public class OrderFolder : WebFolder
    {
        public OrderFolder(WebFolderContext fc) : base(fc)
        {
            CreateVar<OrderVarFolder>();
        }

        //
        // USER
        //

        public void my(WebActionContext ac, string page)
        {
            string userid = ac[0];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE userid = @1 ORDER BY id LIMIT 20 OFFSET @2", p => p.Set(userid).Set(page.ToInt() * 20)))
                {
                    ac.ReplyFolderPage(200, dc.ToList<Order>());
                }
                else
                {
                    ac.Reply(204); // no content
                }
            }
        }

        #region /shop/-shopid-/orderi/ OR /shop/-shopid-/ordero/

        // [Shop]
        [Ui]
        public void @default(WebActionContext ac, string page)
        {
            string shopid = ac[1];
            string key = ac[this];
            if (key.EndsWith("i")) // orderi
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                    {
                        ac.ReplyFolderPage(200, dc.ToList<Order>());
                    }
                    else
                    {
                        ac.ReplyFolderPage(200, (List<Order>)null);
                    }
                }
            }
            else // ordero
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status >= 4 ORDER BY id LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page.ToInt() * 20)))
                    {
                        ac.ReplyFolderPage(200, dc.ToList<Order>());
                    }
                    else
                    {
                        ac.ReplyFolderPage(200, (List<Order>)null);
                    }
                }
            }
        }

        [Ui(Label = "取消")]
        [State(ASKED, FIXED | CANCELLED, CANCELLED)]
        public async Task cancel(WebActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        var order = dc.ToArray<Order>();
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                using (var dc = Service.NewDbContext())
                {
                    dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id IN () AND shopid = @1 AND ").statecond();
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        ac.Reply(303); // see other
                    }
                    else
                    {
                        ac.Reply(303); // see other
                    }
                }
            }
        }

        [Ui(Label = "已备货")]
        [State(ASKED, FIXED | CANCELLED, CANCELLED)]
        public async Task fix(WebActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        public void close(WebActionContext ac)
        {
        }

        [Shop]
        [Ui]
        public void clear(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

        #endregion


        #region /shop/-shopid-/orderout/


        #endregion


        #region /order/

        [Admin]
        [Ui]
        public void exam(WebActionContext ac)
        {

        }

        #endregion
    }
}