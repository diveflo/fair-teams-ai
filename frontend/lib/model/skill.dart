class Skill {
  double skillScore;
  String form;

  Skill({this.skillScore = double.maxFinite, this.form = "none"});

  Skill.fromJson(dynamic json) {
    skillScore =
        json["skillScore"] != null ? json["skillScore"] : double.maxFinite;
    form = json["form"] != null ? json["form"] : "none";
  }

  dynamic toJson() => {
        "skillScore": skillScore,
        "form": form,
      };
}
