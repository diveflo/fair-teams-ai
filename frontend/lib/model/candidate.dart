import 'package:flutter/widgets.dart';

class Candidate {
  String name;
  String steamID;
  bool isSelected;

  Candidate({
    @required this.name,
    this.steamID = "",
    this.isSelected = false,
  });

  Candidate.fromJson(dynamic json) {
    name = json["name"] != null ? json["name"] : "no name found";
    steamID = json["steamID"] != null ? json["steamID"] : ["invalid id"];
    isSelected = json["isSelected"] != null ? json["isSelected"] : false;
  }

  Map<String, dynamic> saveState() => {
        'name': name,
        'steamID': steamID,
        'isSelected': isSelected,
      };

  Map<String, dynamic> toJson() => {
        'name': name,
        'steamID': steamID,
      };
}
