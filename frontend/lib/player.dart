import 'package:flutter/widgets.dart';

class Player {
  final String name;
  bool isSelected;
  final String steamID;

  Player({@required this.name, this.steamID = "", this.isSelected = false}) {
    isSelected = false;
  }

  Map<String, String> toJson() => {
        'name': name,
        'steamID': steamID,
      };
}