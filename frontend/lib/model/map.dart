import 'package:flutter/widgets.dart';

class MapPool {
  List<CsMap> maps;

  MapPool.fromMaps(List<CsMap> maps) {
    this.maps = maps;
  }

  MapPool() {
    this.maps = [
      CsMap(
          name: "Inferno",
          imagePath: "assets/inferno.jpg",
          imageMapCallsPath: "assets/inferno_calls.jpeg"),
      CsMap(
          name: "Dust2",
          imagePath: "assets/dust2.jpg",
          imageMapCallsPath: "assets/dust2_calls.jpeg"),
      CsMap(
          name: "Mirage",
          imagePath: "assets/mirage.jpg",
          imageMapCallsPath: "assets/mirage_calls.jpeg"),
      CsMap(
          name: "Nuke",
          imagePath: "assets/nuke.jpg",
          imageMapCallsPath: "assets/nuke_calls.jpeg"),
      CsMap(
          name: "Overpass",
          imagePath: "assets/overpass.jpg",
          imageMapCallsPath: "assets/overpass_calls.jpeg"),
      CsMap(
          name: "Vertigo",
          imagePath: "assets/vertigo.jpg",
          imageMapCallsPath: "assets/vertigo_calls.jpeg"),
      CsMap(
        name: "Ancient",
        imagePath: "assets/ancient.jpg",
      )
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
  String imageMapCallsPath;
  bool isChecked;

  CsMap({
    this.imagePath = "",
    @required this.name,
    this.imageMapCallsPath,
    this.isChecked = true,
  });

  CsMap.fromJson(dynamic json) {
    name = json["name"] != null ? json["name"] : null;
    imagePath = json["imagePath"] != null ? json["imagePath"] : null;
    imageMapCallsPath =
        json["imageMapCallsPath"] != null ? json["imageMapCallsPath"] : null;
    isChecked = json["isChecked"] != null ? json["isChecked"] : true;
  }

  Map<String, dynamic> toJson() => {
        'name': name,
        'imagePath': imagePath,
        'imageMapCallsPath': imageMapCallsPath,
        'isChecked': isChecked,
      };
}
