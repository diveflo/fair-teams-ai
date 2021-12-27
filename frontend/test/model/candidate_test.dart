import 'package:flutter_test/flutter_test.dart';
import 'package:no_cry_babies/model/candidate.dart';

void main() {
  test("toJson", () {
    Candidate candidate = Candidate(name: "player1", steamID: "player1id");
    Map<String, dynamic> expectedJson = {
      "name": "player1",
      "steamID": "player1id",
    };

    expect(candidate.toJson(), expectedJson);
  });

  test("saveState", () {
    Candidate candidate = Candidate(
      name: "player1",
      steamID: "player1id",
      isSelected: false,
    );
    Map<String, dynamic> expectedJson = {
      "name": "player1",
      "steamID": "player1id",
      "isSelected": false,
    };

    expect(candidate.saveState(), expectedJson);
  });
}
