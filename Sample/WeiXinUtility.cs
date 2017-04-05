﻿using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class WeiXinUtility
    {
        public const string WXAUTH = "wxauth";

        public const string APPID = "wxd007f5ad60226953";

        public const string APPSECRET = "7884c01588649198c2e83ea8d08891b6";

        public const string ADDR = "http://shop.144000.tv";

        public const string MCH_ID = "1445565602";

        public const string NONCE_STR = "sadfasd2s";

        static readonly Connector WcPay = new Connector("https://api.mch.weixin.qq.com");

        static readonly Connector WeiXinClient = new Connector("https://api.weixin.qq.com");

        public static async Task prepay(long orderid, decimal total, string openid)
        {
            XmlContent xml = new XmlContent();
            xml.ELEM("xml", null, () =>
            {
                xml.ELEM("appid", APPID);
                xml.ELEM("mch_id", MCH_ID);
                xml.ELEM("nonce_str", NONCE_STR);
                xml.ELEM("sign", "");
                xml.ELEM("body", "");
                xml.ELEM("out_trade_no", orderid);
                xml.ELEM("total_fee", total);
                xml.ELEM("notify_url", "");
                xml.ELEM("trade_type", "");
                xml.ELEM("openid", openid);
            });
            var rsp = await WcPay.PostAsync("/pay/unifiedorder", xml);

        }

        public static void GiveRedirectWeiXinAuthorize(this ActionContext ac)
        {
            string redirect_url = System.Net.WebUtility.UrlEncode(ADDR + ac.Uri);
            ac.GiveRedirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + APPID + "&redirect_uri=" + redirect_url + "&response_type=code&scope=snsapi_userinfo&state=" + WXAUTH + "#wechat_redirect");
        }

        public static async Task<AccessToken> GetAccessTokenAsync(string code)
        {
            string url = "/sns/oauth2/access_token?appid=" + APPID + "&secret=" + APPSECRET + "&code=" + code + "&grant_type=authorization_code";
            JObj jo = await WeiXinClient.GetAsync<JObj>(null, url);
            if (jo == null) return default(AccessToken);

            string access_token = jo[nameof(access_token)];
            if (access_token == null)
            {
                string errmsg = jo[nameof(errmsg)];
                return default(AccessToken);
            }
            string openid = jo[nameof(openid)];

            return new AccessToken();
        }

        public static async Task<User> GetUserInfoAsync(string access_token, string openid)
        {
            JObj jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid + "&lang=zh_CN");
            string nickname = jo[nameof(nickname)];
            string city = jo[nameof(city)];
            return new User { wx = openid, nickname = nickname, city = city };
        }

        public struct AccessToken
        {
            internal string access_token;

            internal string openid;
        }
    }
}