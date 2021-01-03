using Microsoft.Extensions.Logging;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;
using System;
using System.Timers;

namespace fairTeams.DemoHandling.SteamKitExt
{
    /// <summary>
    ///     Client for CSGO, allows basic operations such as requesting ranks
    /// </summary>
    public partial class CsgoClient
    {
        //APP ID for csgo
        private const int CsgoAppid = 730;
        private readonly SteamGameCoordinator _gameCoordinator;

        //Contains the callbacks
        private readonly CallbackStore _gcMap = new CallbackStore();

        private readonly SteamClient _steamClient;
        private readonly SteamUser _steamUser;

        private readonly System.Timers.Timer HelloTimer;

        private readonly ILogger<CsgoClient> myLogger;


        public CsgoClient(SteamClient steamClient, CallbackManager callbackManager, ILogger<CsgoClient> logger)
        {
            _steamClient = steamClient;
            _steamUser = steamClient.GetHandler<SteamUser>();
            _gameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            myLogger = logger;

            callbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGcMessage);

            HelloTimer = new System.Timers.Timer(1000);
            HelloTimer.AutoReset = true;
            HelloTimer.Elapsed += Knock;
        }

        private void Knock(object state, ElapsedEventArgs elapsedEventArgs)
        {
            Console.WriteLine("Knocking");
            var clientmsg = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
            _gameCoordinator.Send(clientmsg, CsgoAppid);
        }

        private void OnGcMessage(SteamGameCoordinator.MessageCallback obj)
        {
            myLogger.LogTrace($"GC Message: {Enum.GetName(typeof(ECsgoGCMsg), obj.EMsg) ?? Enum.GetName(typeof(EMsg), obj.EMsg)}");

            if (obj.EMsg == (uint)EGCBaseClientMsg.k_EMsgGCClientWelcome)
                HelloTimer.Stop();

            Action<IPacketGCMsg> func;
            if (!_gcMap.TryGetValue(obj.EMsg, out func))
                return;

            func(obj.Message);
        }

        /// <summary>
        ///     Launches the game
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes</param>
        public void Launch(Action<CMsgClientWelcome> callback)
        {
            _gcMap.Add((uint)EGCBaseClientMsg.k_EMsgGCClientWelcome,
                msg => callback(new ClientGCMsgProtobuf<CMsgClientWelcome>(msg).Body));

            myLogger.LogTrace("Launching CSGO");

            var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = CsgoAppid
            });

            _steamClient.Send(playGame);

            HelloTimer.Start();
        }
    }
}