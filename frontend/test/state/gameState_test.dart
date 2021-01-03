import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/player.dart';
import 'package:frontend/model/team.dart';
import 'package:frontend/state/gameState.dart';

void main() {
  group("initial constructor", () {
    test("sets initial game state", () {
      GameState game = GameState.initial();

      expect(game.isLoading, false);
      expect(game.ct.players.length, 0);
      expect(game.t.players.length, 0);
    });
  });

  group("json converter", () {
    Team t = Team(
      [
        Player(
          name: "player1",
          steamName: "player1",
          profilePublic: true,
          steamID: "player1id",
          skillScore: 1.0,
        ),
        Player(
          name: "player2",
          steamName: "player2",
          profilePublic: false,
          steamID: "player2id",
          skillScore: 2.0,
        ),
      ],
      "terror",
    );
    Team ct = Team(
      [
        Player(
          name: "player3",
          steamName: "player3",
          profilePublic: true,
          steamID: "player3id",
          skillScore: 0.0,
        ),
        Player(
          name: "player4",
          steamName: "player4",
          profilePublic: false,
          steamID: "player4id",
          skillScore: 2.2,
        ),
      ],
      "ct",
    );
    GameState game = GameState(t: t, ct: ct, isLoading: false);
    var gameJson = {
      "terrorists": {
        "name": "terror",
        "players": [
          {
            "name": "player1",
            "steamName": "player1",
            "profilePublic": true,
            "steamID": "player1id",
            "skill": {"skillScore": 1.0}
          },
          {
            "name": "player2",
            "steamName": "player2",
            "profilePublic": false,
            "steamID": "player2id",
            "skill": {"skillScore": 2.0}
          }
        ]
      },
      "counterTerrorists": {
        "name": "ct",
        "players": [
          {
            "name": "player3",
            "steamName": "player3",
            "profilePublic": true,
            "steamID": "player3id",
            "skill": {"skillScore": 0.0}
          },
          {
            "name": "player4",
            "steamName": "player4",
            "profilePublic": false,
            "steamID": "player4id",
            "skill": {"skillScore": 2.2}
          }
        ]
      }
    };
    test("convert state to json", () {
      var convertedGameStateJson = game.toJson();

      expect(convertedGameStateJson, gameJson);
    });

    test("convert json to state", () {
      var convertedGameState = GameState.fromJson(gameJson);

      expect(convertedGameState.ct.name, "ct");
      expect(convertedGameState.t.name, "terror");
      expect(convertedGameState.t.players.length, 2);
      expect(convertedGameState.ct.players.length, 2);
      expect(convertedGameState.isLoading, false);
    });
  });
}
