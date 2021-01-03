import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/candidate.dart';
import 'package:frontend/state/gameConfigState.dart';

void main() {
  group("initial constructor", () {
    test("sets game config state with 12 candidates", () {
      GameConfigState gameConfig = GameConfigState.initial();

      expect(gameConfig.candidates.length, 12);
    });
  });
  group("json converter", () {
    GameConfigState gameConfig = GameConfigState(
      candidates: [
        Candidate(
          name: "player1",
          isProfilePublic: true,
          steamID: "player1id",
          isSelected: true,
        ),
        Candidate(
          name: "player2",
          isProfilePublic: false,
          steamID: "player2id",
          isSelected: false,
        )
      ],
    );

    var gameConfigJson = {
      "candidates": [
        {
          "name": "player1",
          "steamID": "player1id",
          "isProfilePublic": true,
          "isSelected": true,
        },
        {
          "name": "player2",
          "steamID": "player2id",
          "isProfilePublic": false,
          "isSelected": false,
        }
      ]
    };
    test("convert state to json", () {
      var convertedGameConfigJson = gameConfig.toJson();

      expect(convertedGameConfigJson, gameConfigJson);
    });

    test("converts json to appState", () {
      var convertedAppState = GameConfigState.fromJson(gameConfigJson);

      expect(convertedAppState.candidates.length, 2);
    });
  });
}
