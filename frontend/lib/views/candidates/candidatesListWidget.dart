import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/model/candidate.dart';
import 'package:no_cry_babies/reducer/gameConfigReducer.dart';
import 'package:no_cry_babies/state/appState.dart';

class CandidatesListWidget extends StatelessWidget {
  const CandidatesListWidget({
    Key key,
    @required ScrollController scrollController,
  })  : _scrollController = scrollController,
        super(key: key);

  final ScrollController _scrollController;

  @override
  Widget build(BuildContext context) {
    return Expanded(
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
                itemExtent: 35,
                itemBuilder: (BuildContext context, int index) {
                  return new CheckboxListTile(
                    title: Text(players[index].name,
                        style: Theme.of(context).primaryTextTheme.bodyText1),
                    value: players[index].isSelected,
                    controlAffinity: ListTileControlAffinity.leading,
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
    );
  }
}
