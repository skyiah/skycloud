﻿using System.IO;

namespace Greatbone.Core
{
    ///
    /// The context for a particular node in the web work hierarchy.
    ///
    public class WebDirectoryContext
    {
        public string key;


        public string Key => key;

        public object State { get; internal set; }

        public bool IsVariable { get; internal set; }

        public int Level { get; internal set; }

        public WebDirectory Parent { get; internal set; }

        public virtual string Folder { get; internal set; }

        public WebService Service { get; internal set; }


        public string GetFilePath(string file)
        {
            return Path.Combine(Folder, file);
        }
    }
}