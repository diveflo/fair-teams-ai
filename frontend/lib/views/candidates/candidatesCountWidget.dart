import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/model/candidate.dart';
import 'package:no_cry_babies/state/appState.dart';

class CandidateCountWidget extends StatelessWidget {
  const CandidateCountWidget({Key key}) : super(key: key);

  int _getActivePlayer(List<Candidate> players) {
    int count = 0;
    players.forEach((element) {
      if (element.isSelected) {
        count++;
      }
    });
    return count;
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: StoreConnector<AppState, List<Candidate>>(
          converter: (store) => store.state.gameConfigState.candidates,
          builder: (context, count) {
            return Text(
              "Players: ${_getActivePlayer(count)}",
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            );
          }),
    );
  }
}
