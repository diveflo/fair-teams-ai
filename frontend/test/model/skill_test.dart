import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/skill.dart';

void main() {
  group("constructor", () {
    test("sets default parameter", () {
      Skill skill = Skill();

      expect(skill.skillTrend, 0);
      expect(skill.skillScore, double.maxFinite);
    });

    test("sets parameter", () {
      Skill skill = Skill(skillTrend: 1, skillScore: 1.0);

      expect(skill.skillTrend, 1);
      expect(skill.skillScore, 1.0);
    });
  });

  group("fromJson", () {
    test("set all parameter from json", () {
      var inputJson = {
        "skillScore": 1.0,
        "skillTrend": 1,
      };

      Skill skill = Skill.fromJson(inputJson);

      expect(skill.skillTrend, 1);
      expect(skill.skillScore, 1.0);
    });

    test("set default params if parameter are not in json", () {
      var inputJson = {};

      Skill skill = Skill.fromJson(inputJson);

      expect(skill.skillTrend, 0);
      expect(skill.skillScore, double.maxFinite);
    });
  });

  test("to json", () {
    Skill skill = Skill(skillTrend: 1, skillScore: 2.2);
    var expectedJson = {
      "skillTrend": 1,
      "skillScore": 2.2,
    };

    expect(skill.toJson(), expectedJson);
  });
}
