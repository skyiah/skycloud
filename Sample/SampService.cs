﻿using Greatbone;

namespace Samp
{
    /// <summary>
    /// The sample service that hosts all official accounts.
    /// </summary>
    public class SampService : Service
    {
        public SampService(ServiceConfig cfg) : base(cfg)
        {
            MakeVar<SampVarWork>();
        }
    }
}