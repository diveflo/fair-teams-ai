import 'package:flutter/widgets.dart';

class Player {
  String name;
  String steamName;
  bool profilePublic;
  String steamID;
  double skillScore;

  bool isSelected;

  Player(
      {@required this.name,
      this.profilePublic = false,
      this.steamID = "",
      this.isSelected = false,
      skillScore = double.maxFinite});

  Player.fromJson(dynamic json) {
    name = json["name"] != null ? json["name"] : "no name found";
    steamName = json["steamName"] != null ? json["steamName"] : "Player1";
    steamID = json["steamID"] != null ? json["steamID"] : ["invalid id"];
    profilePublic = json["profilePublic"];
    if (!profilePublic) {
      skillScore = double.maxFinite;
      return;
    }

    skillScore = json["skill"]["skillScore"] != null
        ? json["skill"]["skillScore"]
        : [0.0];
  }

  Map<String, dynamic> toJson() => {
        'name': name,
        'profilePublic': profilePublic,
        'steamID': steamID,
        'skill': {},
      };
}
