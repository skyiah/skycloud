﻿using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A user data object that is a principal.
    ///
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const int
            WX = 1,
            CREATTED = 2,
            BACKEND = 0x00f0, // inclusive
            CREDENTIAL = 0x0010,
            PERM = 0x0100;

        public const short MANAGER = 7, AID = 3;

        public static readonly Map<short, string> OPR = new Map<short, string>
        {
            [MANAGER] = "经理",
            [AID] = "助理"
        };

        internal string wx; // wexin openid
        internal string name;
        internal string id; // optional unique id
        internal string credential;
        internal string tel;
        internal string city; // default viewing city
        internal string addr;
        internal string oprat; // operator at shopid
        internal short opr; // 
        internal bool adm; // admininistrator
        internal short status;


        public void Read(IDataInput i, int proj = 0x00ff)
        {
            if ((proj & WX) == WX)
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(tel), ref tel);

            if ((proj & BACKEND) != 0) // inclusive
            {
                i.Get(nameof(id), ref id);
                if ((proj & CREDENTIAL) == CREDENTIAL)
                {
                    i.Get(nameof(credential), ref credential);
                }
            }
            if ((proj & PERM) == PERM)
            {
                i.Get(nameof(oprat), ref oprat);
                i.Get(nameof(opr), ref opr);
                i.Get(nameof(adm), ref adm);
            }
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & WX) == WX)
            {
                o.Put(nameof(wx), wx);
            }
            o.Put(nameof(name), name);
            o.Put(nameof(city), city);
            o.Put(nameof(addr), addr);
            o.Put(nameof(tel), tel);

            if ((proj & BACKEND) != 0)
            {
                o.Put(nameof(id), id);
                if ((proj & CREDENTIAL) == CREDENTIAL)
                {
                    o.Put(nameof(credential), credential);
                }
            }
            if ((proj & PERM) == PERM)
            {
                o.Put(nameof(oprat), oprat);
                o.Put(nameof(opr), opr);
                o.Put(nameof(adm), adm);
            }
        }
    }
}