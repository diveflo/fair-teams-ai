const String PLATEAU = "Plateau";
const String UPWARDS = "Upwards";
const String DOWNWARDS = "Downwards";

class Skill {
  double skillScore;
  String skillTrend;
  String rank;

  Skill(
      {this.skillScore = double.maxFinite,
      this.skillTrend = PLATEAU,
      this.rank = "NotRanked"});

  Skill.fromJson(dynamic json) {
    skillScore =
        json["skillScore"] != null ? json["skillScore"] : double.maxFinite;
    skillTrend = json["skillTrend"] != null ? json["skillTrend"] : PLATEAU;
    rank = json["rank"] != null ? json["rank"] : "NotRanked";
  }

  dynamic toJson() => {
        "skillScore": skillScore,
        "skillTrend": skillTrend,
        "rank": rank,
      };
}
