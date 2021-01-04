using Microsoft.Extensions.Logging;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using System;

namespace fairTeams.DemoHandling.SteamKitExt
{
    public partial class CsgoClient
    {
        public void PlayerProfileRequest(uint accountId, Action<CMsgGCCStrike15_v2_PlayersProfile> callback)
        {
            // For gods sake don't ask what the 32 is, i just copied it
            PlayerProfileRequest(accountId, 32, callback);
        }

        public void PlayerProfileRequest(uint accountId, uint requestLevel, Action<CMsgGCCStrike15_v2_PlayersProfile> callback)
        {
            myCallbackStore.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_PlayersProfile,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_PlayersProfile>(msg).Body));

            myLogger.LogTrace($"Requesting profile for account: {accountId}");

            var clientMsgProtobuf =
                new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientRequestPlayersProfile>(
                    (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientRequestPlayersProfile)
                {
                    Body =
                    {
                        account_id = accountId,
                        request_level = requestLevel
                    }
                };

            myGameCoordinator.Send(clientMsgProtobuf, CsgoAppid);
        }
    }
}