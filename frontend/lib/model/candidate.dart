import 'package:flutter/widgets.dart';

class Candidate {
  String name;
  String steamID;
  bool isProfilePublic;
  bool isSelected;

  Candidate({
    @required this.name,
    this.steamID = "",
    this.isSelected = false,
    this.isProfilePublic = false,
  });

  Candidate.fromJson(dynamic json) {
    name = json["name"] != null ? json["name"] : "no name found";
    steamID = json["steamID"] != null ? json["steamID"] : ["invalid id"];
    isProfilePublic =
        json["isProfilePublic"] != null ? json["isProfilePublic"] : false;
    isSelected = json["isSelected"] != null ? json["isSelected"] : false;
  }

  Map<String, dynamic> saveState() => {
        'name': name,
        'steamID': steamID,
        'isProfilePublic': isProfilePublic,
        'isSelected': isSelected,
      };

  Map<String, dynamic> toJson() => {
        'name': name,
        'steamID': steamID,
        'isProfilePublic': isProfilePublic,
        'skill': {},
      };
}
