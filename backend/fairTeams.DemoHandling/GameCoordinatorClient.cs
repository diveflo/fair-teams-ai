using fairTeams.Core;
using fairTeams.DemoHandling.SteamKitExt;
using Microsoft.Extensions.Logging;
using SteamKit2;
using SteamKit2.GC.CSGO.Internal;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fairTeams.DemoHandling
{
    public class GameCoordinatorClient
    {
        private readonly ILoggerFactory myLoggerFactory;
        private readonly ILogger myLogger;

        private readonly SteamClient mySteamClient;
        private readonly CallbackManager myCallbackManager;

        private readonly SteamUser mySteamUser;

        private bool myGotMatch = false;

        private TaskCompletionSource<bool> myIsLoggedIn = new();

        public GameCoordinatorClient(ILoggerFactory loggerFactory)
        {
            myLoggerFactory = loggerFactory;
            myLogger = myLoggerFactory.CreateLogger<GameCoordinatorClient>();

            mySteamClient = new SteamClient();
            myCallbackManager = new CallbackManager(mySteamClient);

            mySteamUser = mySteamClient.GetHandler<SteamUser>();
            mySteamClient.GetHandler<SteamGameCoordinator>();

            myCallbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            myCallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            myCallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
        }

        public GameCoordinatorClient() : this(UnitTestLoggerCreator.CreateUnitTestLoggerFactory()) { }

        public Match GetMatchInfo(Demo demo)
        {
            myGotMatch = false;
            mySteamClient.Connect();
            myLogger.LogTrace("Connecting to Steam...");
            Task.Run(() => HandleCallbacks());
            var waitTimeInMilliseconds = 20000;
            if (!myIsLoggedIn.Task.Wait(waitTimeInMilliseconds))
            {
                myIsLoggedIn = new TaskCompletionSource<bool>();
                throw new GameCoordinatorException($"Steam client didn't connect after {waitTimeInMilliseconds} milliseconds.");
            }

            var requestGameTask = RequestGame(demo.GameRequest);

            try
            {
                var successWithinTimeout = requestGameTask.Wait(waitTimeInMilliseconds);
                if (!successWithinTimeout)
                {
                    myLogger.LogWarning($"Game coordinator didn't provide match details after {waitTimeInMilliseconds} milliseconds. Aborting.");
                    throw new GameCoordinatorException($"Steam didn't answer our request for the game download url after 10000 milliseconds.");
                }
            }
            catch (AggregateException e)
            {
                var innerExceptions = e.InnerExceptions;
                if (innerExceptions.Any(x => x is GameCoordinatorException))
                {
                    throw innerExceptions.Single(x => x is GameCoordinatorException);
                }

                myLogger.LogWarning($"Game coordinator threw unexpected exceptions: {e.InnerExceptions}");
                throw new GameCoordinatorException($"Unexpected exception(s) thrown from game coordinator: {e.Message}");
            }
            finally
            {
                mySteamClient.Disconnect();
                myGotMatch = true;
            }

            var matchInfo = requestGameTask.Result;
            demo.DownloadURL = GetDownloadURL(matchInfo);
            var match = new Match
            {
                Demo = demo,
                Date = GetMatchDate(matchInfo)
            };

            return match;
        }

        private void HandleCallbacks()
        {
            while (!myGotMatch)
            {
                myCallbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        private void OnConnected(SteamClient.ConnectedCallback callback)
        {
            myLogger.LogTrace("Connected to Steam. Logging in now...");

            var steamCredentials = new SteamUser.LogOnDetails { Username = Settings.SteamUsername, Password = Settings.SteamPassword };
            try
            {
                mySteamUser.LogOn(steamCredentials);
            }
            catch (ArgumentException)
            {
                myLogger.LogCritical("You need to provide a Steam account via the environment variables STEAM_USERNAME and STEAM_PASSWORD");
                throw new Exception("No steam account provided via environment variables STEAM_USERNAME and STEAM_PASSWORD");
            }
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            myLogger.LogTrace("Disconnected from Steam.");
            myIsLoggedIn = new TaskCompletionSource<bool>();
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            myLogger.LogTrace("Logged in to Steam.");
            myIsLoggedIn.SetResult(true);
        }

        private Task<CDataGCCStrike15_v2_MatchInfo> RequestGame(GameRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<CDataGCCStrike15_v2_MatchInfo>();

            var csgo = new CsgoClient(mySteamClient, myCallbackManager, myLoggerFactory.CreateLogger<CsgoClient>());
            myLogger.LogTrace("Telling Steam we're playing CS:GO to connect to game coordinator.");
            csgo.Launch(protobuf =>
            {
                myLogger.LogTrace("Asking game coordinator for match details.");
                Thread.Sleep(5000);
                csgo.RequestGame(request, list =>
                {
                    if (!list.matches.Any())
                    {
                        myLogger.LogWarning("Game coordinator doesn't have match details (probably expired).");
                        taskCompletionSource.SetException(new GameCoordinatorException("Game coordinator doesn't have match details (probably expired)."));
                        return;
                    }

                    var matchInfo = list.matches.First();
                    taskCompletionSource.SetResult(matchInfo);
                });
            });

            return taskCompletionSource.Task;
        }

        private static DateTime GetMatchDate(CDataGCCStrike15_v2_MatchInfo matchInfo)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return date.AddSeconds(matchInfo.matchtime);
        }

        private string GetDownloadURL(CDataGCCStrike15_v2_MatchInfo matchInfo)
        {
            var roundStats = matchInfo.roundstatsall;
            try
            {
                var downloadUrl = roundStats.First(x => !string.IsNullOrEmpty(x.map)).map;
                return downloadUrl;
            }
            catch (InvalidOperationException)
            {
                myLogger.LogWarning("MatchInfo doesn't contain download url ('map' property on any of the 'roundstatsall').");
                throw new GameCoordinatorException("MatchInfo doesn't contain download url ('map' property on any of the 'roundstatsall').");
            }
        }
    }
}