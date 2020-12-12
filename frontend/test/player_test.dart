import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/player.dart';

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
      Player testi = Player(name: "player1", steamID: "id122");

      var testiJson = testi.toJson();

      expect(testiJson, {
        "name": "player1",
        "profilePublic": false,
        "steamID": "id122",
        "skill": {}
      });
    });
  });

  group("fromJson", () {
    test("convert", () {
      var json = {
        "name": "testi",
        "steamName": "boon",
        "profilePublic": true,
        "steamID": "001",
        "skill": {"skillScore": 0.1}
      };

      Player testi = Player.fromJson(json);

      expect(testi.name, "testi");
      expect(testi.steamName, "boon");
      expect(testi.profilePublic, true);
      expect(testi.steamID, "001");
      expect(testi.skillScore, 0.1);
    });
  });
}
