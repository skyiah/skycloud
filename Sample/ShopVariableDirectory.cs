using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The shop variable directory.
    ///
    public class ShopVariableDirectory : WebDirectory, IVariable
    {
        readonly WebAction _re_menu_;

        public ShopVariableDirectory(WebMake mk) : base(mk)
        {
            Make<MyCartDirectory>("mycart");

            // order functions
            Make<MyOrderDirectory>("myorder");

            _re_menu_ = GetAction(nameof(remenu));
        }

        ///
        /// Get products and submit to basket.
        ///
        /// <code>
        /// GET /330001/
        /// </code>
        ///
        public void @default(WebExchange wc)
        {
            string shopid = wc.Major;
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Item.Empty)._("FROM items WHERE shopid = @1 AND NOT disabled");
                if (dc.Query(sql.ToString(), p => p.Put(shopid)))
                {
                    var items = dc.ToDatas<Item>();
                    wc.SendHtmlMajor(200, "", main =>
                    {
                        main.form(_re_menu_, p =>
                        {

                        });
                    });
                }
                else
                    wc.SendHtmlMajor(200, "没有记录", main => { });
            }
        }

        ///
        /// recreate menu for this shop with WeChat.
        ///
        public void remenu(WebExchange wc)
        {
        }


        public void basket(WebExchange wc)
        {
        }

        public void invoice(WebExchange wc)
        {
        }
    }
}