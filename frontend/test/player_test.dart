import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/player.dart';

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
    test("convert name and steamID", () {
      Player testi = Player(name: "player1", steamID: "id122");

      var testiJson = testi.toJson();

      expect(testiJson, {
        "name": "player1",
        "steamID": "id122",
      });
    });
  });
}
