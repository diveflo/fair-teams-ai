import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/player.dart';
import 'package:frontend/model/skill.dart';
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
          steamID: "player1id",
          skill: Skill(skillScore: 1.0),
        ),
        Player(
          name: "player2",
          steamName: "player2",
          steamID: "player2id",
          skill: Skill(skillScore: 2.0),
        ),
      ],
      "terror",
    );
    Team ct = Team(
      [
        Player(
          name: "player3",
          steamName: "player3",
          steamID: "player3id",
          skill: Skill(skillScore: 0.0),
        ),
        Player(
          name: "player4",
          steamName: "player4",
          steamID: "player4id",
          skill: Skill(skillScore: 2.2),
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
            "steamID": "player1id",
            "skill": {
              "skillScore": 1.0,
              "skillTrend": "Plateau",
            }
          },
          {
            "name": "player2",
            "steamName": "player2",
            "steamID": "player2id",
            "skill": {
              "skillScore": 2.0,
              "skillTrend": "Plateau",
            }
          }
        ]
      },
      "counterTerrorists": {
        "name": "ct",
        "players": [
          {
            "name": "player3",
            "steamName": "player3",
            "steamID": "player3id",
            "skill": {
              "skillScore": 0.0,
              "skillTrend": "Plateau",
            }
          },
          {
            "name": "player4",
            "steamName": "player4",
            "steamID": "player4id",
            "skill": {
              "skillScore": 2.2,
              "skillTrend": "Plateau",
            }
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
