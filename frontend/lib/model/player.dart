import 'package:frontend/model/skill.dart';

class Player {
  String name;
  String steamName;
  String steamID;
  Skill skill;

  bool isSelected;

  Player(
      {this.name,
      this.steamName = "Player1",
      this.steamID = "",
      this.isSelected = false,
      this.skill}) {
    if (skill == null) {
      skill = Skill();
    }
  }

  Player.fromJson(dynamic json) {
    name = json["name"] != null ? json["name"] : "no name found";
    steamName = json["steamName"] != null ? json["steamName"] : "Player1";
    steamID = json["steamID"] != null ? json["steamID"] : ["invalid id"];
    skill = Skill.fromJson(json["skill"]);
  }

  Map<String, dynamic> toJson() => {
        'name': name,
        'steamName': steamName,
        'steamID': steamID,
        'skill': skill.toJson(),
      };
}
