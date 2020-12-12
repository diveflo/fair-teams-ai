import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/game.dart';

void main() {
  test("fromJson", () {
    var inputJson = '''{
      "terrorists": {
        "name": "Terror",
        "players": [
          {
            "name": "Player1",
            "steamName": "boon1",
            "profilePublic": true,
            "steamID": "10001",            
            "skill": {"skillScore": 0.0}
          },
          {
            "name": "Player2",
            "steamName": "boon11",
            "profilePublic": true,
            "steamID": "10002",
            "skill": {"skillScore": 0.0}
          }
        ]
      },
      "counterTerrorists": {
        "name": "Anti",
        "players": [
          {
            "name": "Player3",
            "steamName": "boon2",
            "profilePublic": false,
            "steamID": "10003",
            "skill": {"skillScore": 0.0}
          },
          {
            "name": "Player4",
            "steamName": "boon22",
            "profilePublic": false,
            "steamID": "10004",
            "skill": {"skillScore": 0.0}
          }
        ]
      }
    }''';

    Game testi = Game.fromJson(jsonDecode(inputJson));

    expect(testi.t.players.length, 2);
    expect(testi.ct.players.length, 2);

    expect(testi.t.players.first.name, "Player1");
    expect(testi.t.players.first.steamID, "10001");
    expect(testi.t.players.first.steamName, "boon1");
    expect(testi.t.players.first.profilePublic, true);

    expect(testi.t.players[1].name, "Player2");
    expect(testi.t.players[1].steamID, "10002");
    expect(testi.t.players[1].steamName, "boon11");
    expect(testi.t.players[1].profilePublic, true);

    expect(testi.ct.players.first.name, "Player3");
    expect(testi.ct.players.first.steamID, "10003");
    expect(testi.ct.players.first.steamName, "boon2");
    expect(testi.ct.players.first.profilePublic, false);

    expect(testi.ct.players[1].name, "Player4");
    expect(testi.ct.players[1].steamID, "10004");
    expect(testi.ct.players[1].steamName, "boon22");
    expect(testi.ct.players[1].profilePublic, false);
  });
}
