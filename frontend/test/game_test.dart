import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/game.dart';

void main() {
  test("toJson", () {
    var inputJson = {
      "terrorists": {
        "name": "Terror",
        "players": [
          {
            "name": "Player1",
            "steamID": "10001",
            "skill": {"skillScore": 0}
          },
          {
            "name": "Player2",
            "steamID": "10002",
            "skill": {"skillScore": 0}
          }
        ]
      },
      "counterTerrorists": {
        "name": "Anti",
        "players": [
          {
            "name": "Player3",
            "steamID": "10003",
            "skill": {"skillScore": 0}
          },
          {
            "name": "Player4",
            "steamID": "10004",
            "skill": {"skillScore": 0}
          }
        ]
      }
    };

    Game testi = Game.fromJson(inputJson);

    expect(testi.t.players.length, 2);
    expect(testi.ct.players.length, 2);

    expect(testi.t.players.first.name, "Player1");
    expect(testi.t.players.first.steamID, "10001");
    expect(testi.t.players[1].name, "Player2");
    expect(testi.t.players[1].steamID, "10002");

    expect(testi.ct.players.first.name, "Player3");
    expect(testi.ct.players.first.steamID, "10003");
    expect(testi.ct.players[1].name, "Player4");
    expect(testi.ct.players[1].steamID, "10004");
  });
}
