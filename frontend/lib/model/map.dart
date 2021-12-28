import 'package:flutter/widgets.dart';

class MapPool {
  List<CsMap> maps;

  MapPool.fromMaps(List<CsMap> maps) {
    this.maps = maps;
  }

  MapPool() {
    this.maps = [
      CsMap(name: "Inferno", imagePath: "assets/inferno.jpg"),
      CsMap(name: "Dust2", imagePath: "assets/dust2.jpg"),
      CsMap(name: "Mirage", imagePath: "assets/mirage.jpg"),
      CsMap(name: "Nuke", imagePath: "assets/nuke.jpg"),
      CsMap(name: "Overpass", imagePath: "assets/overpass.jpg"),
      CsMap(name: "Vertigo", imagePath: "assets/vertigo.jpg"),
      CsMap(name: "Ancient", imagePath: "assets/ancient.jpg")
    ];
  }

  List<CsMap> getPlayableMaps() {
    List<CsMap> playableMaps = [];
    this.maps.forEach((element) {
      if (element.isChecked) {
        playableMaps.add(element);
      }
    });
    return playableMaps;
  }

  MapPool.fromJson(Map<String, dynamic> json) {
    List<dynamic> mapPoolJson = json["mapPool"];
    maps = mapPoolJson.map((map) => CsMap.fromJson(map)).toList();
  }

  dynamic toJson() => this.maps.map((map) => map.toJson()).toList();
}

class CsMap {
  String name;
  String imagePath;
  bool isChecked;

  CsMap({this.imagePath = "", @required this.name, this.isChecked = true});

  CsMap.fromJson(dynamic json) {
    name = json["name"] != null ? json["name"] : null;
    imagePath = json["imagePath"] != null ? json["imagePath"] : null;
    isChecked = json["isChecked"] != null ? json["isChecked"] : true;
  }

  Map<String, dynamic> toJson() => {
        'name': name,
        'imagePath': imagePath,
        'isChecked': isChecked,
      };
}
