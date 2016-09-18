﻿using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The notice service.</summary>
    ///
    public class NoticeHub : WebModule
    {
        public NoticeHub(WebConfig cfg) : base(cfg)
        {
            SetVarHub<NoticeVarHub>(false);
        }

        /// <summary>
        /// Gets the specified top page from the notices table. 
        /// </summary>
        /// <param name="page">page number</param>
        public override void Default(WebContext wc)
        {
            int page;
            wc.GetParam("page", out page);

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Query("SELECT * FROM notices WHERE duedate <= current_date ORDER BY id LIMIT 20 OFFSET @offset", p => p.Set("@offset", page * 20)))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }

        /// <summary>
        /// Gets the specified top page from the notices table. 
        /// </summary>
        public void New(WebContext wc)
        {
            int page;
            wc.GetParam("page", out page);

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Query("INSERT INTO notices () VALUES ()", p => p
                    .Set("@offset", page * 20)
                    .Set("@offset", page * 20)
                ))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }

    }
}