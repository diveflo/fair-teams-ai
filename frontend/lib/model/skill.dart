const String PLATEAU = "Plateau";
const String UPWARDS = "Upwards";
const String DOWNWARDS = "Downwards";

class Skill {
  double skillScore;
  String skillTrend;

  Skill({this.skillScore = double.maxFinite, this.skillTrend = PLATEAU});

  Skill.fromJson(dynamic json) {
    skillScore =
        json["skillScore"] != null ? json["skillScore"] : double.maxFinite;
    skillTrend = json["skillTrend"] != null ? json["skillTrend"] : PLATEAU;
  }

  dynamic toJson() => {
        "skillScore": skillScore,
        "skillTrend": skillTrend,
      };
}
