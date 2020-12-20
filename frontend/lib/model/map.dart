import 'package:flutter/widgets.dart';

class MapPool {
  List<CsMap> maps;

  MapPool() {
    this.maps = [
      CsMap(name: "Inferno", imagePath: "inferno.jpg"),
      CsMap(name: "Dust2", imagePath: "dust2.jpg"),
      CsMap(name: "Mirage", imagePath: "mirage.jpg"),
      CsMap(name: "Nuke", imagePath: "nuke.jpg"),
      CsMap(name: "Train", imagePath: "train.jpg"),
      CsMap(name: "Overpass", imagePath: "overpass.jpg"),
      CsMap(name: "Vertigo", imagePath: "vertigo.jpg"),
    ];
  }
}

class CsMap {
  final String name;
  final String imagePath;
  bool isDismissed;

  CsMap({this.imagePath = "", @required this.name, this.isDismissed = false});
}
