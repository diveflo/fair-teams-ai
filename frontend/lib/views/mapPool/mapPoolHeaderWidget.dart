import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/reducer/gameConfigReducer.dart';
import 'package:no_cry_babies/reducer/gameReducer.dart';
import 'package:no_cry_babies/state/appState.dart';

class MapPoolHeaderWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    void _onNextMap() {
      StoreProvider.of<AppState>(context).dispatch(NextMapAction());
      StoreProvider.of<AppState>(context).dispatch(SwapTeamsAction());
    }

    return Container(
      margin: EdgeInsets.only(bottom: 5),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Text(
            "Maps",
            style: TextStyle(fontWeight: FontWeight.bold, fontSize: 30),
          ),
          IconButton(
            icon: Icon(Icons.cached),
            onPressed: _onNextMap,
          ),
        ],
      ),
    );
  }
}
