import 'package:flutter/widgets.dart';

class Player {
  String name;
  String steamName;
  String steamID;
  double skillScore;

  bool isSelected;

  Player(
      {@required this.name,
      this.steamName = "Player1",
      this.steamID = "",
      this.isSelected = false,
      this.skillScore = double.maxFinite});

  Player.fromJson(dynamic json) {
    name = json["name"] != null ? json["name"] : "no name found";
    steamName = json["steamName"] != null ? json["steamName"] : "Player1";
    steamID = json["steamID"] != null ? json["steamID"] : ["invalid id"];
    skillScore = json["skill"]["skillScore"] != null
          ? json["skill"]["skillScore"]
          : [0.0];
  }

  Map<String, dynamic> toJson() => {
        'name': name,
        'steamName': steamName,
        'steamID': steamID,
        'skill': {'skillScore': skillScore},
      };
}
