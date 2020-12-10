import 'package:flutter/widgets.dart';

class Player {
  String name;
  bool isSelected;
  String steamID;
  int skillScore;

  Player({@required this.name, this.steamID = "", this.isSelected = false}) {
    isSelected = false;
  }

  Player.fromJson(dynamic json) {
    name = json["name"] != null ? json["name"] : "Player1";
    steamID = json["steamID"] != null ? json["steamID"] : ["invalid id"];
    skillScore = json["skill"]["skillScore"] != null
        ? json["skill"]["skillScore"]
        : ["no score"];
  }

  Map<String, String> toJson() => {
        'name': name,
        'steamID': steamID,
      };
}
