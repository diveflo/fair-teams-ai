using fairTeams.Core;
using Microsoft.Extensions.Logging;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;
using System;
using System.Timers;

namespace fairTeams.DemoHandling.SteamKitExt
{
    public partial class CsgoClient : IDisposable
    {
        private const int CsgoAppid = 730;
        private readonly SteamGameCoordinator myGameCoordinator;

        private readonly CallbackStore myCallbackStore = new();
        private readonly IDisposable myOnGcMessageSubscription;

        private readonly SteamClient mySteamClient;
        private readonly SteamUser mySteamUser;

        private readonly Timer HelloTimer;

        private readonly ILogger<CsgoClient> myLogger;


        public CsgoClient(SteamClient steamClient, CallbackManager callbackManager, ILogger<CsgoClient> logger)
        {
            mySteamClient = steamClient;
            mySteamUser = steamClient.GetHandler<SteamUser>();
            myGameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            myLogger = logger;

            myOnGcMessageSubscription = callbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGcMessage);

            HelloTimer = new Timer(1000)
            {
                AutoReset = true
            };
            HelloTimer.Elapsed += Knock;
        }

        public CsgoClient(SteamClient steamClient, CallbackManager callbackManager) : this(steamClient, callbackManager, UnitTestLoggerCreator.CreateUnitTestLogger<CsgoClient>()) { }

        private void Knock(object state, ElapsedEventArgs elapsedEventArgs)
        {
            myLogger.LogTrace("Knocking to get welcomed by csgo game coodinator.");
            var clientmsg = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
            myGameCoordinator.Send(clientmsg, CsgoAppid);
        }

        private void OnGcMessage(SteamGameCoordinator.MessageCallback obj)
        {
            myLogger.LogTrace($"GC Message: {Enum.GetName(typeof(ECsgoGCMsg), obj.EMsg) ?? Enum.GetName(typeof(EMsg), obj.EMsg)}");

            if (obj.EMsg == (uint)EGCBaseClientMsg.k_EMsgGCClientWelcome)
            {
                HelloTimer.Stop();
            }

            if (!myCallbackStore.TryGetValue(obj.EMsg, out Action<IPacketGCMsg> func))
            {
                return;
            }

            func(obj.Message);
        }

        public void Launch(Action<CMsgClientWelcome> callback)
        {
            myCallbackStore.Add((uint)EGCBaseClientMsg.k_EMsgGCClientWelcome,
                msg => callback(new ClientGCMsgProtobuf<CMsgClientWelcome>(msg).Body));

            myLogger.LogTrace("Launching CSGO");

            var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = CsgoAppid
            });

            mySteamClient.Send(playGame);

            HelloTimer.Start();
        }

        public void Dispose()
        {
            myOnGcMessageSubscription.Dispose();

            HelloTimer.Elapsed -= Knock;
            HelloTimer.Stop();
            HelloTimer.Dispose();
        }
    }
}