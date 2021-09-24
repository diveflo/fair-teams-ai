using fairTeams.Core;
using fairTeams.DemoHandling.SteamKitExt;
using Microsoft.Extensions.Logging;
using SteamKit2;
using SteamKit2.GC.CSGO.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fairTeams.DemoHandling
{
    internal enum CallbackType
    {
        Connected,
        Disconnected,
        LoggedOn
    }

    public sealed class GameCoordinatorClient : IDisposable
    {
        private bool myIsDisposing;
        private readonly IDictionary<CallbackType, IDisposable> myRegisteredCallbacks;
        private readonly ILoggerFactory myLoggerFactory;
        private readonly ILogger myLogger;

        private readonly SteamClient mySteamClient;
        private readonly SteamUser mySteamUser;
        private readonly CallbackManager myCallbackManager;
        private CsgoClient myCsgoClient;

        private const int myWaitTimeInMilliseconds = 30000;

        public GameCoordinatorClient(ILoggerFactory loggerFactory)
        {
            myLoggerFactory = loggerFactory;
            myLogger = myLoggerFactory.CreateLogger<GameCoordinatorClient>();

            myLogger.LogTrace("Creating GameCoordinatorClient instance");

            mySteamClient = new SteamClient();
            myCallbackManager = new CallbackManager(mySteamClient);
            mySteamUser = mySteamClient.GetHandler<SteamUser>();

            myIsDisposing = false;
            myRegisteredCallbacks = new Dictionary<CallbackType, IDisposable>();

            Task.Run(() => HandleCallbacks());
        }

        public GameCoordinatorClient() : this(UnitTestLoggerCreator.CreateUnitTestLoggerFactory()) { }

        public Match GetMatchInfo(Demo demo)
        {
            var match = new Match { Demo = demo };

            try
            {
                var matchInfo = RequestGame(demo.GameRequest, myCsgoClient).Result;
                demo.DownloadURL = GetDownloadURL(matchInfo);
                match.Date = GetMatchDate(matchInfo);
                match.Rounds = GetNumberOfRounds(matchInfo);
                (match.CTScore, match.TScore) = GetScore(matchInfo);
                match.PlayerResults = GetMatchPlayerStatistics(matchInfo);
            }
            catch (AggregateException e)
            {
                var innerExceptions = e.InnerExceptions;

                if (innerExceptions.Any(x => x is GameCoordinatorException))
                {
                    throw innerExceptions.Single(x => x is GameCoordinatorException);
                }

                if (innerExceptions.Any(x => x is TimeoutException))
                {
                    var timeoutMessage = innerExceptions.Single(x => x is TimeoutException).Message;
                    myLogger.LogWarning(timeoutMessage);
                    throw new GameCoordinatorException(timeoutMessage);
                }

                throw;
            }

            return match;
        }

        public Rank GetRank(long steamId)
        {
            var accountId = SteamIdDecoder.ToAccountId(steamId);
            var rank = Rank.NotRanked;

            try
            {
                var rankId = GetRank(accountId, myCsgoClient).Result;
                rank = (Rank)rankId;
            }
            catch (AggregateException e)
            {
                var innerExceptions = e.InnerExceptions;

                if (innerExceptions.Any(x => x is GameCoordinatorException))
                {
                    throw innerExceptions.Single(x => x is GameCoordinatorException);
                }

                if (innerExceptions.Any(x => x is TimeoutException))
                {
                    var timeoutMessage = innerExceptions.Single(x => x is TimeoutException).Message;
                    myLogger.LogWarning(timeoutMessage);
                    throw new GameCoordinatorException(timeoutMessage);
                }

                throw;
            }

            return rank;
        }

        private Task<uint> GetRank(uint accountId, CsgoClient csgoClient)
        {
            var taskCompletionSource = TaskHelper.CreateTaskCompletionSourceWithTimeout<uint>(
                myWaitTimeInMilliseconds,
                $"Game coordinator didn't provide profile within {myWaitTimeInMilliseconds} milliseconds.");

            myLogger.LogTrace("Asking game coordinator for rank");

            if (csgoClient == null)
            {
                myLogger.LogError("CsGoClient is unexpectedly null");
                taskCompletionSource.SetException(new GameCoordinatorException("CsGoClient is unexpectedly null"));
                return taskCompletionSource.Task;
            }

            csgoClient.PlayerProfileRequest(accountId, callback =>
            {
                if (callback.account_profiles == null || !callback.account_profiles.Any())
                {
                    taskCompletionSource.SetException(new GameCoordinatorException($"Empty response while trying to get rank for account id {accountId}"));
                }

                var profile = callback.account_profiles.First();
                if (profile.ranking != null)
                {
                    var rankId = profile.ranking.rank_id;
                    taskCompletionSource.SetResult(rankId);
                }
                else
                {
                    myLogger.LogError($"Couldn't get rank for account id {accountId}");
                    taskCompletionSource.SetException(new GameCoordinatorException($"Couldn't get rank for account id {accountId}"));
                }
            });

            return taskCompletionSource.Task;
        }

        public void ConnectAndLogin()
        {
            try
            {
                Connect().Wait();

                UnregisterCallback(CallbackType.Connected);
                UnregisterCallback(CallbackType.Disconnected);

                var loginResult = Login().Result;
                UnregisterCallback(CallbackType.LoggedOn);
                if (loginResult != EResult.OK)
                {
                    throw new GameCoordinatorException($"Couldn't login to the steam client. Result code: {loginResult}");
                }

                myLogger.LogTrace("Successfully connected and logged in to steam account.");
                myCsgoClient = ConnectToCSGOGameCoodinator().Result;
            }
            catch (AggregateException e)
            {
                var innerExceptions = e.InnerExceptions;

                if (innerExceptions.Any(x => x is GameCoordinatorException))
                {
                    throw innerExceptions.Single(x => x is GameCoordinatorException);
                }

                if (innerExceptions.Any(x => x is TimeoutException))
                {
                    var timeoutMessage = innerExceptions.Single(x => x is TimeoutException).Message;
                    myLogger.LogError(timeoutMessage);
                    throw new GameCoordinatorException(timeoutMessage);
                }

                throw;
            }
        }

        private Task Connect()
        {
            var taskCompletionSource = TaskHelper.CreateResultlessTaskCompletionSourceWithTimeout(myWaitTimeInMilliseconds, $"Steam client didn't connect within {myWaitTimeInMilliseconds} milliseconds.");

            if (mySteamClient.IsConnected)
            {
                taskCompletionSource.SetResult();
            }
            else
            {
                var connectedCallbackRegistration = myCallbackManager.Subscribe<SteamClient.ConnectedCallback>((callback) =>
                {
                    myLogger.LogTrace("Successfully connected to steam");
                    taskCompletionSource.SetResult();
                    UnregisterCallback(CallbackType.Connected);
                });

                var disconnectedCallbackRegistration = myCallbackManager.Subscribe<SteamClient.DisconnectedCallback>((callback) =>
                {
                    myLogger.LogError("Steam server seems to be down. Please try again.");
                    taskCompletionSource.SetException(new GameCoordinatorException("Steam server seems to be down. Please try again."));
                });

                myRegisteredCallbacks.Add(CallbackType.Connected, connectedCallbackRegistration);
                myRegisteredCallbacks.Add(CallbackType.Disconnected, disconnectedCallbackRegistration);

                mySteamClient.Connect();
                myLogger.LogTrace("Connecting to Steam...");
            }

            return taskCompletionSource.Task;
        }

        private Task<EResult> Login()
        {
            var taskCompletionSource = TaskHelper.CreateTaskCompletionSourceWithTimeout<EResult>(myWaitTimeInMilliseconds);

            if (!mySteamClient.IsConnected)
            {
                myLogger.LogCritical("Steam client wasn't connected while trying to login. How?");
                taskCompletionSource.SetException(new GameCoordinatorException("Steam client wasn't connected while trying to login. How?"));
                return taskCompletionSource.Task;
            }

            if (mySteamClient.SessionID != null)
            {
                taskCompletionSource.SetResult(EResult.OK);
            }
            else
            {
                var loggedOnCallbackRegistration = myCallbackManager.Subscribe<SteamUser.LoggedOnCallback>((callback) =>
                {
                    myLogger.LogTrace($"Log on result: {callback.Result}");
                    taskCompletionSource.SetResult(callback.Result);
                });

                myRegisteredCallbacks.Add(CallbackType.LoggedOn, loggedOnCallbackRegistration);

                var steamCredentials = new SteamUser.LogOnDetails { Username = Settings.SteamUsername, Password = Settings.SteamPassword };
                try
                {
                    mySteamUser.LogOn(steamCredentials);
                }
                catch (ArgumentException)
                {
                    myLogger.LogCritical("You need to provide a Steam account via the environment variables STEAM_USERNAME and STEAM_PASSWORD");
                    taskCompletionSource.SetException(new Exception("No steam account provided via environment variables STEAM_USERNAME and STEAM_PASSWORD"));
                }
            }

            return taskCompletionSource.Task;
        }

        private Task LogOff()
        {
            var taskCompletionSource = TaskHelper.CreateResultlessTaskCompletionSourceWithTimeout(myWaitTimeInMilliseconds, $"Steam user didn't log off within {myWaitTimeInMilliseconds} milliseconds.");

            UnregisterCallback(CallbackType.Disconnected);

            myRegisteredCallbacks[CallbackType.Disconnected] = myCallbackManager.Subscribe<SteamClient.DisconnectedCallback>((callback) =>
            {
                myLogger.LogInformation("Successfully logged off & disconnected steam client");
                taskCompletionSource.SetResult();
            });

            mySteamUser.LogOff();

            return taskCompletionSource.Task;
        }

        private void HandleCallbacks()
        {
            while (!myIsDisposing)
            {
                myCallbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        private Task<CsgoClient> ConnectToCSGOGameCoodinator()
        {
            var taskCompletionSource = TaskHelper.CreateTaskCompletionSourceWithTimeout<CsgoClient>(myWaitTimeInMilliseconds, $"Game coordinator didn't welcome us after {myWaitTimeInMilliseconds} milliseconds.");

            var csgo = new CsgoClient(mySteamClient, myCallbackManager, myLoggerFactory.CreateLogger<CsgoClient>());
            myLogger.LogTrace("Telling Steam we're playing CS:GO to connect to game coordinator.");
            csgo.Launch((callback) =>
            {
                taskCompletionSource.SetResult(csgo);
            });

            return taskCompletionSource.Task;
        }

        private Task<CDataGCCStrike15_v2_MatchInfo> RequestGame(GameRequest request, CsgoClient csgoClient)
        {
            var taskCompletionSource = TaskHelper.CreateTaskCompletionSourceWithTimeout<CDataGCCStrike15_v2_MatchInfo>(myWaitTimeInMilliseconds);

            myLogger.LogTrace("Asking game coordinator for match details.");
            //Thread.Sleep(5000);
            if (csgoClient == null)
            {
                myLogger.LogError("CsGoClient is unexpectedly null");
                taskCompletionSource.SetException(new GameCoordinatorException("CsGoClient is unexpectedly null"));
                return taskCompletionSource.Task;
            }
            csgoClient.RequestGame(request, matchList =>
            {
                if (matchList == null || matchList.matches == null)
                {
                    myLogger.LogError("Unexpected empty result from game coodinator (matchList or matchList.matches is null)");
                    taskCompletionSource.SetException(new GameCoordinatorException("Unexpected empty result from game coodinator (matchList.matches is null)"));
                    return;
                }

                if (!matchList.matches.Any())
                {
                    myLogger.LogWarning("Game coordinator doesn't have match details (probably expired).");
                    taskCompletionSource.SetException(new GameCoordinatorException("Game coordinator doesn't have match details (probably expired)."));
                    return;
                }

                var matchInfo = matchList.matches.First();
                taskCompletionSource.SetResult(matchInfo);
            });

            return taskCompletionSource.Task;
        }

        private DateTime GetMatchDate(CDataGCCStrike15_v2_MatchInfo matchInfo)
        {
            if (matchInfo.matchtime == 0)
            {
                myLogger.LogWarning($"Couldn't extract match date/time. Assuming 'now'.");
                return DateTime.Now;
            }

            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            date = date.AddSeconds(matchInfo.matchtime);
            myLogger.LogTrace($"Extracted match date/time: {date}");
            return date;
        }

        private IList<MatchStatistics> GetMatchPlayerStatistics(CDataGCCStrike15_v2_MatchInfo matchInfo)
        {
            var roundStats = matchInfo.roundstatsall;
            var finalRoundStats = roundStats.Last();

            var accountIds = finalRoundStats.reservation.account_ids;

            var players = new List<MatchStatistics>();
            
            for (var i = 0; i < accountIds.Count; i++)
            {
                var steamId = SteamIdDecoder.ToSteamId(accountIds[i]);
                var playerStatistics = new MatchStatistics
                {
                    SteamID = steamId,
                    InitialSide = i < 5 ? Side.CounterTerrorists : Side.Terrorists
                };

                players.Add(playerStatistics);
            }

            return players;
        }

        private string GetDownloadURL(CDataGCCStrike15_v2_MatchInfo matchInfo)
        {
            var roundStats = matchInfo.roundstatsall;
            try
            {
                var downloadUrl = roundStats.First(x => !string.IsNullOrEmpty(x.map)).map;
                myLogger.LogTrace($"Extract download url: {downloadUrl}");
                return downloadUrl;
            }
            catch (InvalidOperationException)
            {
                myLogger.LogWarning("MatchInfo doesn't contain download url ('map' property on any of the 'roundstatsall').");
                throw new GameCoordinatorException("MatchInfo doesn't contain download url ('map' property on any of the 'roundstatsall').");
            }
        }

        private static int GetNumberOfRounds(CDataGCCStrike15_v2_MatchInfo matchInfo)
        {
            var roundStats = matchInfo.roundstatsall;
            return roundStats.Count;
        }

        private static (int counterTerroristsScore, int terroristsScore) GetScore(CDataGCCStrike15_v2_MatchInfo matchInfo)
        {
            var roundStats = matchInfo.roundstatsall;
            var finalRoundStats = roundStats.Last();
            return (finalRoundStats.team_scores[0], finalRoundStats.team_scores[1]);
        }

        private void UnregisterCallback(CallbackType type)
        {
            if (myRegisteredCallbacks.ContainsKey(type))
            {
                myRegisteredCallbacks[type]?.Dispose();
                myRegisteredCallbacks.Remove(type);
            }
        }

        public void Dispose()
        {
            myLogger.LogTrace("Disposing GameCoordinatorClient");

            if (myCsgoClient != null)
            {
                myCsgoClient.Dispose();
            }

            try
            {
                LogOff().Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException is TimeoutException)
                {
                    myLogger.LogWarning(e.InnerException.Message);
                    return;
                }

                throw;
            }
            finally
            {
                foreach (var registeredCallback in myRegisteredCallbacks.Values)
                {
                    registeredCallback.Dispose();
                }

                myIsDisposing = true;
                myCallbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }
    }
}