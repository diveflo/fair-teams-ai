import 'package:flutter/widgets.dart';
import 'package:frontend/model/candidate.dart';
import 'package:frontend/model/map.dart';

class GameConfigState {
  final List<Candidate> candidates;
  final bool includeBot;
  final MapPool mapPool;

  GameConfigState(
      {@required this.candidates,
      @required this.mapPool,
      this.includeBot = true});

  factory GameConfigState.initial() {
    return GameConfigState(
      candidates: [
        Candidate(name: "Flo", steamID: "76561197973591119"),
        Candidate(name: "Hubi", steamID: "76561198258023370"),
        Candidate(name: "Alex", steamID: "76561198011775117"),
        Candidate(name: "Sandy", steamID: "76561198011654217"),
        Candidate(name: "Markus", steamID: "76561197984050254"),
        Candidate(name: "Andi", steamID: "76561199045573415"),
        Candidate(name: "Martin", steamID: "76561197978519504"),
        Candidate(name: "Ferdy", steamID: "76561198031200891"),
        Candidate(name: "Niggo", steamID: "76561197995643389"),
        Candidate(name: "Chris", steamID: "76561197976972561"),
        Candidate(name: "Stefan", steamID: "76561198058595736"),
        Candidate(name: "Uwe", steamID: "76561198053826525"),
      ],
      mapPool: MapPool(),
      includeBot: true,
    );
  }

  factory GameConfigState.fromJson(dynamic json) {
    if (json != null) {
      List<Candidate> _candidates = parseList(json);
      bool _includeBot = json["includeBot"];
      return GameConfigState(
          candidates: _candidates,
          mapPool: MapPool.fromJson(json),
          includeBot: _includeBot);
    }
    return GameConfigState.initial();
  }

  dynamic toJson() => {
        "candidates":
            this.candidates.map((candidate) => candidate.saveState()).toList(),
        "mapPool": this.mapPool.toJson(),
        "includeBot": includeBot,
      };

  GameConfigState copyWith(
      {List<Candidate> candidates, MapPool mapPool, bool includeBot}) {
    return new GameConfigState(
      candidates: candidates ?? this.candidates,
      mapPool: mapPool ?? this.mapPool,
      includeBot: includeBot ?? this.includeBot,
    );
  }
}

List<Candidate> parseList(dynamic json) {
  List<Candidate> list = [];
  json["candidates"]
      .forEach((candidate) => list.add(Candidate.fromJson(candidate)));
  return list;
}
