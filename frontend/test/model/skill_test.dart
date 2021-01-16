import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/skill.dart';

void main() {
  group("constructor", () {
    test("sets default parameter", () {
      Skill skill = Skill();

      expect(skill.skillTrend, PLATEAU);
      expect(skill.skillScore, double.maxFinite);
      expect(skill.rank, "NotRanked");
    });

    test("sets parameter", () {
      Skill skill = Skill(skillTrend: UPWARDS, skillScore: 1.0, rank: "master");

      expect(skill.skillTrend, UPWARDS);
      expect(skill.skillScore, 1.0);
      expect(skill.rank, "master");
    });
  });

  group("fromJson", () {
    test("set all parameter from json", () {
      var inputJson = {
        "skillScore": 1.0,
        "skillTrend": "Upwards",
        "rank": "master"
      };

      Skill skill = Skill.fromJson(inputJson);

      expect(skill.skillTrend, UPWARDS);
      expect(skill.skillScore, 1.0);
      expect(skill.rank, "master");
    });

    test("set default params if parameter are not in json", () {
      var inputJson = {};

      Skill skill = Skill.fromJson(inputJson);

      expect(skill.skillTrend, PLATEAU);
      expect(skill.skillScore, double.maxFinite);
      expect(skill.rank, "NotRanked");
    });
  });

  test("to json", () {
    Skill skill = Skill(skillTrend: UPWARDS, skillScore: 2.2, rank: "master");
    var expectedJson = {
      "skillTrend": "Upwards",
      "skillScore": 2.2,
      "rank": "master"
    };

    expect(skill.toJson(), expectedJson);
  });
}
