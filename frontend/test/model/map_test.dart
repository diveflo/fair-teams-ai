import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/map.dart';

void main() {
  group("constructor", () {
    test("init default maps", () {
      MapPool defaultMaps = MapPool();

      expect(defaultMaps.maps.length, 7);
    });
  });

  group("convert", () {
    MapPool mapPool = MapPool.fromMaps([
      CsMap(name: "inferno", imagePath: "i.png", isDismissed: false),
      CsMap(name: "nuke", imagePath: "n.png", isDismissed: true)
    ]);

    var mapPoolJson = {
      "mapPool": [
        {
          "name": "inferno",
          "imagePath": "i.png",
          "isDismissed": false,
        },
        {
          "name": "nuke",
          "imagePath": "n.png",
          "isDismissed": true,
        }
      ]
    };

    test("toJson", () {
      var convertedJson = mapPool.toJson();

      expect(convertedJson, mapPoolJson["mapPool"]);
    });

    test("fromJson", () {
      MapPool convertedMapPool = MapPool.fromJson(mapPoolJson);

      expect(convertedMapPool.maps.length, 2);
      expect(convertedMapPool.maps.first.name, "inferno");
      expect(convertedMapPool.maps.last.imagePath, "n.png");
      expect(convertedMapPool.maps.first.isDismissed, false);
      expect(convertedMapPool.maps.last.isDismissed, true);
    });
  });

  group("getPlayableMaps", () {
    test("all maps are playable", () {
      MapPool mapPool = MapPool.fromMaps([
        CsMap(name: "inferno", imagePath: "i.png", isDismissed: false),
        CsMap(name: "nuke", imagePath: "n.png", isDismissed: false)
      ]);

      List<CsMap> playable = mapPool.getPlayableMaps();

      expect(playable.length, 2);
      expect(playable.first.name, "inferno");
      expect(playable.last.name, "nuke");
    });

    test("one map is playable", () {
      MapPool mapPool = MapPool.fromMaps([
        CsMap(name: "inferno", imagePath: "i.png", isDismissed: false),
        CsMap(name: "nuke", imagePath: "n.png", isDismissed: true)
      ]);

      List<CsMap> playable = mapPool.getPlayableMaps();

      expect(playable.length, 1);
      expect(playable.first.name, "inferno");
    });

    test("no map is playable", () {
      MapPool mapPool = MapPool.fromMaps([
        CsMap(name: "inferno", imagePath: "i.png", isDismissed: true),
        CsMap(name: "nuke", imagePath: "n.png", isDismissed: true)
      ]);

      List<CsMap> playable = mapPool.getPlayableMaps();

      expect(playable.length, 0);
      expect(playable.isEmpty, true);
    });
  });
}
