import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/skill.dart';

void main() {
  group("constructor", () {
    test("sets default parameter", () {
      Skill skill = Skill();

      expect(skill.form, "none");
      expect(skill.skillScore, double.maxFinite);
    });

    test("sets parameter", () {
      Skill skill = Skill(form: "rising", skillScore: 1.0);

      expect(skill.form, "rising");
      expect(skill.skillScore, 1.0);
    });
  });

  group("fromJson", () {
    test("set all parameter from json", () {
      var inputJson = {
        "skillScore": 1.0,
        "form": "dropping",
      };

      Skill skill = Skill.fromJson(inputJson);

      expect(skill.form, "dropping");
      expect(skill.skillScore, 1.0);
    });

    test("set default params if parameter are not in json", () {
      var inputJson = {};

      Skill skill = Skill.fromJson(inputJson);

      expect(skill.form, "none");
      expect(skill.skillScore, double.maxFinite);
    });
  });

  test("to json", () {
    Skill skill = Skill(form: "rising", skillScore: 2.2);
    var expectedJson = {
      "form": "rising",
      "skillScore": 2.2,
    };

    expect(skill.toJson(), expectedJson);
  });
}
