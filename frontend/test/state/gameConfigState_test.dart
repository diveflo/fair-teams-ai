import 'package:flutter_test/flutter_test.dart';
import 'package:NoCrybabies/model/candidate.dart';
import 'package:NoCrybabies/model/map.dart';
import 'package:NoCrybabies/state/gameConfigState.dart';

void main() {
  group("initial constructor", () {
    test("sets game config state with 12 candidates", () {
      GameConfigState gameConfig = GameConfigState.initial();

      expect(gameConfig.candidates.length, 14);
    });
  });

  group("fromJson", () {
    test("sets all values from json", () {
      var gameConfigInputJson = {
        "candidates": [
          {
            "name": "player1",
            "steamID": "player1id",
            "isSelected": true,
          },
          {
            "name": "player2",
            "steamID": "player2id",
            "isSelected": false,
          }
        ],
        "mapPool": [
          {
            "name": "inferno",
            "imagePath": "inferno.png",
            "isChecked": true,
          }
        ],
        "includeBot": false,
      };

      GameConfigState gameConfig =
          GameConfigState.fromJson(gameConfigInputJson);

      expect(gameConfig.candidates.length, 2);
      expect(gameConfig.mapPool.maps.length, 1);
      expect(gameConfig.includeBot, false);
    });
  });

  test("toJson", () {
    var expectedJson = {
      "candidates": [
        {
          "name": "player1",
          "steamID": "player1id",
          "isSelected": true,
        },
        {
          "name": "player2",
          "steamID": "player2id",
          "isSelected": false,
        }
      ],
      "mapPool": [
        {
          "name": "inferno",
          "imagePath": "inferno.png",
          "isChecked": true,
        }
      ],
      "includeBot": false,
    };

    GameConfigState gameConfig = GameConfigState(
      candidates: [
        Candidate(
          name: "player1",
          steamID: "player1id",
          isSelected: true,
        ),
        Candidate(
          name: "player2",
          steamID: "player2id",
          isSelected: false,
        )
      ],
      mapPool: MapPool.fromMaps([
        CsMap(
          name: "inferno",
          imagePath: "inferno.png",
          isChecked: true,
        )
      ]),
      includeBot: false,
    );

    var convertedGameConfigJson = gameConfig.toJson();

    expect(convertedGameConfigJson, expectedJson);
  });
}
