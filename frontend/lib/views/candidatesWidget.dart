import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/model/candidate.dart';
import 'package:frontend/reducer/gameConfigReducer.dart';
import 'package:frontend/state/appState.dart';

class CandidatesWidget extends StatefulWidget {
  @override
  _CandidatesWidgetState createState() => _CandidatesWidgetState();
}

class _CandidatesWidgetState extends State<CandidatesWidget> {
  ScrollController _scrollController;

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
  initState() {
    _scrollController = ScrollController();
    super.initState();
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 60),
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.only(bottom: 10),
            child: StoreConnector<AppState, List<Candidate>>(
                converter: (store) => store.state.gameConfigState.candidates,
                builder: (context, count) {
                  return Text(
                    "Players: ${_getActivePlayer(count)}",
                    style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                  );
                }),
          ),
          Expanded(
            child: Container(
              child: Scrollbar(
                isAlwaysShown: true,
                controller: _scrollController,
                child: StoreConnector<AppState, List<Candidate>>(
                  converter: (store) => store.state.gameConfigState.candidates,
                  builder: (context, players) {
                    return ListView.builder(
                      controller: _scrollController,
                      itemCount: players.length,
                      itemBuilder: (BuildContext context, int index) {
                        return new CheckboxListTile(
                          title: Text(players[index].name,
                              style:
                                  Theme.of(context).primaryTextTheme.bodyText1),
                          value: players[index].isSelected,
                          onChanged: (bool value) {
                            StoreProvider.of<AppState>(context).dispatch(
                                TogglePlayerSelectionAction(players[index]));
                          },
                        );
                      },
                    );
                  },
                ),
              ),
            ),
          )
        ],
      ),
    );
  }
}
