using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainEnviron : DbEnviron
    {
        static ChainPeer info;

        // a bundle of connected peers
        static readonly Map<short, ChainClient> clients = new Map<short, ChainClient>(32);

        // validates ops and archives demostic blocks 
        static Thread archiver;

        // periodic polling and importing of foreign blocks 
        static Thread importer;

        public static ChainPeer Info
        {
            get => info;
            internal set => info = value;
        }

        public static ChainClient GetChainClient(short peerid) => clients[peerid];

        public static Map<short, ChainClient> Clients => clients;


        internal async Task ReloadNative(DbContext dc)
        {
            dc.Sql("SELECT ").collst(ChainPeer.Empty).T(" FROM chain.peers WHERE native = TRUE");
            info = await dc.QueryTopAsync<ChainPeer>();
        }


        /// <summary>
        /// Sets up and start blockchain on this peer node.
        /// </summary>
        public static async Task StartChainAsync()
        {
            // clear up data maps
            clients.Clear();

            //load this node peer
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(ChainPeer.Empty).T(" FROM chain.peers WHERE native = TRUE");
                info = await dc.QueryTopAsync<ChainPeer>();
                // get current blockid
                if (info != null)
                {
                    await info.PeekLastBlockAsync(dc);
                }
            }

            // start the archiver thead
            archiver = new Thread(Archive);
            archiver.Start();

            // load foreign peer connectors
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(ChainPeer.Empty).T(" FROM chain.peers WHERE native = FALSE");
                var arr = await dc.QueryAsync<ChainPeer>();
                if (arr != null)
                {
                    foreach (var o in arr)
                    {
                        var cli = new ChainClient(o);
                        clients.Add(cli);

                        // init current block id
                        await o.PeekLastBlockAsync(dc);
                    }
                }
            }

            // start the importer thead
            importer = new Thread(Import);
            importer.Start();
        }


        const int MAX_BLOCK_SIZE = 64;

        const int MIN_BLOCK_SIZE = 8;

        static async void Archive(object state)
        {
            while (info != null && info.IsRunning)
            {
                Thread.Sleep(90 * 1000); // 90 seconds interval

                // archiving cycle 
                Cycle:
                using (var dc = NewDbContext(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        dc.Sql("SELECT ").collst(FlowState.Empty).T(" FROM chain.ops WHERE status = ").T(FlowOp.STATUS_ENDED).T(" ORDER BY stamp LIMIT ").T(MAX_BLOCK_SIZE);
                        var arr = await dc.QueryAsync<FlowState>();
                        if (arr == null || arr.Length < MIN_BLOCK_SIZE)
                        {
                            continue; // go for delay
                        }

                        // resolve last block
                        if (info.blockid == 0)
                        {
                            await info.PeekLastBlockAsync(dc);
                        }
                        info.IncrementBlockId();

                        // insert archivals
                        //
                        long bchk = 0; // current block checksum
                        dc.Sql("INSERT INTO chain.blocks ").colset(FlowState.Empty, extra: "peerid, seq, cs, blockcs")._VALUES_(FlowState.Empty, extra: "@1, @2, @3, @4");
                        for (short i = 0; i < arr.Length; i++)
                        {
                            var o = arr[i];
                            // set parameters
                            var p = dc.ReCommand();
                            p.Digest = true;
                            o.Write(p);
                            p.Digest = false;
                            CryptoUtility.Digest(p.Checksum, ref bchk);
                            p.Set(info.id).Set(ChainUtility.WeaveSeq(info.blockid, i)).Set(p.Checksum); // set primary and checksum
                            // set block-wise digest
                            if (i == 0) // begin of block 
                            {
                                p.Set(info.blockcs);
                            }
                            else if (i == arr.Length - 1) // end of block
                            {
                                p.Set(info.blockcs = bchk); // assign & set
                            }
                            else
                            {
                                p.SetNull();
                            }
                            await dc.SimpleExecuteAsync();
                        }

                        // delete operationals
                        //
                        dc.Sql("DELETE FROM chain.ops WHERE status = ").T(FlowOp.STATUS_ENDED).T(" AND job = @1 AND step = @2");
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var o = arr[i];
                            await dc.ExecuteAsync(p => p.Set(o.job).Set(o.step));
                        }
                    }
                    catch (Exception e)
                    {
                        dc.Rollback();
                        ServerEnviron.ERR(e.Message);
                        return; // quit the loop
                    }
                }
                goto Cycle;
            }
        }

        static async void Import(object state)
        {
            while (true)
            {
                Cycle:
                var outer = true;
                Thread.Sleep(60 * 1000); // 60 seconds delay

                for (int i = 0; i < clients.Count; i++) // LOOP
                {
                    var cli = clients.ValueAt(i);

                    if (!cli.Info.IsRunning) continue;

                    int busy = 0;
                    var code = cli.TryReap(out var arr);
                    if (code == 200)
                    {
                        try
                        {
                            using var dc = NewDbContext(IsolationLevel.ReadCommitted);
                            long bchk = 0; // block checksum
                            for (short k = 0; k < arr.Length; k++)
                            {
                                var o = arr[k];

                                // long seq = ChainUtility.WeaveSeq(blockid, i);
                                dc.Sql("INSERT INTO chain.blocks ").colset(ChainState.Empty, extra: "peerid, cs, blockcs")._VALUES_(ChainState.Empty, extra: "@1, @2, @3");
                                // direct parameter setting
                                var p = dc.ReCommand();
                                p.Digest = true;
                                o.Write(p);
                                p.Digest = false;

                                // validate record-wise checksum
                                if (o.cs != p.Checksum)
                                {
                                    cli.SetDataError(o.seq);
                                    break;
                                }

                                CryptoUtility.Digest(p.Checksum, ref bchk);
                                p.Set(p.Checksum); // set primary and checksum
                                // block-wise digest
                                if (i == 0) // begin of block 
                                {
                                    if (o.blockcs != bchk) // compare with previous block
                                    {
                                        cli.SetDataError("");
                                        break;
                                    }

                                    p.Set(bchk);
                                }
                                else if (i == arr.Length - 1) // end of block
                                {
                                    // validate block check
                                    if (bchk != o.blockcs)
                                    {
                                        cli.SetDataError("");
                                    }
                                    p.Set(bchk = o.blockcs); // assign & set
                                }
                                else
                                {
                                    p.SetNull();
                                }

                                await dc.SimpleExecuteAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            cli.SetInternalError(e.Message);
                        }
                    }

                    if (code == 200 || (outer && (code == 0 || code == 204)))
                    {
                        cli.ScheduleRemotePoll(0);
                        busy++;
                    }

                    //
                    if (busy == 0) // to outer for delay
                    {
                        break;
                    }
                    outer = false;
                }
            } // outer
        }
    }
}