﻿using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    /// <summary>A user record that is a web access token for all the services. </summary>
    ///
    public class User : IToken, IPersist
    {
        const ushort MGT = 0x01;

        internal string id;

        internal string name;

        internal string credential;

        internal bool fame;

        internal bool brand;

        internal bool admin;

        internal DateTime date;

        internal FavPost[] favposts;

        internal Fav[] friends;

        internal Fav[] favs;

        public string Key => id;

        public string[] Roles => null;

        public string Name => name;

        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(name), ref name);
            sc.Got(nameof(credential), ref credential);
            sc.Got(nameof(fame), ref fame);
            sc.Got(nameof(brand), ref brand);
            sc.Got(nameof(admin), ref admin);
            if ((x & MGT) == x)
            {
                sc.Got(nameof(date), ref date);
                sc.Got(nameof(favposts), ref favposts);
                sc.Got(nameof(friends), ref friends);
                sc.Got(nameof(favs), ref favs);
            }
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(name), name);
            sk.Put(nameof(credential), credential);
            sk.Put(nameof(fame), fame);
            sk.Put(nameof(brand), brand);
            sk.Put(nameof(admin), admin);
            if ((x & MGT) == x)
            {
                sk.Put(nameof(date), date);
                sk.Put(nameof(favposts), favposts, x);
                sk.Put(nameof(friends), friends, x);
                sk.Put(nameof(favs), favs, x);
            }
        }

        public static string Encrypt(string orig)
        {
            return null;
        }

        public static string Decrypt(string src)
        {
            return null;
        }

        public struct Fav : IPersist
        {
            internal string id;

            public void Load(ISource sc, ushort x = 0xffff)
            {
                throw new NotImplementedException();
            }

            public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
            {
                throw new NotImplementedException();
            }
        }

        public struct FavPost : IPersist
        {
            internal int id;

            public void Load(ISource sc, ushort x = 0xffff)
            {
                throw new NotImplementedException();
            }

            public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
            {
                throw new NotImplementedException();
            }
        }
    }
}