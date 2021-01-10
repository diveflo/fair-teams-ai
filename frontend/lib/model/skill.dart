class Skill {
  double skillScore;
  int skillTrend;

  Skill({this.skillScore = double.maxFinite, this.skillTrend = 0});

  Skill.fromJson(dynamic json) {
    skillScore =
        json["skillScore"] != null ? json["skillScore"] : double.maxFinite;
    skillTrend = json["skillTrend"] != null ? json["skillTrend"] : 0;
  }

  dynamic toJson() => {
        "skillScore": skillScore,
        "skillTrend": skillTrend,
      };
}
