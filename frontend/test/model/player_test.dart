import 'package:flutter_test/flutter_test.dart';
import 'package:no_cry_babies/model/player.dart';

void main() {
  group("constructor", () {
    test("set default steam ID", () {
      Player testi = Player(name: "testi");

      expect(testi.name, "testi");
      expect(testi.steamID, "");
    });

    test("steam ID as set", () {
      Player testi = Player(name: "testiMcTesti", steamID: "02AA20202");

      expect(testi.name, "testiMcTesti");
      expect(testi.steamID, "02AA20202");
    });
  });
  group("toJson", () {
    test("convert to api request json", () {
      Player testi = Player(
        name: "player1",
        steamName: "player1",
        steamID: "id122",
      );

      var testiJson = testi.toJson();

      expect(testiJson, {
        "name": "player1",
        "steamName": "player1",
        "steamID": "id122",
        "skill": {
          "skillScore": double.maxFinite,
          "skillTrend": "Plateau",
          "rank": "NotRanked"
        }
      });
    });
  });

  group("fromJson", () {
    test("convert", () {
      var json = {
        "name": "testi",
        "steamName": "boon",
        "steamID": "001",
        "skill": {"skillScore": 0.1, "skillTrend": "Upwards", "rank": "master"}
      };

      Player testi = Player.fromJson(json);

      expect(testi.name, "testi");
      expect(testi.steamName, "boon");
      expect(testi.steamID, "001");
      expect(testi.skill.skillScore, 0.1);
      expect(testi.skill.rank, "master");
    });
  });
}
