using fairTeams.Core;
using fairTeams.DemoHandling.SteamKitExt;
using Microsoft.Extensions.Logging;
using SteamKit2;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fairTeams.DemoHandling
{
    public class GameCoordinatorClient
    {
        private readonly ILogger myLogger;

        private readonly SteamClient mySteamClient;
        private readonly CallbackManager myCallbackManager;

        private readonly SteamUser mySteamUser;

        private bool myGotMatch = false;

        private TaskCompletionSource<bool> myIsLoggedIn = new();

        public GameCoordinatorClient(ILogger<GameCoordinatorClient> logger)
        {
            myLogger = logger;

            mySteamClient = new SteamClient();
            myCallbackManager = new CallbackManager(mySteamClient);

            mySteamUser = mySteamClient.GetHandler<SteamUser>();
            mySteamClient.GetHandler<SteamGameCoordinator>();

            myCallbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            myCallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            myCallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
        }

        public GameCoordinatorClient() : this(UnitTestLoggerCreator.CreateUnitTestLogger<GameCoordinatorClient>()) { }

        public Task<string> GetDownloadURLForMatch(GameRequest request)
        {
            myGotMatch = false;
            mySteamClient.Connect();
            Task.Run(() => HandleCallbacks());
            var connectWaitTimeInMilliseconds = 10000;
            if (!myIsLoggedIn.Task.Wait(connectWaitTimeInMilliseconds))
            {
                myIsLoggedIn = new TaskCompletionSource<bool>();
                throw new Exception($"Steam client didn't connect after {connectWaitTimeInMilliseconds} milliseconds.");
            }

            return RequestGame(request);
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
            myIsLoggedIn = new TaskCompletionSource<bool>();
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            myIsLoggedIn.SetResult(true);
        }

        private Task<string> RequestGame(GameRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            var csgo = new CsgoClient(mySteamClient, myCallbackManager, true);
            csgo.Launch(protobuf =>
            {
                Thread.Sleep(1000);
                csgo.RequestGame(request, list =>
                {
                    var downloadUrl = list.matches.First().roundstatsall.First(x => !string.IsNullOrEmpty(x.map)).map;
                    taskCompletionSource.SetResult(downloadUrl);
                    myGotMatch = true;
                    mySteamClient.Disconnect();
                });
            });

            return taskCompletionSource.Task;
        }
    }
}